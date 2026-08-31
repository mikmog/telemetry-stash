using System.Drawing;

namespace TelemetryStash.IliDisplay.GraphicDriver
{
    public class TextMetric : MetricDefinition
    {
        public TextMetric(int x, int y, int width, int height, Color backgroundColor, nanoFramework.UI.Font font, int fontHeight, Color textColor)
            : base(x, y, width, height, backgroundColor)
        {
            Font = font;
            FontHeight = fontHeight;
            TextColor = textColor;
        }

        public nanoFramework.UI.Font Font { get; }
        public int FontHeight { get; }
        public Color TextColor { get; }
    }
}
