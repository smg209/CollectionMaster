namespace CM.Shared {

    /// <summary>
    /// One physical item a collector owns, with the card it is, what was paid, and the prices
    /// needed to value it. Filled by CollectionQueries.GetItemsAsync.
    /// </summary>
    public class CollectionItemRow {
        public long CollectionItemOid { get; set; }
        public long CollectibleVariantOid { get; set; }
        public long CollectibleOid { get; set; }
        public string? Name { get; set; }
        public string? CardNumber { get; set; }
        public string? Category { get; set; }
        public long CardSetOid { get; set; }
        public string? SetName { get; set; }
        public string? SetCode { get; set; }
        public string? PrintingName { get; set; }
        public string? LanguageName { get; set; }
        public string? Subtype { get; set; }
        public string? Stamp { get; set; }
        public string? ThumbnailUrl { get; set; }

        public long? ConditionOid { get; set; }
        public string? ConditionName { get; set; }
        public string? ConditionAbbreviation { get; set; }
        public decimal? ValueFactor { get; set; }

        public long? CertOid { get; set; }
        public string? GradingCompany { get; set; }
        public string? GradeCode { get; set; }
        public string? CertNumber { get; set; }

        public decimal? PurchasePrice { get; set; }
        public DateTime? AcquiredOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Notes { get; set; }

        /// <summary>A source's price for exactly this condition (raw) - JustTCG. Null when none.</summary>
        public decimal? ConditionPrice { get; set; }
        /// <summary>The version's TCGplayer market price, which is not split by condition. Null when none.</summary>
        public decimal? MarketPrice { get; set; }
        /// <summary>True when MarketPrice is only the lowest asking price - the version has no sales-based price.</summary>
        public bool MarketPriceIsListing { get; set; }
        /// <summary>A source's price for exactly this grade (slabs). Null when none.</summary>
        public decimal? GradedPrice { get; set; }

        public bool IsGraded => CertOid != null;

        public string VersionName {
            get {
                string sName = $"{PrintingName} - {LanguageName}";
                if(!string.IsNullOrEmpty(Subtype)) sName += $" - {Subtype}";
                if(!string.IsNullOrEmpty(Stamp)) sName += $" - {Stamp}";
                return sName;
            }
        }

        /// <summary>"PSA 9" for a slab, "Sealed" for a sealed product, otherwise the condition ("Near Mint" when none was recorded).</summary>
        public string GradeOrCondition => IsGraded ? $"{GradingCompany} {GradeCode}".Trim()
            : Category == "Sealed" ? "Sealed"
            : (ConditionName ?? "Near Mint");

        /// <summary>
        /// What this one item is worth today (USD), best evidence first:
        ///   slab: a price for that exact grade, else the raw market price (a floor, flagged as an estimate);
        ///   raw:  a price for that exact condition, else market price x the condition's ValueFactor (an estimate).
        /// Null when no source prices this version at all.
        /// </summary>
        public decimal? Value {
            get {
                if(IsGraded) return GradedPrice ?? MarketPrice;
                if(ConditionPrice != null) return ConditionPrice;
                if(MarketPrice == null) return null;
                return Math.Round(MarketPrice.Value * (ValueFactor ?? 1m), 2);
            }
        }

        /// <summary>How Value was arrived at, in words the collection page can show.</summary>
        public string ValueBasis {
            get {
                if(Value == null) return "No price yet";
                if(IsGraded) return GradedPrice != null ? "Price for this grade" : "Raw price (no price for this grade yet)";
                if(ConditionPrice != null) return "Price for this condition";
                string sBase = MarketPriceIsListing ? "Lowest asking price (no recent sales)" : "Market price";
                return (ValueFactor ?? 1m) == 1m ? sBase : $"{sBase} x {(ValueFactor ?? 1m):0.##} for condition (estimate)";
            }
        }

        public bool IsEstimate => Value != null
            && (IsGraded ? GradedPrice == null : ConditionPrice == null && (MarketPriceIsListing || (ValueFactor ?? 1m) != 1m));

        public decimal? Gain => Value != null && PurchasePrice != null ? Value - PurchasePrice : null;
    }

    /// <summary>Identical items rolled into one line of the collection table: same version, same condition or grade.</summary>
    public class CollectionLine {
        public CollectionItemRow First { get; set; } = new();
        public List<CollectionItemRow> Items { get; set; } = new();

        public int Quantity => Items.Count;
        public decimal? UnitValue => First.Value;
        public decimal? TotalValue => Items.All(oItem => oItem.Value == null) ? null : Items.Sum(oItem => oItem.Value ?? 0m);
        public decimal? TotalCost => Items.All(oItem => oItem.PurchasePrice == null) ? null : Items.Sum(oItem => oItem.PurchasePrice ?? 0m);

        /// <summary>Gain over the items that have BOTH a value and a purchase price - anything else would mix priced and unpriced copies.</summary>
        public decimal? Gain {
            get {
                List<CollectionItemRow> oBoth = Items.Where(oItem => oItem.Gain != null).ToList();
                return oBoth.Count == 0 ? null : oBoth.Sum(oItem => oItem.Gain ?? 0m);
            }
        }
    }

    /// <summary>The collection's total value on one day, from stored price history.</summary>
    public class CollectionValuePointRow {
        public DateTime Day { get; set; }
        public decimal? Value { get; set; }
        public int ItemsPriced { get; set; }
    }

    /// <summary>How complete one set is for a collector.</summary>
    public class SetCompletionRow {
        public long CardSetOid { get; set; }
        public string? SetName { get; set; }
        public string? SetCode { get; set; }
        public int OwnedCards { get; set; }
        public int TotalCards { get; set; }

        public decimal Fraction => TotalCards == 0 ? 0m : (decimal)OwnedCards / TotalCards;
    }

    /// <summary>One owned copy being edited on the collection page: the things about a copy that can change.</summary>
    public class CollectionItemEdit {
        public long CollectionItemOid { get; set; }
        public bool IsGraded { get; set; }
        public long? ConditionOid { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? AcquiredOn { get; set; }
        public string Notes { get; set; } = "";
    }

    /// <summary>What the add-to-collection form collects.</summary>
    public class AddToCollectionRequest {
        public long CollectibleVariantOid { get; set; }
        public bool IsGraded { get; set; }
        public long? ConditionOid { get; set; }
        public long? GradingCompanyOid { get; set; }
        public long? GradeOid { get; set; }
        public string CertNumber { get; set; } = "";
        public int Quantity { get; set; } = 1;
        public decimal? PurchasePrice { get; set; }
        public DateTime? AcquiredOn { get; set; }
        public string Notes { get; set; } = "";
    }
}
