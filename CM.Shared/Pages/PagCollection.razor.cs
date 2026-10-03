using System.Globalization;

namespace CM.Shared {

    /// <summary>One set's share of a collection.</summary>
    public class SetValueRow {
        public long CardSetOid { get; set; }
        public string SetName { get; set; } = "";
        public int Items { get; set; }
        public decimal? Value { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Gain { get; set; }
        public decimal? Share { get; set; }
    }

    /// <summary>
    /// What the collector owns: totals, value over time, every line with what it is worth and
    /// how that was worked out, and how complete each set is.
    /// </summary>
    public class PagCollection_Base : BaseAppStatePage {

        public const int HistoryDays = 90;

        protected override string ActiveMenuKey => "Collection";

        #region Fields
        protected List<CollectionItemRow> _items = new();
        protected List<CollectionLine> _lines = new();
        protected List<SetCompletionRow> _completion = new();
        protected List<ChartSeries> _valueSeries = new();

        protected decimal? _totalValue;
        protected decimal? _totalCost;
        protected decimal? _totalGain;
        protected decimal? _gainFraction;
        protected int _pricedCount;
        protected int _estimatedCount;
        protected int _costCount;

        protected string _filter = "";
        protected string _sort = "value";
        protected string _message = "";
        protected bool _isBusy;

        // The line whose individual copies are shown for editing ("" = none), and one edit model per copy.
        protected string _openLineKey = "";
        protected List<CollectionItemEdit> _edits = new();
        protected List<LookupRow> _conditions = new();
        #endregion (Fields)

        private long _collectorOid => GetAppState().CollectorOid ?? 0;

        protected override async Task LoadPageDataAsync() {
            _items = await CollectionQueries.GetItemsAsync(_collectorOid);
            _lines = CollectionQueries.ToLines(_items);
            if(_conditions.Count == 0) _conditions = await CatalogQueries.GetConditionsAsync();
            _completion = await CollectionQueries.GetSetCompletionAsync(_collectorOid);

            List<CollectionItemRow> oPriced = _items.Where(oItem => oItem.Value != null).ToList();
            List<CollectionItemRow> oWithCost = _items.Where(oItem => oItem.PurchasePrice != null).ToList();
            List<CollectionItemRow> oWithBoth = _items.Where(oItem => oItem.Gain != null).ToList();

            _pricedCount = oPriced.Count;
            _estimatedCount = oPriced.Count(oItem => oItem.IsEstimate);
            _costCount = oWithCost.Count;
            _totalValue = oPriced.Count == 0 ? null : oPriced.Sum(oItem => oItem.Value ?? 0m);
            _totalCost = oWithCost.Count == 0 ? null : oWithCost.Sum(oItem => oItem.PurchasePrice ?? 0m);

            // Gain compares like with like: only items that have both a value and a price paid.
            _totalGain = oWithBoth.Count == 0 ? null : oWithBoth.Sum(oItem => oItem.Gain ?? 0m);
            decimal dCostOfBoth = oWithBoth.Sum(oItem => oItem.PurchasePrice ?? 0m);
            _gainFraction = _totalGain == null || dCostOfBoth == 0 ? null : _totalGain / dCostOfBoth;

            List<CollectionValuePointRow> oPoints = _items.Count == 0
                ? new List<CollectionValuePointRow>()
                : await CollectionQueries.GetValueHistoryAsync(_collectorOid, HistoryDays);

            // Price history starts on different days for different cards. A day that prices only
            // some of the items would draw a jump that is really just a card "arriving", so the
            // chart only uses days on which every item that has history was priced.
            int iFullCoverage = oPoints.Count == 0 ? 0 : oPoints.Max(oPoint => oPoint.ItemsPriced);

            _valueSeries = new List<ChartSeries> {
                new ChartSeries {
                    Name = "Collection value",
                    Points = oPoints.Where(oPoint => oPoint.Value != null && oPoint.ItemsPriced > 0 && oPoint.ItemsPriced == iFullCoverage)
                                    .Select(oPoint => new ChartPoint { When = oPoint.Day, Value = oPoint.Value ?? 0m })
                                    .ToList()
                }
            };
        }

        protected List<CollectionLine> VisibleLines() {
            string sFilter = _filter.Trim();
            IEnumerable<CollectionLine> oLines = _lines.Where(oLine => sFilter.Length == 0
                || (oLine.First.Name ?? "").Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                || (oLine.First.SetName ?? "").Contains(sFilter, StringComparison.OrdinalIgnoreCase));

            return (_sort switch {
                "gain" => oLines.OrderByDescending(oLine => oLine.Gain ?? decimal.MinValue),
                "name" => oLines.OrderBy(oLine => oLine.First.Name),
                "set" => oLines.OrderBy(oLine => oLine.First.SetName).ThenBy(oLine => oLine.First.CardNumber),
                "newest" => oLines.OrderByDescending(oLine => oLine.Items.Max(oItem => oItem.CreatedOn)),
                _ => oLines.OrderByDescending(oLine => oLine.TotalValue ?? -1m)
            }).ToList();
        }

