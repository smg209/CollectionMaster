using Blazored.LocalStorage;
using Blazored.SessionStorage;
using CM;
using CM.Shared;
using FSAuth.Core;
using FSAuth.Server;
using FSCommon;
using FSDataUtil.Core;
using FSDataUtil.Server;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Static web assets from the referenced Razor class libraries (CM.Shared, FSCommon.Blazor,
// MudBlazor) are only enabled automatically in the Development environment. Enabling them
// explicitly keeps "_content/..." working when the app is started from the build output under
// any other environment name.
builder.WebHost.UseStaticWebAssets();

// -- 1. Configuration ---------------------------------------------------------------------
// Loaded explicitly so the order is guaranteed. appsettings.Development.json is NOT committed
// (see .gitignore) - it carries the connection strings and the real JWT secret.
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Sign-in can be switched off for development ("Authentication:RequireSignIn": false).
// Missing from configuration means ON. See CMSettings.RequireSignIn for what OFF does.
CMSettings.RequireSignIn = builder.Configuration.GetValue("Authentication:RequireSignIn", true);

// -- 2. JWT Settings ----------------------------------------------------------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// -- 3. ORM - REGISTRATION PHASE (before Build) -------------------------------------------
// Connection string keys must exactly match Record<T>.DatabaseName. DbPool reads the
// ConnectionStrings section itself, including the StartupDatabases list.
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddSingleton<ConnectionStrings>(sp => sp.GetRequiredService<IOptions<ConnectionStrings>>().Value);
builder.Services.AddSingleton<DbPool>();
// NOTE: DbPoolAccessor.Instance is NOT set here - it cannot be set until after Build().

// -- 4. Identity Infrastructure -----------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();

// One AppStateManager instance behind both interfaces, so the in-memory AppState dictionary is
// the same no matter which interface a consumer asks for: CM.Shared.IAppStateManager for the
// storage helpers, FSCommon.IAppStateManager<AppState_CM> for FSCommon.BaseAppStatePage<TAppState>.
builder.Services.AddSingleton<AppStateManager>();
builder.Services.AddSingleton<IAppStateManager>(sp => sp.GetRequiredService<AppStateManager>());
builder.Services.AddSingleton<FSCommon.IAppStateManager<AppState_CM>>(sp => sp.GetRequiredService<AppStateManager>());

// -- 5. FSAuthentication ------------------------------------------------------------------
// FSAuthenticationService implements IFSAuthenticationService directly. Register the concrete
// type, then map the interface onto that same singleton instance.
builder.Services.AddSingleton<FSAuthenticationService>();
builder.Services.AddSingleton<IFSAuthenticationService>(sp => sp.GetRequiredService<FSAuthenticationService>());

// -- 6. SignalR ---------------------------------------------------------------------------
builder.Services.AddSignalR();

// -- 7. Storage and UI Services -----------------------------------------------------------
// The two converters let UserAuth_DTO (interface-typed Roles/Permissions collections) survive
// the localStorage round trip AppStateManager uses to restore a session after a server restart.
builder.Services.AddBlazoredLocalStorage(toOptions => {
    toOptions.JsonSerializerOptions.Converters.Add(new RoleDTOJsonConverter());
    toOptions.JsonSerializerOptions.Converters.Add(new PermissionDTOJsonConverter());
});
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddMudServices();

// FSCommon.ThemeService is the shared, app-agnostic registry. CollectionMaster registers
// FSCommon.Blazor's own theme classes - it has no app-specific colors yet, so there is no
// app-local theme tier (BaseAppTheme/AppThemeService) to wire in.
builder.Services.AddScoped<ThemeService>(sp => {
    var oSvc = new ThemeService(sp.GetRequiredService<IJSRuntime>());
    oSvc.Register("Ocean", () => new OceanTheme());
    oSvc.Register("Light", () => new LightTheme());
    oSvc.Register("Dark", () => new DarkTheme());
    oSvc.Register("Forest", () => new ForestTheme());
    oSvc.Register("Desert", () => new DesertTheme());
    oSvc.Register("GunMetal", () => new GunMetalTheme());
    return oSvc;
});
builder.Services.AddScoped<SpinnerService>();
builder.Services.AddScoped<FSModalService>();
builder.Services.AddScoped<MessageBoxService>();

// -- 8. Razor Components - InteractiveServer ONLY during development ----------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    //.AddInteractiveWebAssemblyComponents(); // Uncomment before deploy - flip to InteractiveAuto

builder.Services.Configure<CircuitOptions>(toOptions => {
    toOptions.DetailedErrors = true;  // Remove or set false in production
});

// -- 9. Logging ---------------------------------------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

var app = builder.Build();

// -- 10. ORM - HYDRATION PHASE (after Build - this is the critical step) ------------------
// DbPool is now fully constructed by the DI container. Assigning it to the static accessor is
// what lets every Record<T> class reach it. Nothing that touches the ORM can run before this.
var oDbPool = app.Services.GetRequiredService<DbPool>();
DbPoolAccessor.Instance = oDbPool;

// Optional smoke test - confirms the ORM and the Authentication database are connected:
// var oUser = UserAuth.First("Oid = @0", 1);

// -- 11. HTTP Pipeline --------------------------------------------------------------------
if(app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
} else {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// An unknown URL re-executes as /not-found (PagNotFound) so the visitor gets a real page with a
// 404 status instead of an empty response.
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Endpoints for build-time static web assets (fingerprinted, compressed). On .NET 10 the Blazor
// script (_framework/blazor.web.js) is one of them - see RequiresAspNetWebAssets in CM.csproj.
app.MapStaticAssets();

// -- 12. SignalR Hub ----------------------------------------------------------------------
app.MapHub<CMHub>(CMConstants.HubPath);

// -- 13. Razor Components -----------------------------------------------------------------
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
    //.AddInteractiveWebAssemblyRenderMode()  // Uncomment with step 8 for deploy
    //.AddAdditionalAssemblies(typeof(CM.Client._Imports).Assembly)

// -- 14. Auth Cookie Endpoints ------------------------------------------------------------
const string UidCookieName = "uid";
const string UidProtectorPurpose = "UidCookie.v1";

app.MapPost("/auth/exchange", (UserToken toToken, IDataProtectionProvider toProtection, HttpContext toContext) => {
    if(toToken is null || toToken.UserOid <= 0) return Results.BadRequest("Invalid userOid.");

    var oProtector = toProtection.CreateProtector(UidProtectorPurpose);
    string sValue = oProtector.Protect(toToken.UserOid.ToString(CultureInfo.InvariantCulture));

    toContext.Response.Cookies.Append(UidCookieName, sValue, new CookieOptions {
        HttpOnly = true,
        // Matches the request's own scheme instead of a hard-coded true: a Secure cookie set on a
        // plain-HTTP response (local dev on http://localhost) is never sent back by the browser,
        // which makes /auth/whoami report "not signed in" on every call. Still Secure in
        // production, where the app is served over HTTPS.
        Secure = toContext.Request.IsHttps,
        SameSite = SameSiteMode.Lax,
        IsEssential = true,
        Expires = DateTimeOffset.UtcNow.AddDays(7),
        Path = "/"
    });

    return Results.Ok();
});

app.MapGet("/auth/whoami", (IDataProtectionProvider toProtection, HttpContext toContext) => {
    if(!toContext.Request.Cookies.TryGetValue(UidCookieName, out var sRaw)) return Results.NoContent();

    var oProtector = toProtection.CreateProtector(UidProtectorPurpose);
    string sUnprotected;
    try { sUnprotected = oProtector.Unprotect(sRaw); } catch { return Results.Unauthorized(); }

    if(!long.TryParse(sUnprotected, NumberStyles.None, CultureInfo.InvariantCulture, out var lUserOid))
        return Results.Unauthorized();

    return Results.Ok(new { userOid = lUserOid });
});

app.MapPost("/auth/logout", (HttpContext toContext) => {
    toContext.Response.Cookies.Delete(UidCookieName, new CookieOptions { Path = "/" });
    return Results.Ok();
});

app.Run();
