using System.Drawing;

namespace TelemetryStash.IliDisplay.GraphicDriver
{
    public abstract class MetricDefinition
    {
        protected MetricDefinition(int x, int y, int width, int height, Color backgroundColor)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            BackgroundColor = backgroundColor;
        }

        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public Color BackgroundColor { get; }
    }
}
