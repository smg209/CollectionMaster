namespace CM.Shared {

    /// <summary>The market as a whole: what moved, and what is worth the most.</summary>
    public class PagMarket_Base : BaseAppStatePage {

        public static readonly int[] Windows = { 1, 3, 7, 30, 90 };
        public const decimal MinimumPrice = 2m;

        protected override string ActiveMenuKey => "Market";

        protected MarketHistoryDepthRow _depth = new();
        protected List<MarketCardRow> _gainers = new();
        protected List<MarketCardRow> _losers = new();
        protected List<MarketCardRow> _topCards = new();
        protected List<MarketCardRow> _topSealed = new();
        protected int _days = 7;

        protected override async Task LoadPageDataAsync() {
            _depth = await MarketQueries.GetHistoryDepthAsync();
            _topCards = await MarketQueries.GetMostValuableAsync(false, 50);
            _topSealed = await MarketQueries.GetMostValuableAsync(true, 25);

            // Open on the longest of the short windows that already has movers, so the page is not
            // empty while the stored history is still only a few days deep.
            foreach(int iDays in new[] { 7, 3, 1 }) {
                _days = iDays;
                await LoadMoversAsync();
                if(_gainers.Count > 0 || _losers.Count > 0) break;
            }
        }

        private async Task LoadMoversAsync() {
            _gainers = await MarketQueries.GetMoversAsync(_days, true, MinimumPrice);
            _losers = await MarketQueries.GetMoversAsync(_days, false, MinimumPrice);
        }

        protected async Task SelectWindowAsync(int tiDays) {
            if(tiDays == _days) return;

            try {
                _days = tiDays;
                await LoadMoversAsync();
            } catch(Exception oEx) {
                _loadError = oEx.Message;
            }
        }
    }
}
