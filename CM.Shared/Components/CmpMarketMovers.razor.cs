using Microsoft.AspNetCore.Components;

namespace CM.Shared {

    /// <summary>A table of price movers: what each card cost before, what it costs now, and the change.</summary>
    public class CmpMarketMovers_Base : ComponentBase {

        [Parameter] public List<MarketCardRow> Rows { get; set; } = new();
    }
}
