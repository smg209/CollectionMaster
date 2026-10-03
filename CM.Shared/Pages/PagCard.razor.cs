using Microsoft.AspNetCore.Components;

namespace CM.Shared {

    /// <summary>A link out to a marketplace or price site for the card being viewed.</summary>
    public class MarketLink {
        public string Text { get; set; } = "";
        public string Url { get; set; } = "";
        public string Note { get; set; } = "";
    }

    /// <summary>
    /// One card: its picture and names in each language, every version of it with every
    /// current price from every source, how those prices have moved, where it sells, and the
    /// form that adds a copy to the collection.
    /// </summary>
    public class PagCard_Base : BaseAppStatePage {

        [Parameter] public long CollectibleOid { get; set; }

        /// <summary>Language code carried over from the set page ("fr"), so the card opens in the same language.</summary>
        [SupplyParameterFromQuery(Name = "lang")] public string? Lang { get; set; }

        protected override string ActiveMenuKey => "Catalog";

        protected override string PageKey => $"card {CollectibleOid}";

        #region Fields
        protected CatalogCardDetailRow? _card;
        protected List<CatalogNameRow> _names = new();
        protected List<CatalogImageRow> _images = new();
        protected List<CatalogVariantRow> _variants = new();
        protected List<CatalogVariantExternalIdRow> _externalIds = new();
        protected List<CatalogPriceRow> _prices = new();
        protected List<CatalogPriceHistoryRow> _history = new();
        protected List<MarketLink> _links = new();
        protected List<CatalogCardRow> _sameName = new();

        protected List<LookupRow> _conditions = new();
        protected List<LookupRow> _companies = new();
        protected List<LookupRow> _grades = new();

        protected long _languageOid;
        protected long? _historyPriceOid;

        // The add-to-collection form. CollectibleVariantOid 0 means the form is closed.
        protected AddToCollectionRequest _add = new();
        protected string _addKind = "raw";
        protected bool _isSaving;
        protected string _message = "";
        protected bool _messageIsError;
        #endregion (Fields)

        private long _collectorOid => GetAppState().CollectorOid ?? 0;

        protected CatalogImageRow? _image => _images.FirstOrDefault(oImage => oImage.LanguageOid == _languageOid) ?? _images.FirstOrDefault();

        protected string _displayName => _names.FirstOrDefault(oName => oName.LanguageOid == _languageOid)?.Name ?? _card?.Name ?? "";

        protected int _sourceCount => _prices.Select(oPrice => oPrice.SourceName).Distinct().Count();

        /// <summary>
        /// The headline price: the first version (plain printings come first) that has a
        /// sales-based raw USD price - Near Mint, or not split by condition. Asking prices are
        /// never the headline.
        /// </summary>
        private CatalogPriceRow? Headline() {
            foreach(CatalogVariantRow oVariant in _variants) {
                CatalogPriceRow? oPrice = SalesPrices()
                    .Where(oRow => oRow.CollectibleVariantOid == oVariant.CollectibleVariantOid)
                    .OrderBy(oRow => oRow.SourceName == "TCGplayer" ? 0 : 1)
                    .FirstOrDefault();
                if(oPrice != null) return oPrice;
            }
            return null;
        }

        protected decimal? _headlinePrice => Headline()?.Price;

        protected string _headlineNote {
            get {
                CatalogPriceRow? oTop = Headline();
                if(oTop == null) return "No sales-based price yet";
                if(_variants.Count <= 1) return $"{oTop.SourceName}, {CMFormat.Date(oTop.LastConfirmed)}";

                string sVersion = _variants.FirstOrDefault(oVariant => oVariant.CollectibleVariantOid == oTop.CollectibleVariantOid)?.DisplayName ?? "";
                List<decimal> oAll = SalesPrices().Select(oPrice => oPrice.Price).ToList();
                return $"{sVersion}. Versions range {CMFormat.Money(oAll.Min())} to {CMFormat.Money(oAll.Max())}";
            }
        }

        private IEnumerable<CatalogPriceRow> SalesPrices() {
            return _prices.Where(oPrice => !oPrice.IsGraded && oPrice.CurrencyCode == "USD" && oPrice.SourceName != "TCGplayer listings"
                && (string.IsNullOrEmpty(oPrice.ConditionName) || oPrice.ConditionName == "Near Mint"));
        }

