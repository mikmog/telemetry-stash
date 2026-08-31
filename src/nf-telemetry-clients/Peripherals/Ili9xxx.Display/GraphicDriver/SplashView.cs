using nanoFramework.UI;

namespace TelemetryStash.IliDisplay.GraphicDriver
{
    public class SplashView : IliView
    {
        private readonly Bitmap _splash;

        public SplashView(Bitmap splash)
        {
            _splash = splash;
        }

        public override void Render(Bitmap screen)
        {
            screen.DrawImage(
                xDst: 0,
                yDst: 0,
                bitmap: _splash,
                xSrc: 0,
                ySrc: 0,
                width: _splash.Width,
                height: _splash.Height,
                opacity: Bitmap.OpacityOpaque);
        }

        public override void Dispose()
        {
            _splash?.Dispose();
        }
    }
}
