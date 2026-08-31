using nanoFramework.UI;
using System.Drawing;

namespace TelemetryStash.IliDisplay.GraphicDriver
{
    public class GenericView : IliView
    {
        private readonly Color _backgroundColor;
        private readonly int _width;
        private readonly int _height;

        public GenericView(Color backgroundColor, int width, int height)
        {
            _backgroundColor = backgroundColor;
            _width = width;
            _height = height;
        }

        public override void Render(Bitmap screen)
        {
            screen.DrawRectangle(
                colorOutline: _backgroundColor,
                thicknessOutline: 0,
                x: 0,
                y: 0,
                width: _width,
                height: _height,
                xCornerRadius: 0,
                yCornerRadius: 0,
                colorGradientStart: _backgroundColor,
                xGradientStart: 0,
                yGradientStart: 0,
                colorGradientEnd: _backgroundColor,
                xGradientEnd: _width,
                yGradientEnd: _height,
                opacity: Bitmap.OpacityOpaque);
        }

        public override void Dispose()
        {
        }
    }
}
