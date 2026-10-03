namespace CM.Shared {

    // Flat read models for the catalog pages. Each one is the shape of ONE query in
    // CatalogQueries - several tables joined into the row a page displays - and is filled by
    // Record<T>.FetchAsync<TProjection>. They are never saved; anything that is edited and
    // saved goes through the generated Record<T> classes instead.

    /// <summary>One set, as listed on PagSets and shown at the top of PagSetCards.</summary>
    public class CatalogSetRow {
        public long CardSetOid { get; set; }
        public string? Name { get; set; }
        public string? EnglishName { get; set; }
        public string? Code { get; set; }
        public string? Series { get; set; }
        public string? GameName { get; set; }
        public long LanguageOid_Primary { get; set; }
        public string? LanguageName { get; set; }
        public DateTime? ReleasedOn { get; set; }
        public int? CardCountOfficial { get; set; }
        public int? CardCountTotal { get; set; }
        public string? LogoUrl { get; set; }
        public string? SymbolUrl { get; set; }
        public int CardCount { get; set; }
        public int SealedCount { get; set; }
        public int LanguageCount { get; set; }

        /// <summary>What one copy of every card costs, taking the cheapest version of each (TCGplayer market, USD).</summary>
        public decimal? SetValue { get; set; }

        /// <summary>How many different cards of the set the current collector owns.</summary>
        public int OwnedCards { get; set; }

        /// <summary>The name to show an English reader: the set's own name, plus the English one when they differ.</summary>
        public string DisplayName => string.IsNullOrEmpty(EnglishName) || EnglishName == Name ? (Name ?? "") : $"{EnglishName} ({Name})";
    }

    /// <summary>A name in one language - of a set (PagSetCards) or of a card (PagCard).</summary>
    public class CatalogNameRow {
        public long LanguageOid { get; set; }
        public string? LanguageCode { get; set; }
        public string? LanguageName { get; set; }
        public string? Name { get; set; }
    }

    /// <summary>One card (or sealed product) in a grid: a set's cards, or search results.</summary>
    public class CatalogCardRow {
        public long CollectibleOid { get; set; }
        public string? CardNumber { get; set; }
        public string? Name { get; set; }
        public string? LocalName { get; set; }
        public string? Rarity { get; set; }
        public string? Category { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int VariantCount { get; set; }
        public long CardSetOid { get; set; }
        public string? SetName { get; set; }
        public string? SetCode { get; set; }
        public DateTime? SetReleasedOn { get; set; }

        /// <summary>Highest raw Near Mint (or condition-less) USD price among the card's versions.</summary>
        public decimal? MarketPrice { get; set; }

        /// <summary>Copies of this card the current collector owns, any version.</summary>
        public int OwnedCount { get; set; }

        public bool IsSealed => Category == "Sealed";

        public string DisplayName => string.IsNullOrEmpty(LocalName) ? (Name ?? "") : LocalName;
    }

    /// <summary>The card itself, at the top of PagCard.</summary>
    public class CatalogCardDetailRow {
        public long CollectibleOid { get; set; }
        public string? Name { get; set; }
        public string? CardNumber { get; set; }
        public string? Rarity { get; set; }
        public string? Category { get; set; }
        public string? Illustrator { get; set; }
        public long CardSetOid { get; set; }
        public string? SetName { get; set; }
        public string? SetCode { get; set; }
        public int? CardCountOfficial { get; set; }
        public string? GameName { get; set; }

        public bool IsSealed => Category == "Sealed";
    }

    public class CatalogImageRow {
        public long LanguageOid { get; set; }
        public string? LanguageCode { get; set; }
        public string? LanguageName { get; set; }
        public string? SourceName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
    }

    /// <summary>One version of a card: a printing in a language, optionally split by subtype and stamp.</summary>
    public class CatalogVariantRow {
        public long CollectibleVariantOid { get; set; }
        public string? PrintingName { get; set; }
        public int PrintingSort { get; set; }
        public string? LanguageName { get; set; }
        public string? Subtype { get; set; }
        public string? Stamp { get; set; }
        public int OwnedCount { get; set; }

        public string DisplayName {
            get {
                string sName = $"{PrintingName} - {LanguageName}";
                if(!string.IsNullOrEmpty(Subtype)) sName += $" - {Subtype}";
                if(!string.IsNullOrEmpty(Stamp)) sName += $" - {Stamp}";
                return sName;
            }
        }
    }

    /// <summary>A data source's own id for a variant (a TCGplayer or Cardmarket product id).</summary>
    public class CatalogVariantExternalIdRow {
        public long CollectibleVariantOid { get; set; }
        public string? SourceName { get; set; }
        public string? ExternalId { get; set; }
    }

    /// <summary>One current price of one variant, with every lookup already resolved to text.</summary>
    public class CatalogPriceRow {
        public long CollectiblePriceOid { get; set; }
        public long CollectibleVariantOid { get; set; }
        public string? SourceName { get; set; }
        public string? RetrievedFromName { get; set; }
        public bool IsGraded { get; set; }
        public string? GradingCompany { get; set; }
        public string? GradeCode { get; set; }
        public decimal? GradeValue { get; set; }
        public string? ConditionName { get; set; }
        public int? ConditionSort { get; set; }
        public string? Qualifier { get; set; }
        public decimal Price { get; set; }
        public decimal? LowPrice { get; set; }
        public decimal? HighPrice { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime LastConfirmed { get; set; }
        public string? SourceUrl { get; set; }
        public int HistoryCount { get; set; }

        /// <summary>"PSA 8", "PSA 8 (OC)", "Near Mint", or "Any condition" for a raw price the source does not split.</summary>
        public string GradeOrCondition {
            get {
                if(IsGraded) return $"{GradingCompany} {GradeCode}" + (string.IsNullOrEmpty(Qualifier) ? "" : $" ({Qualifier})");
                if(SourceName == "TCGplayer listings") return "Lowest asking price";
                return string.IsNullOrEmpty(ConditionName) ? "Any condition" : ConditionName;
            }
        }

        public string SourceDisplay => string.IsNullOrEmpty(RetrievedFromName) ? (SourceName ?? "") : $"{SourceName} (via {RetrievedFromName})";

        /// <summary>Legend text for a chart line: "TCGplayer - Any condition".</summary>
        public string SeriesName => $"{SourceName} - {GradeOrCondition}";
    }

    public class CatalogPriceHistoryRow {
        public long CollectiblePriceOid { get; set; }
        public decimal Price { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime FirstConfirmed { get; set; }
        public DateTime LastConfirmed { get; set; }
    }

    /// <summary>A row of a lookup list for a dropdown. ParentOid is the grading company of a grade.</summary>
    public class LookupRow {
        public long Oid { get; set; }
        public string? Name { get; set; }
        public long? ParentOid { get; set; }
    }

    /// <summary>Headline numbers about the whole catalog, for the home page.</summary>
    public class CatalogStatsRow {
        public int SetCount { get; set; }
        public int CardCount { get; set; }
        public int SealedCount { get; set; }
        public int VariantCount { get; set; }
        public int PriceCount { get; set; }
        public int HistoryCount { get; set; }
        public DateTime? NewestPrice { get; set; }
    }
}
