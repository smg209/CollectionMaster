using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace CM.Shared {

    /// <summary>One tab of the top menu: its text, where it goes, and every route that keeps it highlighted.</summary>
    public class MainMenuTab {
        public string Text { get; set; } = "";
        public string Route { get; set; } = "";
        public string[] Routes { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// The frame around every signed-in page. A tab stays highlighted for all of its pages -
    /// "Catalog" covers the set list, a set's cards and a single card - which NavLink's own
    /// prefix matching cannot express, so the active tab is worked out from the address here.
    /// </summary>
    public class MainLayout_Base : LayoutComponentBase, IDisposable {

        [Inject] protected NavigationManager _nav { get; set; } = default!;

        protected readonly List<MainMenuTab> _tabs = new() {
            new MainMenuTab { Text = "Home", Route = CMConstants.HomeRoute, Routes = new[] { CMConstants.HomeRoute } },
            new MainMenuTab { Text = "Catalog", Route = CMConstants.CatalogRoute, Routes = new[] { CMConstants.CatalogRoute, CMConstants.SetCardsRoute, CMConstants.CardRoute } },
            new MainMenuTab { Text = "Search", Route = CMConstants.SearchRoute, Routes = new[] { CMConstants.SearchRoute } },
            new MainMenuTab { Text = "My Collection", Route = CMConstants.CollectionRoute, Routes = new[] { CMConstants.CollectionRoute } },
            new MainMenuTab { Text = "Market", Route = CMConstants.MarketRoute, Routes = new[] { CMConstants.MarketRoute } },
        };

        protected override void OnInitialized() {
            _nav.LocationChanged += OnLocationChanged;
        }

        protected bool IsActive(MainMenuTab toTab) {
            string sPath = "/" + _nav.ToBaseRelativePath(_nav.Uri).Split('?', '#')[0];
            return toTab.Routes.Any(sRoute =>
                sPath.Equals(sRoute, StringComparison.OrdinalIgnoreCase) ||
                sPath.StartsWith(sRoute + "/", StringComparison.OrdinalIgnoreCase));
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose() {
            _nav.LocationChanged -= OnLocationChanged;
        }
    }
}
