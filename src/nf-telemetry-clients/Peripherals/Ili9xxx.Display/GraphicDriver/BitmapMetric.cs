using nanoFramework.UI;
using System.Drawing;

namespace TelemetryStash.IliDisplay.GraphicDriver
{
    public class BitmapMetric : MetricDefinition
    {
        public BitmapMetric(int x, int y, int width, int height, Color backgroundColor, Bitmap bitmap)
            : base(x, y, width, height, backgroundColor)
        {
            Image = bitmap;
        }

        public Bitmap Image { get; }
    }
}
