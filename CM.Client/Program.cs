using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// The WebAssembly runtime for InteractiveAuto. Not used while the server runs InteractiveServer
// only - see the two commented lines in CM/Program.cs (steps 8 and 13) for the flip.
var builder = WebAssemblyHostBuilder.CreateDefault(args);

await builder.Build().RunAsync();
