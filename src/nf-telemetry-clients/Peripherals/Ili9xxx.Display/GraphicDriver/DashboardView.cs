using nanoFramework.UI;
using System.Drawing;

namespace TelemetryStash.IliDisplay.GraphicDriver
{
    public class DashboardView : IliView
    {
        private readonly Bitmap _background;

        public DashboardView(Bitmap background)
        {
            _background = background;
        }

        public override void Render(Bitmap screen)
        {
            screen.DrawImage(
                xDst: 0,
                yDst: 0,
                bitmap: _background,
                xSrc: 0,
                ySrc: 0,
                width: _background.Width,
                height: _background.Height,
                opacity: Bitmap.OpacityOpaque);
        }

        public override void Dispose()
        {
        }

        public void UpdateMetric(TextMetric metric, Bitmap screen, string value)
        {
            // Clear placeholder region with background color
            screen.DrawRectangle(
                colorOutline: metric.BackgroundColor,
                thicknessOutline: 0,
                x: metric.X,
                y: metric.Y,
                width: metric.Width,
                height: metric.Height,
                xCornerRadius: 0,
                yCornerRadius: 0,
                colorGradientStart: metric.BackgroundColor,
                xGradientStart: metric.X,
                yGradientStart: metric.Y,
                colorGradientEnd: metric.BackgroundColor,
                xGradientEnd: metric.X + metric.Width,
                yGradientEnd: metric.Y + metric.Height,
                opacity: Bitmap.OpacityOpaque);

            // Calculate text position (bottom-left aligned)
            var textX = metric.X;
            var textY = metric.Y + metric.Height - metric.FontHeight;

            screen.DrawText(value, metric.Font, metric.TextColor, textX, textY);

            // Partial flush for the metric region only
            screen.Flush(metric.X, metric.Y, metric.Width, metric.Height);
        }

        public void UpdateMetric(BitmapMetric metric, Bitmap screen, Bitmap bitmap)
        {
            // Clear placeholder region with background color
            screen.DrawRectangle(
                colorOutline: metric.BackgroundColor,
                thicknessOutline: 0,
                x: metric.X,
                y: metric.Y,
                width: metric.Width,
                height: metric.Height,
                xCornerRadius: 0,
                yCornerRadius: 0,
                colorGradientStart: metric.BackgroundColor,
                xGradientStart: metric.X,
                yGradientStart: metric.Y,
                colorGradientEnd: metric.BackgroundColor,
                xGradientEnd: metric.X + metric.Width,
                yGradientEnd: metric.Y + metric.Height,
                opacity: Bitmap.OpacityOpaque);

            // Draw bitmap bottom-left aligned
            var bitmapY = metric.Y + metric.Height - bitmap.Height;

            screen.DrawImage(
                xDst: metric.X,
                yDst: bitmapY,
                bitmap: bitmap,
                xSrc: 0,
                ySrc: 0,
                width: bitmap.Width,
                height: bitmap.Height,
                opacity: Bitmap.OpacityOpaque);

            // Partial flush for the metric region only
            screen.Flush(metric.X, metric.Y, metric.Width, metric.Height);
        }
    }
}
