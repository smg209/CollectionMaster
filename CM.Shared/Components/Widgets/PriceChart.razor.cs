using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Text;

namespace CM.Shared {

    /// <summary>One line on a PriceChart: a name and its points in time order.</summary>
    public class ChartSeries {
        public string Name { get; set; } = "";
        public List<ChartPoint> Points { get; set; } = new();
    }

    public class ChartPoint {
        public DateTime When { get; set; }
        public decimal Value { get; set; }
    }

    public class ChartPath {
        public string Name { get; set; } = "";
        public string Color { get; set; } = "";
    }

    /// <summary>
    /// Draws one or more value-over-time lines. Give it Series; it works out the scales itself.
    /// A series needs two points to make a line - with fewer it shows EmptyText instead, which
    /// is the normal state of a price that has only just started being recorded.
    /// </summary>
    public class PriceChart_Base : ComponentBase {

        [Parameter] public List<ChartSeries> Series { get; set; } = new();
        [Parameter] public string Title { get; set; } = "Price history";
        [Parameter] public string CurrencyCode { get; set; } = "USD";
        [Parameter] public string EmptyText { get; set; } = "Not enough price history yet - it builds up as prices are refreshed each day.";
        [Parameter] public int Width { get; set; } = 640;
        [Parameter] public int Height { get; set; } = 220;
        [Parameter] public bool ShowLegend { get; set; }
        /// <summary>True draws each value as holding until the next point (prices); false joins the points directly (daily totals).</summary>
        [Parameter] public bool Stepped { get; set; } = true;

        // Distinct, readable on light and dark themes.
        private static readonly string[] Colors = { "#2E7DD1", "#E07B39", "#3BA272", "#B455B6", "#D14B4B", "#8A8F2A", "#5B6ABF", "#1F9E9E" };

        protected bool _hasData;
        protected string _svgBody = "";
        protected List<ChartPath> _paths = new();
        protected readonly int _left = 62;
        protected readonly int _right = 12;
        private readonly int _top = 10;
        private readonly int _bottom = 24;

        protected static string F(double tdValue) => tdValue.ToString("0.##", CultureInfo.InvariantCulture);

        protected override void OnParametersSet() {
            _svgBody = "";
            _paths = new();

            List<ChartSeries> oSeries = Series.Where(oOne => oOne.Points.Count >= 2).ToList();
            _hasData = oSeries.Count > 0;
            if(!_hasData) return;

            DateTime oStart = oSeries.Min(oOne => oOne.Points.Min(oPoint => oPoint.When));
            DateTime oEnd = oSeries.Max(oOne => oOne.Points.Max(oPoint => oPoint.When));
            double dSpan = Math.Max(1, (oEnd - oStart).TotalSeconds);

            double dMin = (double)oSeries.Min(oOne => oOne.Points.Min(oPoint => oPoint.Value));
            double dMax = (double)oSeries.Max(oOne => oOne.Points.Max(oPoint => oPoint.Value));
            if(dMax - dMin < 0.01) { dMin -= Math.Max(0.5, dMin * 0.05); dMax += Math.Max(0.5, dMax * 0.05); }
            double dPad = (dMax - dMin) * 0.08;
            dMin = Math.Max(0, dMin - dPad);
            dMax += dPad;

            double dPlotWidth = Width - _left - _right;
            double dPlotHeight = Height - _top - _bottom;
            double X(DateTime toWhen) => _left + (toWhen - oStart).TotalSeconds / dSpan * dPlotWidth;
            double Y(double tdValue) => _top + (1 - (tdValue - dMin) / (dMax - dMin)) * dPlotHeight;

            // Colours come from currentColor, which the stylesheet sets from the theme on the <svg>.
            var oSvg = new StringBuilder();
            for(int i = 0; i <= 4; i++) {
                double dValue = dMin + (dMax - dMin) * i / 4;
                double dY = Y(dValue);
                string sLabel = System.Net.WebUtility.HtmlEncode(CMFormat.Money((decimal)dValue, CurrencyCode));
                oSvg.Append($"<line x1=\"{_left}\" x2=\"{Width - _right}\" y1=\"{F(dY)}\" y2=\"{F(dY)}\" stroke=\"currentColor\" stroke-opacity=\"0.2\" stroke-width=\"1\" />");
                oSvg.Append($"<text x=\"{_left - 6}\" y=\"{F(dY + 4)}\" text-anchor=\"end\" font-size=\"11\" fill=\"currentColor\">{sLabel}</text>");
            }
            oSvg.Append($"<text x=\"{_left}\" y=\"{Height - 6}\" text-anchor=\"start\" font-size=\"11\" fill=\"currentColor\">{oStart:MMM d, yyyy}</text>");
            oSvg.Append($"<text x=\"{Width - _right}\" y=\"{Height - 6}\" text-anchor=\"end\" font-size=\"11\" fill=\"currentColor\">{oEnd:MMM d, yyyy}</text>");

            int iColor = 0;
            foreach(ChartSeries oOne in oSeries) {
                var oData = new StringBuilder();
                double dLastX = 0, dLastY = 0;
                bool bFirst = true;

                foreach(ChartPoint oPoint in oOne.Points.OrderBy(oPoint => oPoint.When)) {
                    double dX = X(oPoint.When), dY = Y((double)oPoint.Value);
                    if(bFirst) {
                        oData.Append($"M{F(dX)} {F(dY)}");
                        bFirst = false;
                    } else if(Stepped) {
                        oData.Append($" H{F(dX)} V{F(dY)}");   // hold the old value until this moment, then move
                    } else {
                        oData.Append($" L{F(dX)} {F(dY)}");
                    }
                    dLastX = dX;
                    dLastY = dY;
                }

                string sColor = Colors[iColor % Colors.Length];
                oSvg.Append($"<path d=\"{oData}\" fill=\"none\" stroke=\"{sColor}\" stroke-width=\"2\" stroke-linejoin=\"round\" />");
                oSvg.Append($"<circle cx=\"{F(dLastX)}\" cy=\"{F(dLastY)}\" r=\"3\" fill=\"{sColor}\" />");
                _paths.Add(new ChartPath { Name = oOne.Name, Color = sColor });
                iColor++;
            }

            _svgBody = oSvg.ToString();
        }
    }
}