        #region Loading
        protected override async Task LoadPageDataAsync() {
            // This also runs when the page moves from one card to another: nothing of the last card may linger.
            _historyPriceOid = null;
            _add = new AddToCollectionRequest();
            _message = "";

            _card = await CatalogQueries.GetCardAsync(CollectibleOid);
            if(_card == null) return;

            _names = await CatalogQueries.GetCardNamesAsync(CollectibleOid);
            _images = await CatalogQueries.GetCardImagesAsync(CollectibleOid);
            _externalIds = await CatalogQueries.GetVariantExternalIdsAsync(CollectibleOid);
            _prices = await CatalogQueries.GetPricesAsync(CollectibleOid);
            _history = await CatalogQueries.GetCardPriceHistoryAsync(CollectibleOid);
            await LoadVariantsAsync();
            _sameName = _card.IsSealed ? new List<CatalogCardRow>() : await CatalogQueries.GetSameNameCardsAsync(CollectibleOid, _collectorOid);

            _conditions = await CatalogQueries.GetConditionsAsync();
            _companies = await CatalogQueries.GetGradingCompaniesAsync();
            _grades = await CatalogQueries.GetGradesAsync();

            // The language asked for, else English, else whatever the card has.
            _languageOid = _names.FirstOrDefault(oName => oName.LanguageCode == Lang)?.LanguageOid
                ?? _names.FirstOrDefault(oName => oName.LanguageCode == "en")?.LanguageOid
                ?? _names.FirstOrDefault()?.LanguageOid
                ?? 0;

            BuildLinks();
        }

        private async Task LoadVariantsAsync() {
            _variants = await CatalogQueries.GetVariantsAsync(CollectibleOid, _collectorOid);
        }

        /// <summary>
        /// Links to where the card is listed and where its sales can be checked. The product
        /// links use the marketplace's own id for the first version that has one; the searches
        /// are built from the English name, the card number and the set.
        /// </summary>
        private void BuildLinks() {
            _links = new();
            if(_card == null) return;

            string? sTcgplayer = FirstExternalId("TCGplayer");
            string? sCardmarket = FirstExternalId("Cardmarket");

            string sEnglish = _names.FirstOrDefault(oName => oName.LanguageCode == "en")?.Name ?? _card.Name ?? "";
            string sNumber = _card.IsSealed || string.IsNullOrEmpty(_card.CardNumber) ? ""
                : _card.CardCountOfficial != null ? $"{_card.CardNumber}/{_card.CardCountOfficial}" : _card.CardNumber;
            string sQuery = Uri.EscapeDataString($"pokemon {sEnglish} {sNumber} {_card.SetName}".Replace("  ", " ").Trim());

            if(!string.IsNullOrEmpty(sTcgplayer)) {
                _links.Add(new MarketLink { Text = "TCGplayer listings", Url = $"https://www.tcgplayer.com/product/{sTcgplayer}", Note = "Copies for sale now, by condition" });
            }
            if(!string.IsNullOrEmpty(sCardmarket)) {
                _links.Add(new MarketLink { Text = "Cardmarket listings", Url = $"https://www.cardmarket.com/en/Pokemon/Products?idProduct={sCardmarket}", Note = "European listings, in euros" });
            }
            _links.Add(new MarketLink { Text = "eBay sold listings", Url = $"https://www.ebay.com/sch/i.html?_nkw={sQuery}&LH_Sold=1&LH_Complete=1", Note = "What copies actually sold for in the last 90 days" });
            _links.Add(new MarketLink { Text = "eBay for sale", Url = $"https://www.ebay.com/sch/i.html?_nkw={sQuery}", Note = "Copies listed on eBay now" });
            _links.Add(new MarketLink { Text = "PriceCharting", Url = $"https://www.pricecharting.com/search-products?q={sQuery}&type=prices", Note = "Raw and graded sales history" });
        }
        /// <summary>The marketplace's id for the first version (in display order) that has one - the plain version where it exists.</summary>
        private string? FirstExternalId(string tsSourceName) {
            foreach(CatalogVariantRow oVariant in _variants) {
                string? sId = _externalIds.FirstOrDefault(oId => oId.CollectibleVariantOid == oVariant.CollectibleVariantOid && oId.SourceName == tsSourceName)?.ExternalId;
                if(!string.IsNullOrEmpty(sId)) return sId;
            }
            return null;
        }
        #endregion (Loading)

        #region Display helpers
        protected void SelectLanguage(long tlLanguageOid) {
            _languageOid = tlLanguageOid;
        }

        protected List<CatalogPriceRow> PricesOf(long tlCollectibleVariantOid) {
            return _prices.Where(oPrice => oPrice.CollectibleVariantOid == tlCollectibleVariantOid).ToList();
        }

        protected List<CatalogPriceHistoryRow> HistoryOf(long tlCollectiblePriceOid) {
            return _history.Where(oRow => oRow.CollectiblePriceOid == tlCollectiblePriceOid)
                           .OrderByDescending(oRow => oRow.FirstConfirmed)
                           .ToList();
        }

        /// <summary>"TCGplayer 42382 - Cardmarket 273699"</summary>
        protected string ExternalIdsOf(long tlCollectibleVariantOid) {
            return string.Join(" - ", _externalIds
                .Where(oId => oId.CollectibleVariantOid == tlCollectibleVariantOid)
                .Select(oId => $"{oId.SourceName} {oId.ExternalId}"));
        }

