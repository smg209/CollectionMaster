using Microsoft.AspNetCore.Components;

namespace CM.Shared {

    /// <summary>A table of cards or sealed products with their TCGplayer market price.</summary>
    public class CmpMarketValues_Base : ComponentBase {

        [Parameter] public List<MarketCardRow> Rows { get; set; } = new();
    }
}
