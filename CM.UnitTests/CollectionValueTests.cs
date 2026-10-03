using CM.Shared;

namespace CM.UnitTests {

    /// <summary>How an owned item is valued, and how items roll up into lines. No database involved.</summary>
    public class CollectionValueTests {

        private static CollectionItemRow Raw(decimal? tdConditionPrice, decimal? tdMarketPrice, decimal? tdFactor, decimal? tdPaid = null) {
            return new CollectionItemRow {
                CollectibleVariantOid = 1, Name = "Charizard", ConditionName = "Lightly Played", ValueFactor = tdFactor,
                ConditionPrice = tdConditionPrice, MarketPrice = tdMarketPrice, PurchasePrice = tdPaid
            };
        }

        [Fact]
        public void Raw_WithAPriceForItsCondition_UsesThatPrice() {
            CollectionItemRow oItem = Raw(540.17m, 944.53m, 0.8m);

            Assert.Equal(540.17m, oItem.Value);
            Assert.False(oItem.IsEstimate);
        }

        [Fact]
        public void Raw_WithOnlyAMarketPrice_ScalesItByTheConditionFactor_AndSaysItIsAnEstimate() {
            CollectionItemRow oItem = Raw(null, 100m, 0.8m);

            Assert.Equal(80m, oItem.Value);
            Assert.True(oItem.IsEstimate);
            Assert.Contains("estimate", oItem.ValueBasis);
        }

        [Fact]
        public void Raw_NearMintOnMarketPrice_IsNotAnEstimate() {
            CollectionItemRow oItem = Raw(null, 100m, 1m);

            Assert.Equal(100m, oItem.Value);
            Assert.False(oItem.IsEstimate);
        }

        [Fact]
        public void Raw_ValuedFromAnAskingPrice_IsAlwaysAnEstimate() {
            CollectionItemRow oItem = Raw(null, 100m, 1m);
            oItem.MarketPriceIsListing = true;

            Assert.Equal(100m, oItem.Value);
            Assert.True(oItem.IsEstimate);
            Assert.Contains("asking price", oItem.ValueBasis);
        }

        [Fact]
        public void Slab_WithAPriceForItsGrade_UsesIt_OtherwiseFallsBackToTheRawPriceAsAnEstimate() {
            var oPriced = new CollectionItemRow { CertOid = 5, GradingCompany = "PSA", GradeCode = "8", GradedPrice = 1479.99m, MarketPrice = 944.53m };
            var oUnpriced = new CollectionItemRow { CertOid = 6, GradingCompany = "PSA", GradeCode = "10", MarketPrice = 944.53m };

            Assert.Equal(1479.99m, oPriced.Value);
            Assert.False(oPriced.IsEstimate);
            Assert.Equal("PSA 8", oPriced.GradeOrCondition);

            Assert.Equal(944.53m, oUnpriced.Value);
            Assert.True(oUnpriced.IsEstimate);
        }

        [Fact]
        public void NoPriceAtAll_HasNoValue_AndNoGain() {
            CollectionItemRow oItem = Raw(null, null, 0.8m, 10m);

            Assert.Null(oItem.Value);
            Assert.Null(oItem.Gain);
            Assert.Equal("No price yet", oItem.ValueBasis);
        }

        [Fact]
        public void SealedProduct_IsLabelledSealed_NotNearMint() {
            var oItem = new CollectionItemRow { Category = "Sealed", MarketPrice = 488.84m };

            Assert.Equal("Sealed", oItem.GradeOrCondition);
            Assert.Equal(488.84m, oItem.Value);
            Assert.False(oItem.IsEstimate);
        }

        [Fact]
        public void Gain_IsValueMinusPricePaid() {
            CollectionItemRow oItem = Raw(50m, null, 1m, 30m);

            Assert.Equal(20m, oItem.Gain);
        }

        [Fact]
        public void ToLines_RollsIdenticalItemsTogether_AndKeepsDifferentConditionsApart() {
            var oItems = new List<CollectionItemRow> {
                new() { CollectibleVariantOid = 1, Name = "A", ConditionName = "Near Mint", ConditionPrice = 10m, PurchasePrice = 4m },
                new() { CollectibleVariantOid = 1, Name = "A", ConditionName = "Near Mint", ConditionPrice = 10m },
                new() { CollectibleVariantOid = 1, Name = "A", ConditionName = "Damaged", ConditionPrice = 2m },
                new() { CollectibleVariantOid = 2, Name = "B", ConditionName = "Near Mint", ConditionPrice = 100m, PurchasePrice = 120m },
            };

            List<CollectionLine> oLines = CollectionQueries.ToLines(oItems);

            Assert.Equal(3, oLines.Count);
            Assert.Equal("B", oLines[0].First.Name);            // most valuable line first
            Assert.Equal(-20m, oLines[0].Gain);

            CollectionLine oNearMintA = oLines.Single(oLine => oLine.First.Name == "A" && oLine.First.ConditionName == "Near Mint");
            Assert.Equal(2, oNearMintA.Quantity);
            Assert.Equal(20m, oNearMintA.TotalValue);
            Assert.Equal(4m, oNearMintA.TotalCost);              // only one copy has a price paid
            Assert.Equal(6m, oNearMintA.Gain);                   // ... so gain counts only that copy
        }
    }

    public class CMFormatTests {

        [Fact]
        public void Money_FormatsByCurrency_AndKeepsTheSignInFront() {
            Assert.Equal("$1,234.50", CMFormat.Money(1234.5m));
            Assert.Equal("-$20.00", CMFormat.Money(-20m));
            Assert.Equal("12.00 GBP", CMFormat.Money(12m, "GBP"));
            Assert.Equal("", CMFormat.Money(null));
            Assert.EndsWith("647.64", CMFormat.Money(647.64m, "EUR"));
            Assert.Equal(7, CMFormat.Money(647.64m, "EUR").Length);
        }

        [Fact]
        public void Percent_ShowsTheSign() {
            Assert.Equal("+12.3%", CMFormat.Percent(0.123m));
            Assert.Equal("-4.0%", CMFormat.Percent(-0.04m));
            Assert.Equal("", CMFormat.Percent(null));
        }
    }
}
