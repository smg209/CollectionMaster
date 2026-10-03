namespace CM.Shared {

    /// <summary>
    /// Every read the catalog pages make. The catalog is global (not per-Collector); the only
    /// per-collector part is the "owned" counts, which take the CollectorOid as a parameter
    /// (0 when there is no collector).
    ///
    /// Each query joins several tables into one flat row and is run through the generated
    /// Record&lt;T&gt; class of its main table, so it uses the same connection handling as every
    /// other read. Lookup values are matched by name ('TCGplayer', 'NM') only where a page needs
    /// one specific source or condition for a headline figure.
    /// </summary>
    public static class CatalogQueries {

        /// <summary>
        /// A card's headline price: the highest raw USD price among its versions, counting only
        /// Near Mint prices and prices the source does not split by condition. Asking prices
        /// ('TCGplayer listings') are left out - only prices based on sales. Needs the card as alias c.
        /// </summary>
        private const string MarketPriceSql = @"
(SELECT MAX(p.Price) FROM CollectiblePrice p
 JOIN CollectibleVariant pv ON pv.Oid = p.CollectibleVariantOid
 WHERE pv.CollectibleOid = c.Oid AND p.IsGraded = 0 AND p.CurrencyCode = 'USD'
   AND p.DataSourceOid NOT IN (SELECT Oid FROM DataSource WHERE Name = 'TCGplayer listings')
   AND (p.ConditionOid IS NULL OR p.ConditionOid = (SELECT Oid FROM Condition WHERE Abbreviation = 'NM')))";

        /// <summary>Copies of card c the collector (@collector placeholder) owns - raw items by variant, slabs through their cert.</summary>
        private const string OwnedCountSql = @"
(SELECT COUNT(*) FROM CollectionItem ci
 LEFT JOIN Cert ce ON ce.Oid = ci.CertOid
 JOIN CollectibleVariant ov ON ov.Oid = ISNULL(ci.CollectibleVariantOid, ce.CollectibleVariantOid)
 WHERE ci.CollectorOid = {0} AND ov.CollectibleOid = c.Oid)";

        /// <summary>English names, where a set or card of another language has one. Need the aliases s and c.</summary>
        private const string EnglishSetNameSql = "(SELECT TOP 1 n.Name FROM CardSetName n JOIN [Language] nl ON nl.Oid = n.LanguageOid WHERE n.CardSetOid = s.Oid AND nl.Code = 'en')";
        private const string EnglishLanguageSql = "(SELECT Oid FROM [Language] WHERE Code = 'en')";

        private const string ThumbnailSql = @"
(SELECT TOP 1 i.ThumbnailUrl FROM CollectibleImage i WHERE i.CollectibleOid = c.Oid
 ORDER BY CASE WHEN i.LanguageOid = {0} THEN 0 WHEN i.LanguageOid = s.LanguageOid_Primary THEN 1 ELSE 2 END, i.Oid)";

        private const string SetSelect = @"
SELECT s.Oid AS CardSetOid, s.Name, s.Code, s.Series, g.Name AS GameName, s.LanguageOid_Primary, l.Name AS LanguageName,
       s.ReleasedOn, s.CardCountOfficial, s.CardCountTotal, s.LogoUrl, s.SymbolUrl,
       (SELECT COUNT(*) FROM Collectible c WHERE c.CardSetOid = s.Oid AND ISNULL(c.Category, '') <> 'Sealed') AS CardCount,
       (SELECT COUNT(*) FROM Collectible c WHERE c.CardSetOid = s.Oid AND c.Category = 'Sealed') AS SealedCount,
       (SELECT COUNT(*) FROM CardSetName n WHERE n.CardSetOid = s.Oid) AS LanguageCount,
       (SELECT TOP 1 n.Name FROM CardSetName n JOIN [Language] nl ON nl.Oid = n.LanguageOid WHERE n.CardSetOid = s.Oid AND nl.Code = 'en') AS EnglishName,
       (SELECT SUM(x.MinPrice) FROM (
            SELECT MIN(p.Price) AS MinPrice
            FROM Collectible c
            JOIN CollectibleVariant v ON v.CollectibleOid = c.Oid
            JOIN CollectiblePrice p ON p.CollectibleVariantOid = v.Oid
            JOIN DataSource d ON d.Oid = p.DataSourceOid
            WHERE c.CardSetOid = s.Oid AND ISNULL(c.Category, '') <> 'Sealed' AND d.Name = 'TCGplayer' AND p.IsGraded = 0
            GROUP BY c.Oid) x) AS SetValue,
       (SELECT COUNT(DISTINCT ov.CollectibleOid) FROM CollectionItem ci
        LEFT JOIN Cert ce ON ce.Oid = ci.CertOid
        JOIN CollectibleVariant ov ON ov.Oid = ISNULL(ci.CollectibleVariantOid, ce.CollectibleVariantOid)
        JOIN Collectible oc ON oc.Oid = ov.CollectibleOid
        WHERE ci.CollectorOid = @0 AND oc.CardSetOid = s.Oid AND ISNULL(oc.Category, '') <> 'Sealed') AS OwnedCards
FROM CardSet s
JOIN Game g ON g.Oid = s.GameOid
JOIN [Language] l ON l.Oid = s.LanguageOid_Primary";

        private static string CardSelect(string tsLanguageParameter, string tsCollectorParameter) => $@"
SELECT c.Oid AS CollectibleOid, c.CardNumber, c.Name, c.Rarity, c.Category, s.Oid AS CardSetOid, ISNULL({EnglishSetNameSql}, s.Name) AS SetName, s.Code AS SetCode,
       s.ReleasedOn AS SetReleasedOn,
       (SELECT TOP 1 n.Name FROM CollectibleName n WHERE n.CollectibleOid = c.Oid AND n.LanguageOid = {tsLanguageParameter}) AS LocalName,
       {string.Format(ThumbnailSql, tsLanguageParameter)} AS ThumbnailUrl,
       (SELECT COUNT(*) FROM CollectibleVariant v WHERE v.CollectibleOid = c.Oid) AS VariantCount,
       {MarketPriceSql} AS MarketPrice,
       {string.Format(OwnedCountSql, tsCollectorParameter)} AS OwnedCount
FROM Collectible c
JOIN CardSet s ON s.Oid = c.CardSetOid";

        #region Sets
        public static async Task<List<CatalogSetRow>> GetSetsAsync(long tlCollectorOid) {
            return await CardSet.FetchAsync<CatalogSetRow>(SetSelect + " ORDER BY s.ReleasedOn DESC, s.Name", tlCollectorOid);
        }

        public static async Task<CatalogSetRow?> GetSetAsync(long tlCardSetOid, long tlCollectorOid) {
            List<CatalogSetRow> oRows = await CardSet.FetchAsync<CatalogSetRow>(SetSelect + " WHERE s.Oid = @1", tlCollectorOid, tlCardSetOid);
            return oRows.FirstOrDefault();
        }

        /// <summary>
        /// The set's name in every language its cards also have names in - a source can list a
        /// set under a language without having translated the cards. The primary language comes first.
        /// </summary>
        public static async Task<List<CatalogNameRow>> GetSetNamesAsync(long tlCardSetOid) {
            return await CardSetName.FetchAsync<CatalogNameRow>(@"
SELECT n.LanguageOid, l.Code AS LanguageCode, l.Name AS LanguageName, n.Name
FROM CardSetName n
JOIN [Language] l ON l.Oid = n.LanguageOid
JOIN CardSet s ON s.Oid = n.CardSetOid
WHERE n.CardSetOid = @0
  AND EXISTS (SELECT 1 FROM CollectibleName cn JOIN Collectible c ON c.Oid = cn.CollectibleOid
              WHERE c.CardSetOid = n.CardSetOid AND cn.LanguageOid = n.LanguageOid)
ORDER BY CASE WHEN n.LanguageOid = s.LanguageOid_Primary THEN 0 ELSE 1 END, l.Oid", tlCardSetOid);
        }
        #endregion (Sets)

        #region Cards
        /// <summary>
        /// The cards and sealed products of a set, named and pictured in the requested language
        /// where the card has that language, otherwise in the set's primary language. In card-number order.
        /// </summary>
        public static async Task<List<CatalogCardRow>> GetCardsAsync(long tlCardSetOid, long tlLanguageOid, long tlCollectorOid) {
            List<CatalogCardRow> oRows = await Collectible.FetchAsync<CatalogCardRow>(
                CardSelect("@1", "@2") + " WHERE c.CardSetOid = @0", tlCardSetOid, tlLanguageOid, tlCollectorOid);

            // Card numbers are text ("4", "004", "TG12"): numeric ones in number order, the rest after.
            return oRows
                .OrderBy(oRow => oRow.IsSealed)
                .ThenBy(oRow => LeadingNumber(oRow.CardNumber) ?? int.MaxValue)
                .ThenBy(oRow => oRow.CardNumber, StringComparer.OrdinalIgnoreCase)
                .ThenBy(oRow => oRow.Name)
                .ToList();
        }

        /// <summary>
        /// Cards whose name in ANY language contains the text, or whose card number equals it.
        /// Shown under their English name where they have one (a Japanese card found by its English name).
        /// The most valuable matches first; capped, because a search for "a" matches most of the catalog.
        /// </summary>
        public static async Task<List<CatalogCardRow>> SearchCardsAsync(string tsText, long tlCollectorOid, int tiMaxRows = 120) {
            string sText = (tsText ?? "").Trim();
            if(sText.Length == 0) return new List<CatalogCardRow>();

            // LIKE wildcards typed by the user are matched literally.
            string sPattern = "%" + sText.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + "%";

            return await Collectible.FetchAsync<CatalogCardRow>($@"
SELECT TOP {tiMaxRows} x.* FROM ({CardSelect(EnglishLanguageSql, "@2")}
WHERE c.Oid IN (SELECT n.CollectibleOid FROM CollectibleName n WHERE n.Name LIKE @0) OR c.CardNumber = @1) x
ORDER BY CASE WHEN x.MarketPrice IS NULL THEN 1 ELSE 0 END, x.MarketPrice DESC, x.Name", sPattern, sText, tlCollectorOid);
        }

        /// <summary>
        /// Every other card that shares a name with this one in any language - the same Pokemon or
        /// trainer card printed in other sets, including other languages. The most valuable first.
        /// </summary>
        public static async Task<List<CatalogCardRow>> GetSameNameCardsAsync(long tlCollectibleOid, long tlCollectorOid, int tiMaxRows = 60) {
            return await Collectible.FetchAsync<CatalogCardRow>($@"
SELECT TOP {tiMaxRows} x.* FROM ({CardSelect(EnglishLanguageSql, "@1")}
WHERE c.Oid <> @0 AND ISNULL(c.Category, '') <> 'Sealed'
  AND (c.Name = (SELECT o.Name FROM Collectible o WHERE o.Oid = @0)
       OR c.Oid IN (SELECT n.CollectibleOid FROM CollectibleName n
                    WHERE n.Name IN (SELECT m.Name FROM CollectibleName m WHERE m.CollectibleOid = @0)))) x
ORDER BY CASE WHEN x.MarketPrice IS NULL THEN 1 ELSE 0 END, x.MarketPrice DESC, x.SetReleasedOn DESC", tlCollectibleOid, tlCollectorOid);
        }

        public static async Task<CatalogCardDetailRow?> GetCardAsync(long tlCollectibleOid) {
            List<CatalogCardDetailRow> oRows = await Collectible.FetchAsync<CatalogCardDetailRow>(@"
SELECT c.Oid AS CollectibleOid, c.Name, c.CardNumber, c.Rarity, c.Category, c.Illustrator,
       s.Oid AS CardSetOid, s.Name AS SetName, s.Code AS SetCode, s.CardCountOfficial, g.Name AS GameName
FROM Collectible c
JOIN CardSet s ON s.Oid = c.CardSetOid
JOIN Game g ON g.Oid = s.GameOid
WHERE c.Oid = @0", tlCollectibleOid);
            return oRows.FirstOrDefault();
        }

        public static async Task<List<CatalogNameRow>> GetCardNamesAsync(long tlCollectibleOid) {
            return await CollectibleName.FetchAsync<CatalogNameRow>(@"
SELECT n.LanguageOid, l.Code AS LanguageCode, l.Name AS LanguageName, n.Name
FROM CollectibleName n
JOIN [Language] l ON l.Oid = n.LanguageOid
WHERE n.CollectibleOid = @0
ORDER BY l.Oid", tlCollectibleOid);
        }

        public static async Task<List<CatalogImageRow>> GetCardImagesAsync(long tlCollectibleOid) {
            return await CollectibleImage.FetchAsync<CatalogImageRow>(@"
SELECT i.LanguageOid, l.Code AS LanguageCode, l.Name AS LanguageName, d.Name AS SourceName, i.ImageUrl, i.ThumbnailUrl
FROM CollectibleImage i
JOIN [Language] l ON l.Oid = i.LanguageOid
JOIN DataSource d ON d.Oid = i.DataSourceOid
WHERE i.CollectibleOid = @0
ORDER BY l.Oid", tlCollectibleOid);
        }
        #endregion (Cards)

        #region Versions and prices
        /// <summary>The card's versions: plain printings first, then the special print runs and stamps.</summary>
        public static async Task<List<CatalogVariantRow>> GetVariantsAsync(long tlCollectibleOid, long tlCollectorOid) {
            return await CollectibleVariant.FetchAsync<CatalogVariantRow>(@"
SELECT v.Oid AS CollectibleVariantOid, pr.Name AS PrintingName, pr.SortOrder AS PrintingSort, l.Name AS LanguageName, v.Subtype, v.Stamp,
       (SELECT COUNT(*) FROM CollectionItem ci LEFT JOIN Cert ce ON ce.Oid = ci.CertOid
        WHERE ci.CollectorOid = @1 AND ISNULL(ci.CollectibleVariantOid, ce.CollectibleVariantOid) = v.Oid) AS OwnedCount
FROM CollectibleVariant v
JOIN Printing pr ON pr.Oid = v.PrintingOid
JOIN [Language] l ON l.Oid = v.LanguageOid
WHERE v.CollectibleOid = @0
ORDER BY CASE WHEN v.Stamp IS NULL THEN 0 ELSE 1 END,
         CASE WHEN ISNULL(v.Subtype, 'unlimited') = 'unlimited' THEN 0 ELSE 1 END,
         pr.SortOrder, l.Oid, v.Subtype, v.Stamp", tlCollectibleOid, tlCollectorOid);
        }

        public static async Task<List<CatalogVariantExternalIdRow>> GetVariantExternalIdsAsync(long tlCollectibleOid) {
            return await CollectibleVariantExternalId.FetchAsync<CatalogVariantExternalIdRow>(@"
SELECT e.CollectibleVariantOid, d.Name AS SourceName, e.ExternalId
FROM CollectibleVariantExternalId e
JOIN CollectibleVariant v ON v.Oid = e.CollectibleVariantOid
JOIN DataSource d ON d.Oid = e.DataSourceOid
WHERE v.CollectibleOid = @0
ORDER BY d.Name", tlCollectibleOid);
        }

        /// <summary>Every current price of every version of one card.</summary>
        public static async Task<List<CatalogPriceRow>> GetPricesAsync(long tlCollectibleOid) {
            return await CollectiblePrice.FetchAsync<CatalogPriceRow>(@"
SELECT p.Oid AS CollectiblePriceOid, p.CollectibleVariantOid, d.Name AS SourceName, via.Name AS RetrievedFromName,
       p.IsGraded, gc.Abbreviation AS GradingCompany, g.Code AS GradeCode, g.GradeValue,
       cn.Name AS ConditionName, cn.SortOrder AS ConditionSort, p.Qualifier,
       p.Price, p.LowPrice, p.HighPrice, p.CurrencyCode, p.LastConfirmed, p.SourceUrl,
       (SELECT COUNT(*) FROM CollectiblePriceHistory h WHERE h.CollectiblePriceOid = p.Oid) AS HistoryCount
FROM CollectiblePrice p
JOIN CollectibleVariant v ON v.Oid = p.CollectibleVariantOid
JOIN DataSource d ON d.Oid = p.DataSourceOid
LEFT JOIN DataSource via ON via.Oid = p.DataSourceOid_RetrievedFrom
LEFT JOIN Grade g ON g.Oid = p.GradeOid
LEFT JOIN GradingCompany gc ON gc.Oid = g.GradingCompanyOid
LEFT JOIN Condition cn ON cn.Oid = p.ConditionOid
WHERE v.CollectibleOid = @0
ORDER BY p.IsGraded, d.Name, cn.SortOrder, gc.Abbreviation, g.GradeValue DESC", tlCollectibleOid);
        }

        /// <summary>The whole price history of every price of one card, oldest first within each price.</summary>
        public static async Task<List<CatalogPriceHistoryRow>> GetCardPriceHistoryAsync(long tlCollectibleOid) {
            return await CollectiblePriceHistory.FetchAsync<CatalogPriceHistoryRow>(@"
SELECT h.CollectiblePriceOid, h.Price, h.CurrencyCode, h.FirstConfirmed, h.LastConfirmed
FROM CollectiblePriceHistory h
JOIN CollectiblePrice p ON p.Oid = h.CollectiblePriceOid
JOIN CollectibleVariant v ON v.Oid = p.CollectibleVariantOid
WHERE v.CollectibleOid = @0
ORDER BY h.CollectiblePriceOid, h.FirstConfirmed", tlCollectibleOid);
        }
        #endregion (Versions and prices)

        #region Lookups and totals
        public static async Task<List<LookupRow>> GetConditionsAsync() {
            return await Condition.FetchAsync<LookupRow>("SELECT Oid, Name, CAST(NULL AS BIGINT) AS ParentOid FROM Condition WHERE IsActive = 1 ORDER BY SortOrder");
        }

        public static async Task<List<LookupRow>> GetGradingCompaniesAsync() {
            return await GradingCompany.FetchAsync<LookupRow>("SELECT Oid, Abbreviation AS Name, CAST(NULL AS BIGINT) AS ParentOid FROM GradingCompany WHERE IsActive = 1 ORDER BY Oid");
        }

        /// <summary>Every company's grades, best first. ParentOid is the GradingCompanyOid.</summary>
        public static async Task<List<LookupRow>> GetGradesAsync() {
            return await Grade.FetchAsync<LookupRow>("SELECT Oid, Code + ' - ' + Name AS Name, GradingCompanyOid AS ParentOid FROM Grade WHERE IsActive = 1 ORDER BY GradingCompanyOid, SortOrder");
        }

        public static async Task<CatalogStatsRow> GetStatsAsync() {
            List<CatalogStatsRow> oRows = await CardSet.FetchAsync<CatalogStatsRow>(@"
SELECT (SELECT COUNT(*) FROM CardSet) AS SetCount,
       (SELECT COUNT(*) FROM Collectible WHERE ISNULL(Category, '') <> 'Sealed') AS CardCount,
       (SELECT COUNT(*) FROM Collectible WHERE Category = 'Sealed') AS SealedCount,
       (SELECT COUNT(*) FROM CollectibleVariant) AS VariantCount,
       (SELECT COUNT(*) FROM CollectiblePrice) AS PriceCount,
       (SELECT COUNT(*) FROM CollectiblePriceHistory) AS HistoryCount,
       (SELECT MAX(LastConfirmed) FROM CollectiblePrice) AS NewestPrice");
            return oRows.FirstOrDefault() ?? new CatalogStatsRow();
        }
        #endregion (Lookups and totals)

        private static int? LeadingNumber(string? tsCardNumber) {
            if(string.IsNullOrEmpty(tsCardNumber)) return null;
            int iLength = 0;
            while(iLength < tsCardNumber.Length && char.IsDigit(tsCardNumber[iLength])) iLength++;
            return iLength > 0 && int.TryParse(tsCardNumber.AsSpan(0, iLength), out int iNumber) ? iNumber : null;
        }
    }
}