        /// <summary>The collection split by set, largest value first. Gain counts only items with both a value and a price paid.</summary>
        protected List<SetValueRow> ValueBySet() {
            return _items
                .GroupBy(oItem => (oItem.CardSetOid, oItem.SetName))
                .Select(oGroup => {
                    List<CollectionItemRow> oPriced = oGroup.Where(oItem => oItem.Value != null).ToList();
                    List<CollectionItemRow> oWithCost = oGroup.Where(oItem => oItem.PurchasePrice != null).ToList();
                    List<CollectionItemRow> oWithBoth = oGroup.Where(oItem => oItem.Gain != null).ToList();
                    decimal? dValue = oPriced.Count == 0 ? null : oPriced.Sum(oItem => oItem.Value ?? 0m);
                    return new SetValueRow {
                        CardSetOid = oGroup.Key.CardSetOid,
                        SetName = oGroup.Key.SetName ?? "",
                        Items = oGroup.Count(),
                        Value = dValue,
                        Cost = oWithCost.Count == 0 ? null : oWithCost.Sum(oItem => oItem.PurchasePrice ?? 0m),
                        Gain = oWithBoth.Count == 0 ? null : oWithBoth.Sum(oItem => oItem.Gain ?? 0m),
                        Share = dValue == null || _totalValue == null || _totalValue == 0 ? null : dValue / _totalValue
                    };
                })
                .OrderByDescending(oRow => oRow.Value ?? -1m)
                .ToList();
        }

        #region Editing the copies of a line
        protected static string LineKey(CollectionLine toLine) => $"{toLine.First.CollectibleVariantOid}|{toLine.First.GradeOrCondition}";

        /// <summary>Opens (or closes) the per-copy rows of a line. Each copy gets its own edit model, so one can be changed without the others.</summary>
        protected void ToggleCopies(CollectionLine toLine) {
            string sKey = LineKey(toLine);
            if(_openLineKey == sKey) {
                _openLineKey = "";
                _edits = new();
                return;
            }

            _openLineKey = sKey;
            _edits = toLine.Items
                .OrderBy(oItem => oItem.CreatedOn).ThenBy(oItem => oItem.CollectionItemOid)
                .Select(oItem => new CollectionItemEdit {
                    CollectionItemOid = oItem.CollectionItemOid,
                    IsGraded = oItem.IsGraded,
                    ConditionOid = oItem.ConditionOid,
                    PurchasePrice = oItem.PurchasePrice,
                    AcquiredOn = oItem.AcquiredOn,
                    Notes = oItem.Notes ?? ""
                })
                .ToList();
        }

        protected async Task SaveCopyAsync(CollectionItemEdit toEdit) {
            await ChangeAsync(async () => {
                bool bSaved = await CollectionQueries.UpdateAsync(_collectorOid, GetAppState().UserOid, toEdit);
                return bSaved ? "Saved." : "That item is no longer in the collection.";
            });
        }

        protected async Task RemoveCopyAsync(CollectionItemEdit toEdit) {
            await ChangeAsync(async () => {
                bool bRemoved = await CollectionQueries.RemoveAsync(_collectorOid, toEdit.CollectionItemOid);
                return bRemoved ? "Removed one copy." : "That item was already gone.";
            });
        }

        /// <summary>
        /// Runs one change, then reloads everything so every total is recalculated. A copy whose
        /// condition changed moves to another line, so the open line is closed rather than guessed at.
        /// </summary>
        private async Task ChangeAsync(Func<Task<string>> toChange) {
            if(_isBusy) return;
            _isBusy = true;
            _message = "";

            try {
                string sMessage = await toChange();
                _openLineKey = "";
                _edits = new();
                await LoadPageDataAsync();
                _message = sMessage;
            } catch(Exception oEx) {
                _loadError = oEx.Message;
            } finally {
                _isBusy = false;
            }
        }
        #endregion (Editing the copies of a line)

        /// <summary>Removes the most recently added copy on the line, then reloads so every total is recalculated.</summary>
        protected async Task RemoveOneAsync(CollectionLine toLine) {
            if(_isBusy) return;
            _isBusy = true;
            _message = "";

            try {
                CollectionItemRow oItem = toLine.Items.OrderByDescending(oRow => oRow.CreatedOn).ThenByDescending(oRow => oRow.CollectionItemOid).First();
                bool bRemoved = await CollectionQueries.RemoveAsync(_collectorOid, oItem.CollectionItemOid);
                await LoadPageDataAsync();
                _message = bRemoved ? $"Removed one copy of {oItem.Name}." : "That item was already gone.";
            } catch(Exception oEx) {
                _loadError = oEx.Message;
            } finally {
                _isBusy = false;
            }
        }

        protected static string BarWidth(SetCompletionRow toSet) {
            return (toSet.Fraction * 100m).ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}
