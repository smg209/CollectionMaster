namespace CM.Shared {

    /// <summary>How the pages print money, percentages and dates - one place so every page agrees.</summary>
    public static class CMFormat {

        /// <summary>"$12.50", the euro sign for EUR, anything else as "12.50 XXX". Empty for null.</summary>
        public static string Money(decimal? tdAmount, string? tsCurrencyCode = "USD") {
            if(tdAmount == null) return "";
            string sAmount = Math.Abs(tdAmount.Value).ToString("N2");
            string sSign = tdAmount.Value < 0 ? "-" : "";
            return tsCurrencyCode switch {
                "USD" => sSign + "$" + sAmount,
                "EUR" => sSign + (char)0x20AC + sAmount,
                _ => $"{sSign}{sAmount} {tsCurrencyCode}"
            };
        }

        /// <summary>"+12.3%" / "-4.0%". Empty for null.</summary>
        public static string Percent(decimal? tdFraction) {
            if(tdFraction == null) return "";
            return (tdFraction.Value >= 0 ? "+" : "") + (tdFraction.Value * 100m).ToString("N1") + "%";
        }

        public static string Date(DateTime? toDate) => toDate?.ToString("MMM d, yyyy") ?? "";

        public static string DateTime(DateTime? toDate) => toDate?.ToString("MMM d, yyyy h:mm tt") ?? "";

        /// <summary>CSS class for a gain or a loss: "up", "down" or "".</summary>
        public static string Direction(decimal? tdAmount) => tdAmount == null || tdAmount == 0 ? "" : tdAmount > 0 ? "up" : "down";
    }
}
