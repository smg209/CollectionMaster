using Microsoft.AspNetCore.Components;

namespace CM.Shared {

    /// <summary>
    /// Catalog-wide search. The text lives in the address (?q=...), so a search can be
    /// bookmarked, and the Back button returns to the results.
    /// </summary>
    public class PagSearch_Base : BaseAppStatePage {

        public const int MaxRows = 120;

        [SupplyParameterFromQuery(Name = "q")] public string? Q { get; set; }

        protected override string ActiveMenuKey => "Search";

        // The search text is part of what the page shows, so Back / Forward between searches reload the results.
        protected override string PageKey => $"search {Q}";

        protected string _text = "";
        protected string _searched = "";
        protected bool _isSearching;
        protected List<CatalogCardRow> _results = new();
        protected string _sort = "price";
        protected string _show = "all";

        protected override async Task LoadPageDataAsync() {
            _text = Q ?? "";
            await RunSearchAsync();
        }

        /// <summary>The results after the kind filter, in the chosen order. SearchCardsAsync returns them most valuable first.</summary>
        protected List<CatalogCardRow> Visible() {
            IEnumerable<CatalogCardRow> oCards = _results.Where(oCard => _show switch {
                "cards" => !oCard.IsSealed,
                "sealed" => oCard.IsSealed,
                "owned" => oCard.OwnedCount > 0,
                _ => true
            });

            return (_sort switch {
                "priceAsc" => oCards.OrderBy(oCard => oCard.MarketPrice ?? decimal.MaxValue),
                "newest" => oCards.OrderByDescending(oCard => oCard.SetReleasedOn),
                "oldest" => oCards.OrderBy(oCard => oCard.SetReleasedOn ?? DateTime.MaxValue),
                "name" => oCards.OrderBy(oCard => oCard.DisplayName),
                _ => oCards
            }).ToList();
        }

        /// <summary>Puts the text in the address; the changed address is what runs the search (see PageKey).</summary>
        protected void Search() {
            string sText = _text.Trim();
            if(sText == (Q ?? "").Trim()) return;
            _nav.NavigateTo($"{CMConstants.SearchRoute}?q={Uri.EscapeDataString(sText)}");
        }

        private async Task RunSearchAsync() {
            string sText = _text.Trim();
            _searched = sText;
            _results = new();
            if(sText.Length == 0) return;

            _isSearching = true;

            try {
                _results = await CatalogQueries.SearchCardsAsync(sText, GetAppState().CollectorOid ?? 0, MaxRows);
            } catch(Exception oEx) {
                _loadError = oEx.Message;
            } finally {
                _isSearching = false;
            }
        }
    }
}
