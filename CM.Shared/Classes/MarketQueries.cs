namespace CM.Shared {

    /// <summary>One card on the market page: what it costs now and, for movers, what it cost before.</summary>
    public class MarketCardRow {
        public long CollectibleOid { get; set; }
        public string? Name { get; set; }
        public string? CardNumber { get; set; }
        public string? Rarity { get; set; }
        public string? SetName { get; set; }
        public string? SetCode { get; set; }
        public string? PrintingName { get; set; }
        public string? Subtype { get; set; }
        public string? Stamp { get; set; }
        public string? SourceName { get; set; }
        public string? ConditionName { get; set; }
        public string? ThumbnailUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public DateTime? OldPriceOn { get; set; }

        public decimal? Change => OldPrice == null ? null : Price - OldPrice;
        public decimal? ChangeFraction => OldPrice == null || OldPrice == 0 ? null : (Price - OldPrice) / OldPrice;

        public string VersionName {
            get {
                string sName = PrintingName ?? "";
                if(!string.IsNullOrEmpty(Subtype)) sName += $" - {Subtype}";
                if(!string.IsNullOrEmpty(Stamp)) sName += $" - {Stamp}";
                return sName;
            }
        }
    }

    /// <summary>How far back the stored price history reaches - decides which mover windows mean anything yet.</summary>
    public class MarketHistoryDepthRow {
        public DateTime? Oldest { get; set; }
        public DateTime? Newest { get; set; }
        public int PricesWithHistory { get; set; }
    }

    /// <summary>
    /// Market-wide reads. Movers compare a price with what the SAME source showed for the SAME
    /// version N days ago, from CollectiblePriceHistory - so they only exist once history has
    /// been collected for that long.
    /// </summary>
    public static class MarketQueries {

        private const string CardColumns = @"
       c.Oid AS CollectibleOid, ISNULL((SELECT TOP 1 n.Name FROM CollectibleName n JOIN [Language] nl ON nl.Oid = n.LanguageOid WHERE n.CollectibleOid = c.Oid AND nl.Code = 'en'), c.Name) AS Name, c.CardNumber, c.Rarity,
       ISNULL((SELECT TOP 1 n.Name FROM CardSetName n JOIN [Language] nl ON nl.Oid = n.LanguageOid WHERE n.CardSetOid = s.Oid AND nl.Code = 'en'), s.Name) AS SetName, s.Code AS SetCode,
       pr.Name AS PrintingName, v.Subtype, v.Stamp, d.Name AS SourceName, cn.Name AS ConditionName,
       (SELECT TOP 1 i.ThumbnailUrl FROM CollectibleImage i WHERE i.CollectibleOid = c.Oid
        ORDER BY CASE WHEN i.LanguageOid = v.LanguageOid THEN 0 ELSE 1 END, i.Oid) AS ThumbnailUrl";

        private const string CardJoins = @"
JOIN CollectibleVariant v ON v.Oid = p.CollectibleVariantOid
JOIN Collectible c ON c.Oid = v.CollectibleOid
JOIN CardSet s ON s.Oid = c.CardSetOid
JOIN Printing pr ON pr.Oid = v.PrintingOid
JOIN DataSource d ON d.Oid = p.DataSourceOid
LEFT JOIN Condition cn ON cn.Oid = p.ConditionOid";

        /// <summary>Raw USD prices that describe a Near Mint card: Near Mint itself, or not split by condition.</summary>
        private const string RawNearMint = @"p.IsGraded = 0 AND p.CurrencyCode = 'USD'
  AND (p.ConditionOid IS NULL OR p.ConditionOid = (SELECT Oid FROM Condition WHERE Abbreviation = 'NM'))";

        /// <summary>The most expensive versions in the catalog (TCGplayer market price), cards or sealed products.</summary>
        public static async Task<List<MarketCardRow>> GetMostValuableAsync(bool tbSealed, int tiRows = 50) {
            return await CollectiblePrice.FetchAsync<MarketCardRow>($@"
SELECT TOP {tiRows} {CardColumns}, p.Price
FROM CollectiblePrice p {CardJoins}
WHERE {RawNearMint} AND d.Name = 'TCGplayer' AND {(tbSealed ? "c.Category = 'Sealed'" : "ISNULL(c.Category, '') <> 'Sealed'")}
ORDER BY p.Price DESC");
        }

        /// <summary>
        /// The biggest risers (tbGainers) or fallers over the last tiDays days, among prices of at
        /// least tdMinimumPrice - below that, a few cents is a huge percentage and means nothing.
        /// </summary>
        public static async Task<List<MarketCardRow>> GetMoversAsync(int tiDays, bool tbGainers, decimal tdMinimumPrice = 2m, int tiRows = 25) {
            return await CollectiblePrice.FetchAsync<MarketCardRow>($@"
SELECT TOP {tiRows} {CardColumns}, p.Price, o.Price AS OldPrice, o.FirstConfirmed AS OldPriceOn
FROM CollectiblePrice p {CardJoins}
CROSS APPLY (SELECT TOP 1 h.Price, h.FirstConfirmed FROM CollectiblePriceHistory h
             WHERE h.CollectiblePriceOid = p.Oid AND h.FirstConfirmed <= DATEADD(DAY, -@0, GETDATE())
             ORDER BY h.FirstConfirmed DESC) o
WHERE {RawNearMint} AND p.Price >= @1 AND o.Price >= @1 AND o.Price <> p.Price
  AND p.LastConfirmed >= DATEADD(DAY, -3, GETDATE())
  AND {(tbGainers ? "p.Price > o.Price" : "p.Price < o.Price")}
ORDER BY (p.Price - o.Price) / o.Price {(tbGainers ? "DESC" : "ASC")}", tiDays, tdMinimumPrice);
        }

        public static async Task<MarketHistoryDepthRow> GetHistoryDepthAsync() {
            List<MarketHistoryDepthRow> oRows = await CollectiblePriceHistory.FetchAsync<MarketHistoryDepthRow>(@"
SELECT MIN(h.FirstConfirmed) AS Oldest, MAX(h.LastConfirmed) AS Newest,
       (SELECT COUNT(*) FROM (SELECT CollectiblePriceOid FROM CollectiblePriceHistory GROUP BY CollectiblePriceOid HAVING COUNT(*) > 1) m) AS PricesWithHistory
FROM CollectiblePriceHistory h");
            return oRows.FirstOrDefault() ?? new MarketHistoryDepthRow();
        }
    }
}