        /// <summary>One version's raw price from one source for the comparison table: Near Mint where the source splits by condition.</summary>
        protected string Summary(long tlCollectibleVariantOid, string tsSourceName) {
            CatalogPriceRow? oPrice = _prices
                .Where(oRow => oRow.CollectibleVariantOid == tlCollectibleVariantOid && oRow.SourceName == tsSourceName && !oRow.IsGraded)
                .OrderBy(oRow => oRow.ConditionSort ?? 0)
                .FirstOrDefault();
            if(oPrice != null) return CMFormat.Money(oPrice.Price, oPrice.CurrencyCode);

            // No sales-based TCGplayer price: say what the cheapest copy is listed at instead.
            if(tsSourceName == "TCGplayer") {
                CatalogPriceRow? oListing = _prices.FirstOrDefault(oRow => oRow.CollectibleVariantOid == tlCollectibleVariantOid && oRow.SourceName == "TCGplayer listings");
                if(oListing != null) return $"listed from {CMFormat.Money(oListing.Price, oListing.CurrencyCode)}";
            }
            return "";
        }

        protected string BestGraded(long tlCollectibleVariantOid) {
            CatalogPriceRow? oPrice = _prices
                .Where(oRow => oRow.CollectibleVariantOid == tlCollectibleVariantOid && oRow.IsGraded)
                .OrderByDescending(oRow => oRow.Price)
                .FirstOrDefault();
            return oPrice == null ? "" : $"{CMFormat.Money(oPrice.Price, oPrice.CurrencyCode)} ({oPrice.GradeOrCondition})";
        }

        /// <summary>
        /// Chart lines for one version: each raw USD price that has moved at least once. Every
        /// history row is a span, so it contributes its start, and the newest row also its end -
        /// the line then runs up to the last time the source was checked.
        /// </summary>
        protected List<ChartSeries> ChartOf(List<CatalogPriceRow> toPrices) {
            var oSeries = new List<ChartSeries>();

            foreach(CatalogPriceRow oPrice in toPrices.Where(oRow => !oRow.IsGraded && oRow.CurrencyCode == "USD" && oRow.SourceName != "TCGplayer listings")) {
                List<CatalogPriceHistoryRow> oRows = _history
                    .Where(oRow => oRow.CollectiblePriceOid == oPrice.CollectiblePriceOid)
                    .OrderBy(oRow => oRow.FirstConfirmed)
                    .ToList();
                if(oRows.Count == 0) continue;

                var oPoints = oRows.Select(oRow => new ChartPoint { When = oRow.FirstConfirmed, Value = oRow.Price }).ToList();
                CatalogPriceHistoryRow oLast = oRows[^1];
                if(oLast.LastConfirmed > oLast.FirstConfirmed) {
                    oPoints.Add(new ChartPoint { When = oLast.LastConfirmed, Value = oLast.Price });
                }

                oSeries.Add(new ChartSeries { Name = oPrice.SeriesName, Points = oPoints });
            }

            return oSeries;
        }

        protected void ToggleHistory(long tlCollectiblePriceOid) {
            _historyPriceOid = _historyPriceOid == tlCollectiblePriceOid ? null : tlCollectiblePriceOid;
        }
        #endregion (Display helpers)

        #region Add to collection
        protected void OpenAddForm(long tlCollectibleVariantOid) {
            _message = "";
            _addKind = "raw";
            _add = new AddToCollectionRequest {
                CollectibleVariantOid = tlCollectibleVariantOid,
                ConditionOid = _conditions.FirstOrDefault()?.Oid,
                GradingCompanyOid = _companies.FirstOrDefault()?.Oid,
                Quantity = 1
            };
            OnCompanyChanged();
        }

        protected void CloseAddForm() {
            _add = new AddToCollectionRequest();
        }

        protected List<LookupRow> GradesOfCompany() {
            return _grades.Where(oGrade => oGrade.ParentOid == _add.GradingCompanyOid).ToList();
        }

        /// <summary>A grade belongs to one company, so changing the company resets the grade to that company's best.</summary>
        protected void OnCompanyChanged() {
            _add.GradeOid = GradesOfCompany().FirstOrDefault()?.Oid;
        }

        protected async Task SaveAddAsync() {
            if(_isSaving) return;
            _isSaving = true;
            _message = "";

            try {
                _add.IsGraded = _addKind == "graded";
                int iAdded = await CollectionQueries.AddAsync(_collectorOid, GetAppState().UserOid, _add);
                await LoadVariantsAsync();

                _messageIsError = false;
                _message = iAdded == 1 ? "Added to your collection." : $"Added {iAdded} copies to your collection.";
                CloseAddForm();
            } catch(Exception oEx) {
                _messageIsError = true;
                _message = oEx.Message;
            } finally {
                _isSaving = false;
            }
        }
        #endregion (Add to collection)
    }
}
