using System.Globalization;

namespace CM.Shared {

    /// <summary>The catalog's front page: every set that has been imported, filterable and grouped by series.</summary>
    public class PagSets_Base : BaseAppStatePage {

        protected override string ActiveMenuKey => "Catalog";

        protected List<CatalogSetRow> _sets = new();
        protected List<string> _languages = new();
        protected string _filter = "";
        protected string _language = "";
        protected string _sort = "series";

        protected override async Task LoadPageDataAsync() {
            _sets = await CatalogQueries.GetSetsAsync(GetAppState().CollectorOid ?? 0);

            // The language a set was released in. English first, since most sets are English.
            _languages = _sets.Select(oSet => oSet.LanguageName ?? "")
                              .Where(sLanguage => sLanguage.Length > 0)
                              .Distinct()
                              .OrderBy(sLanguage => sLanguage == "English" ? 0 : 1)
                              .ThenBy(sLanguage => sLanguage)
                              .ToList();
            if(_languages.Count > 1 && _languages.Contains("English")) _language = "English";
        }

        /// <summary>The filtered sets in display order. Only the "series" sort has headings; the others are one group with no heading.</summary>
        protected IEnumerable<IGrouping<string, CatalogSetRow>> Groups() {
            string sFilter = _filter.Trim();
            IEnumerable<CatalogSetRow> oSets = _sets
                .Where(oSet => _language.Length == 0 || oSet.LanguageName == _language)
                .Where(oSet => sFilter.Length == 0
                    || (oSet.DisplayName ?? "").Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                    || (oSet.Code ?? "").Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                    || (oSet.Series ?? "").Contains(sFilter, StringComparison.OrdinalIgnoreCase));

            return _sort switch {
                "newest" => oSets.OrderByDescending(oSet => oSet.ReleasedOn).GroupBy(oSet => ""),
                "value" => oSets.OrderByDescending(oSet => oSet.SetValue ?? -1m).GroupBy(oSet => ""),
                "name" => oSets.OrderBy(oSet => oSet.DisplayName).GroupBy(oSet => ""),
                // A series is as new as its newest set; inside a series, newest set first. "Other
                // products" (promo groups, miscellaneous sealed products) is not a real series and goes last.
                _ => oSets.GroupBy(oSet => oSet.Series ?? "Other")
                          .OrderBy(oGroup => oGroup.Key == "Other products" ? 1 : 0)
                          .ThenByDescending(oGroup => oGroup.Max(oSet => oSet.ReleasedOn))
                          .SelectMany(oGroup => oGroup.OrderByDescending(oSet => oSet.ReleasedOn))
                          .GroupBy(oSet => oSet.Series ?? "Other")
            };
        }

        protected static string Percent(CatalogSetRow toSet) {
            return (100m * toSet.OwnedCards / Math.Max(1, toSet.CardCount)).ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}
