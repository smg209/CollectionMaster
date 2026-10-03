namespace CM.Shared {

    /// <summary>
    /// Reads and writes for what a Collector owns. Everything here is tenant data: every query
    /// filters by CollectorOid and every write stamps it.
    ///
    /// CollectionItem is one row per physical copy (the schema has no quantity column), so
    /// adding three copies inserts three rows and the pages roll identical rows into one line.
    /// </summary>
    public static class CollectionQueries {

        #region Reads
        /// <summary>
        /// Every item the collector owns, newest first, each with the three prices its value is
        /// worked out from (see CollectionItemRow.Value). Slabs reach their card through the Cert.
        /// </summary>
        public static async Task<List<CollectionItemRow>> GetItemsAsync(long tlCollectorOid) {
            return await CollectionItem.FetchAsync<CollectionItemRow>(@"
SELECT ci.Oid AS CollectionItemOid, v.Oid AS CollectibleVariantOid, c.Oid AS CollectibleOid,
       ISNULL((SELECT TOP 1 n.Name FROM CollectibleName n JOIN [Language] nl ON nl.Oid = n.LanguageOid WHERE n.CollectibleOid = c.Oid AND nl.Code = 'en'), c.Name) AS Name, c.CardNumber, c.Category,
       s.Oid AS CardSetOid, ISNULL((SELECT TOP 1 n.Name FROM CardSetName n JOIN [Language] nl ON nl.Oid = n.LanguageOid WHERE n.CardSetOid = s.Oid AND nl.Code = 'en'), s.Name) AS SetName, s.Code AS SetCode,
       pr.Name AS PrintingName, l.Name AS LanguageName, v.Subtype, v.Stamp,
       (SELECT TOP 1 i.ThumbnailUrl FROM CollectibleImage i WHERE i.CollectibleOid = c.Oid
        ORDER BY CASE WHEN i.LanguageOid = v.LanguageOid THEN 0 ELSE 1 END, i.Oid) AS ThumbnailUrl,
       ci.ConditionOid, cn.Name AS ConditionName, cn.Abbreviation AS ConditionAbbreviation, cn.ValueFactor,
       ci.CertOid, gc.Abbreviation AS GradingCompany, g.Code AS GradeCode, ce.CertNumber,
       ci.PurchasePrice, ci.AcquiredOn, ci.CreatedOn, ci.Notes,
       cp.Price AS ConditionPrice, mp.Price AS MarketPrice, mp.IsListing AS MarketPriceIsListing, gp.Price AS GradedPrice
FROM CollectionItem ci
LEFT JOIN Cert ce ON ce.Oid = ci.CertOid
JOIN CollectibleVariant v ON v.Oid = ISNULL(ci.CollectibleVariantOid, ce.CollectibleVariantOid)
JOIN Collectible c ON c.Oid = v.CollectibleOid
JOIN CardSet s ON s.Oid = c.CardSetOid
JOIN Printing pr ON pr.Oid = v.PrintingOid
JOIN [Language] l ON l.Oid = v.LanguageOid
LEFT JOIN Condition cn ON cn.Oid = ci.ConditionOid
LEFT JOIN Grade g ON g.Oid = ce.GradeOid
LEFT JOIN GradingCompany gc ON gc.Oid = ce.GradingCompanyOid
OUTER APPLY (SELECT TOP 1 p.Price FROM CollectiblePrice p
             WHERE p.CollectibleVariantOid = v.Oid AND p.IsGraded = 0 AND p.CurrencyCode = 'USD' AND ci.CertOid IS NULL
               AND p.ConditionOid = ISNULL(ci.ConditionOid, (SELECT Oid FROM Condition WHERE Abbreviation = 'NM'))
             ORDER BY p.LastConfirmed DESC) cp
OUTER APPLY (SELECT TOP 1 p.Price, CAST(CASE WHEN d.Name = 'TCGplayer listings' THEN 1 ELSE 0 END AS BIT) AS IsListing
             FROM CollectiblePrice p JOIN DataSource d ON d.Oid = p.DataSourceOid
             WHERE p.CollectibleVariantOid = v.Oid AND p.IsGraded = 0 AND p.CurrencyCode = 'USD' AND p.ConditionOid IS NULL
             ORDER BY CASE WHEN d.Name = 'TCGplayer listings' THEN 1 ELSE 0 END, p.LastConfirmed DESC) mp
OUTER APPLY (SELECT TOP 1 p.Price FROM CollectiblePrice p
             WHERE p.CollectibleVariantOid = v.Oid AND p.IsGraded = 1 AND p.CurrencyCode = 'USD' AND p.GradeOid = ce.GradeOid
             ORDER BY p.LastConfirmed DESC) gp
WHERE ci.CollectorOid = @0
ORDER BY ci.CreatedOn DESC, ci.Oid DESC", tlCollectorOid);
        }

        /// <summary>Rolls identical items (same version, same condition or grade) into one line, most valuable line first.</summary>
        public static List<CollectionLine> ToLines(List<CollectionItemRow> toItems) {
            return toItems
                .GroupBy(oItem => (oItem.CollectibleVariantOid, oItem.GradeOrCondition))
                .Select(oGroup => new CollectionLine { First = oGroup.First(), Items = oGroup.ToList() })
                .OrderByDescending(oLine => oLine.TotalValue ?? -1m)
                .ThenBy(oLine => oLine.First.Name)
                .ToList();
        }

        /// <summary>
        /// The value of TODAY'S holdings on each of the last N days, from stored price history:
        /// each item is valued at the price its source showed on that day. Days before a price
        /// was first recorded leave that item out, so ItemsPriced says how much of the collection
        /// each point really covers. Uses the same price choice as CollectionItemRow.Value, except
        /// that a slab with no price for its grade is left out rather than estimated.
        /// </summary>
        public static async Task<List<CollectionValuePointRow>> GetValueHistoryAsync(long tlCollectorOid, int tiDays) {
            return await CollectionItem.FetchAsync<CollectionValuePointRow>(@"
SELECT d.[Day], SUM(h.Price * x.Factor) AS Value, COUNT(h.Price) AS ItemsPriced
FROM (SELECT CAST(DATEADD(DAY, 1 - n.N, CAST(GETDATE() AS DATE)) AS DATETIME) AS [Day]
      FROM (SELECT TOP (@1) ROW_NUMBER() OVER (ORDER BY o.object_id) AS N FROM sys.all_objects o ORDER BY o.object_id) n) d
CROSS JOIN (
    SELECT ci.Oid,
           COALESCE(gp.Oid, cp.Oid, mp.Oid) AS PriceOid,
           CASE WHEN gp.Oid IS NULL AND cp.Oid IS NULL THEN ISNULL(cn.ValueFactor, 1) ELSE 1 END AS Factor
    FROM CollectionItem ci
    LEFT JOIN Cert ce ON ce.Oid = ci.CertOid
    JOIN CollectibleVariant v ON v.Oid = ISNULL(ci.CollectibleVariantOid, ce.CollectibleVariantOid)
    LEFT JOIN Condition cn ON cn.Oid = ci.ConditionOid
    OUTER APPLY (SELECT TOP 1 p.Oid FROM CollectiblePrice p
                 WHERE p.CollectibleVariantOid = v.Oid AND p.IsGraded = 0 AND p.CurrencyCode = 'USD' AND ci.CertOid IS NULL
                   AND p.ConditionOid = ISNULL(ci.ConditionOid, (SELECT Oid FROM Condition WHERE Abbreviation = 'NM'))
                 ORDER BY p.LastConfirmed DESC) cp
    OUTER APPLY (SELECT TOP 1 p.Oid FROM CollectiblePrice p JOIN DataSource d ON d.Oid = p.DataSourceOid
                 WHERE p.CollectibleVariantOid = v.Oid AND p.IsGraded = 0 AND p.CurrencyCode = 'USD' AND p.ConditionOid IS NULL AND ci.CertOid IS NULL
                 ORDER BY CASE WHEN d.Name = 'TCGplayer listings' THEN 1 ELSE 0 END, p.LastConfirmed DESC) mp
    OUTER APPLY (SELECT TOP 1 p.Oid FROM CollectiblePrice p
                 WHERE p.CollectibleVariantOid = v.Oid AND p.IsGraded = 1 AND p.CurrencyCode = 'USD' AND p.GradeOid = ce.GradeOid
                 ORDER BY p.LastConfirmed DESC) gp
    WHERE ci.CollectorOid = @0) x
OUTER APPLY (SELECT TOP 1 ph.Price FROM CollectiblePriceHistory ph
             WHERE ph.CollectiblePriceOid = x.PriceOid AND ph.FirstConfirmed < DATEADD(DAY, 1, d.[Day])
             ORDER BY ph.FirstConfirmed DESC) h
GROUP BY d.[Day]
ORDER BY d.[Day]", tlCollectorOid, tiDays);
        }

        /// <summary>For every set the collector owns a card of: how many of its cards they have.</summary>
        public static async Task<List<SetCompletionRow>> GetSetCompletionAsync(long tlCollectorOid) {
            return await CollectionItem.FetchAsync<SetCompletionRow>(@"
SELECT s.Oid AS CardSetOid, ISNULL((SELECT TOP 1 n.Name FROM CardSetName n JOIN [Language] nl ON nl.Oid = n.LanguageOid WHERE n.CardSetOid = s.Oid AND nl.Code = 'en'), s.Name) AS SetName, s.Code AS SetCode, o.OwnedCards,
       (SELECT COUNT(*) FROM Collectible c WHERE c.CardSetOid = s.Oid AND ISNULL(c.Category, '') <> 'Sealed') AS TotalCards
FROM CardSet s
JOIN (SELECT c.CardSetOid, COUNT(DISTINCT c.Oid) AS OwnedCards
      FROM CollectionItem ci
      LEFT JOIN Cert ce ON ce.Oid = ci.CertOid
      JOIN CollectibleVariant v ON v.Oid = ISNULL(ci.CollectibleVariantOid, ce.CollectibleVariantOid)
      JOIN Collectible c ON c.Oid = v.CollectibleOid
      WHERE ci.CollectorOid = @0 AND ISNULL(c.Category, '') <> 'Sealed'
      GROUP BY c.CardSetOid) o ON o.CardSetOid = s.Oid
ORDER BY o.OwnedCards DESC, s.Name", tlCollectorOid);
        }
        #endregion (Reads)

        #region Writes
        /// <summary>
        /// Adds Quantity copies of one version to the collection.
        /// A raw copy is a CollectionItem pointing at the version, with its condition.
        /// A slab is a Cert (company + cert number + grade) and a CollectionItem pointing at it;
        /// a cert number is one physical slab, so a graded add is always a single item. With no
        /// cert number typed, a placeholder is stored so the slab can still be tracked.
        /// Returns how many items were added.
        /// </summary>
        public static async Task<int> AddAsync(long tlCollectorOid, long? tlUserOid, AddToCollectionRequest toRequest) {
            if(tlCollectorOid <= 0) throw new InvalidOperationException("There is no collector to add this to.");
            if(toRequest.CollectibleVariantOid <= 0) throw new InvalidOperationException("Choose a version of the card first.");
            if(toRequest.PurchasePrice < 0) throw new InvalidOperationException("The price paid cannot be negative.");

            string sNotes = (toRequest.Notes ?? "").Trim();

            if(toRequest.IsGraded) {
                if(toRequest.GradingCompanyOid == null || toRequest.GradeOid == null) {
                    throw new InvalidOperationException("Choose the grading company and the grade.");
                }

                string sCertNumber = (toRequest.CertNumber ?? "").Trim();
                if(sCertNumber.Length == 0) sCertNumber = "NOCERT-" + Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
                if(sCertNumber.Length > 30) throw new InvalidOperationException("A cert number is at most 30 characters.");

                Cert? oCert = await Cert.FirstAsync("WHERE GradingCompanyOid = @0 AND CertNumber = @1", toRequest.GradingCompanyOid.Value, sCertNumber);
                if(oCert == null) {
                    oCert = new Cert();
                    oCert.GradingCompanyOid = toRequest.GradingCompanyOid.Value;
                    oCert.CertNumber = sCertNumber;
                    oCert.CollectibleVariantOid = toRequest.CollectibleVariantOid;
                    oCert.GradeOid = toRequest.GradeOid;
                    oCert.CreatedOn = DateTime.Now;
                    await oCert.SaveAsync();
                } else {
                    CollectionItem? oExisting = await CollectionItem.FirstAsync("WHERE CollectorOid = @0 AND CertOid = @1", tlCollectorOid, oCert.Oid);
                    if(oExisting != null) throw new InvalidOperationException($"Cert {sCertNumber} is already in this collection.");
                }

                await SaveItemAsync(tlCollectorOid, tlUserOid, toRequest, oCert.Oid, null, sNotes);
                return 1;
            }

            int iQuantity = Math.Clamp(toRequest.Quantity, 1, 500);
            for(int i = 0; i < iQuantity; i++) {
                await SaveItemAsync(tlCollectorOid, tlUserOid, toRequest, null, toRequest.ConditionOid, sNotes);
            }
            return iQuantity;
        }

        private static async Task SaveItemAsync(long tlCollectorOid, long? tlUserOid, AddToCollectionRequest toRequest, long? tlCertOid, long? tlConditionOid, string tsNotes) {
            var oItem = new CollectionItem();
            oItem.CollectorOid = tlCollectorOid;
            oItem.CollectibleVariantOid = toRequest.CollectibleVariantOid;
            oItem.CertOid = tlCertOid;
            oItem.ConditionOid = tlConditionOid;
            oItem.AcquiredOn = toRequest.AcquiredOn;
            oItem.PurchasePrice = toRequest.PurchasePrice;
            oItem.Notes = tsNotes;
            oItem.CreatedOn = DateTime.Now;
            oItem.UserAuthOid_CreatedBy = tlUserOid > 0 ? tlUserOid : null;
            await oItem.SaveAsync();
        }

        /// <summary>
        /// Saves the editable facts of one owned copy: condition (raw copies only - a slab's
        /// grade belongs to its cert), price paid, date acquired and notes. The CollectorOid is
        /// part of the lookup, so nobody can edit another collection's item. False when not found.
        /// </summary>
        public static async Task<bool> UpdateAsync(long tlCollectorOid, long? tlUserOid, CollectionItemEdit toEdit) {
            if(toEdit.PurchasePrice < 0) throw new InvalidOperationException("The price paid cannot be negative.");

            CollectionItem? oItem = await CollectionItem.FirstAsync("WHERE Oid = @0 AND CollectorOid = @1", toEdit.CollectionItemOid, tlCollectorOid);
            if(oItem == null) return false;

            if(oItem.CertOid == null) oItem.ConditionOid = toEdit.ConditionOid;
            oItem.PurchasePrice = toEdit.PurchasePrice;
            oItem.AcquiredOn = toEdit.AcquiredOn;
            oItem.Notes = (toEdit.Notes ?? "").Trim();
            oItem.ModifiedOn = DateTime.Now;
            oItem.UserAuthOid_ModifiedBy = tlUserOid > 0 ? tlUserOid : null;
            await oItem.SaveAsync();
            return true;
        }

        /// <summary>
        /// Removes one item. The CollectorOid is part of the lookup, so an Oid belonging to
        /// someone else's collection finds nothing. Returns false when it was not found.
        /// A slab's Cert row is kept: it is a fact about the slab, not about who owns it.
        /// </summary>
        public static async Task<bool> RemoveAsync(long tlCollectorOid, long tlCollectionItemOid) {
            CollectionItem? oItem = await CollectionItem.FirstAsync("WHERE Oid = @0 AND CollectorOid = @1", tlCollectionItemOid, tlCollectorOid);
            if(oItem == null) return false;
            await oItem.DeleteAsync();
            return true;
        }
        #endregion (Writes)
    }
}
