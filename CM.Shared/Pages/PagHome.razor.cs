namespace CM.Shared {

    /// <summary>Landing page: search, the collection at a glance, and what the catalog holds.</summary>
    public class PagHome_Base : BaseAppStatePage {

        // Published to AppState.ActiveMenuKey while this page is displayed.
        protected override string ActiveMenuKey => "Home";

        protected string _displayName => _appState?.CurrentLoggedInUser?.DisplayName ?? "";

        protected string _search = "";
        protected CatalogStatsRow _stats = new();
        protected List<MarketCardRow> _topCards = new();

        protected int _itemCount;
        protected decimal? _value;
        protected decimal? _gain;
        protected string _topName = "";
        protected decimal? _topValue;

        protected override async Task LoadPageDataAsync() {
            _stats = await CatalogQueries.GetStatsAsync();
            _topCards = await MarketQueries.GetMostValuableAsync(false, 12);

            List<CollectionItemRow> oItems = await CollectionQueries.GetItemsAsync(GetAppState().CollectorOid ?? 0);
            _itemCount = oItems.Count;
            if(_itemCount == 0) return;

            List<CollectionItemRow> oPriced = oItems.Where(oItem => oItem.Value != null).ToList();
            List<CollectionItemRow> oWithBoth = oItems.Where(oItem => oItem.Gain != null).ToList();
            _value = oPriced.Count == 0 ? null : oPriced.Sum(oItem => oItem.Value ?? 0m);
            _gain = oWithBoth.Count == 0 ? null : oWithBoth.Sum(oItem => oItem.Gain ?? 0m);

            CollectionItemRow? oTop = oPriced.OrderByDescending(oItem => oItem.Value).FirstOrDefault();
            _topName = oTop?.Name ?? "-";
            _topValue = oTop?.Value;
        }

        protected void Search() {
            _nav.NavigateTo($"{CMConstants.SearchRoute}?q={Uri.EscapeDataString(_search.Trim())}");
        }
    }
}
