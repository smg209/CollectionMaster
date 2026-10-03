using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace CM.Shared {

    /// <summary>One rarity of a set: how many cards it has and what they cost.</summary>
    public class RarityRow {
        public string Rarity { get; set; } = "";
        public int Cards { get; set; }
        public int Priced { get; set; }
        public decimal? Average { get; set; }
        public decimal? Total { get; set; }
        public CatalogCardRow? Top { get; set; }
    }

    /// <summary>
    /// The cards of one set: language choice for names and pictures, filters, sort, and how
    /// much of the set the collector owns.
    /// </summary>
    public class PagSetCards_Base : BaseAppStatePage {

        [Parameter] public long CardSetOid { get; set; }

        protected override string ActiveMenuKey => "Catalog";

        protected override string PageKey => $"set {CardSetOid}";

        protected CatalogSetRow? _set;
        protected List<CatalogNameRow> _setNames = new();
        protected List<CatalogCardRow> _cards = new();
        protected List<string> _rarities = new();
        protected long _languageOid;

        protected string _filter = "";
        protected string _rarity = "";
        protected string _show = "cards";
        protected string _sort = "number";

        protected string _languageCode => _setNames.FirstOrDefault(oName => oName.LanguageOid == _languageOid)?.LanguageCode ?? "";

        private long _collectorOid => GetAppState().CollectorOid ?? 0;

        protected override async Task LoadPageDataAsync() {
            _set = await CatalogQueries.GetSetAsync(CardSetOid, _collectorOid);
            if(_set == null) return;

            _setNames = await CatalogQueries.GetSetNamesAsync(CardSetOid);

            // Start in English where the set has an English name, otherwise in its own language.
            _languageOid = _setNames.FirstOrDefault(oName => oName.LanguageCode == "en")?.LanguageOid ?? _set.LanguageOid_Primary;
            await LoadCardsAsync(_languageOid);
        }

        private async Task LoadCardsAsync(long tlLanguageOid) {
            _cards = await CatalogQueries.GetCardsAsync(CardSetOid, tlLanguageOid, _collectorOid);
            _rarities = _cards.Where(oCard => !oCard.IsSealed && !string.IsNullOrEmpty(oCard.Rarity))
                              .Select(oCard => oCard.Rarity!)
                              .Distinct()
                              .OrderBy(sRarity => sRarity)
                              .ToList();
        }

        protected async Task SelectLanguageAsync(long tlLanguageOid) {
            if(tlLanguageOid == _languageOid) return;

            try {
                await LoadCardsAsync(tlLanguageOid);
                _languageOid = tlLanguageOid;
            } catch(Exception oEx) {
                _loadError = oEx.Message;
            }
        }

        /// <summary>The cards after the filters, in the chosen order. GetCardsAsync already returns card-number order.</summary>
        protected List<CatalogCardRow> Visible() {
            string sFilter = _filter.Trim();

            IEnumerable<CatalogCardRow> oCards = _cards.Where(oCard => _show switch {
                "sealed" => oCard.IsSealed,
                "owned" => oCard.OwnedCount > 0,
                "missing" => !oCard.IsSealed && oCard.OwnedCount == 0,
                _ => !oCard.IsSealed
            });

            if(_rarity.Length > 0) oCards = oCards.Where(oCard => oCard.Rarity == _rarity);

            if(sFilter.Length > 0) {
                oCards = oCards.Where(oCard =>
                    oCard.DisplayName.Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                    || (oCard.Name ?? "").Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                    || (oCard.CardNumber ?? "").Equals(sFilter, StringComparison.OrdinalIgnoreCase));
            }

            oCards = _sort switch {
                "priceDesc" => oCards.OrderByDescending(oCard => oCard.MarketPrice ?? -1m),
                "priceAsc" => oCards.OrderBy(oCard => oCard.MarketPrice ?? decimal.MaxValue),
                "name" => oCards.OrderBy(oCard => oCard.DisplayName),
                _ => oCards
            };

            return oCards.ToList();
        }

        /// <summary>The set's cards grouped by rarity, dearest rarity first. Sealed products are left out.</summary>
        protected List<RarityRow> RarityBreakdown() {
            return _cards
                .Where(oCard => !oCard.IsSealed)
                .GroupBy(oCard => string.IsNullOrEmpty(oCard.Rarity) ? "None" : oCard.Rarity!)
                .Select(oGroup => {
                    List<CatalogCardRow> oPriced = oGroup.Where(oCard => oCard.MarketPrice != null).ToList();
                    return new RarityRow {
                        Rarity = oGroup.Key,
                        Cards = oGroup.Count(),
                        Priced = oPriced.Count,
                        Average = oPriced.Count == 0 ? null : Math.Round(oPriced.Average(oCard => oCard.MarketPrice ?? 0m), 2),
                        Total = oPriced.Count == 0 ? null : oPriced.Sum(oCard => oCard.MarketPrice ?? 0m),
                        Top = oPriced.OrderByDescending(oCard => oCard.MarketPrice).FirstOrDefault()
                    };
                })
                .OrderByDescending(oRow => oRow.Average ?? -1m)
                .ToList();
        }

        /// <summary>Clicking a rarity in the breakdown filters the grid to it.</summary>
        protected void FilterRarity(string tsRarity) {
            _rarity = _rarities.Contains(tsRarity) ? tsRarity : "";
            _show = "cards";
        }

        protected string OwnedPercent() {
            if(_set == null) return "0";
            return (100m * _set.OwnedCards / Math.Max(1, _set.CardCount)).ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}
