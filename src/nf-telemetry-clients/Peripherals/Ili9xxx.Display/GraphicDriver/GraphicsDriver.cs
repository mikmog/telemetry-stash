using nanoFramework.UI;

namespace TelemetryStash.IliDisplay.GraphicDriver
{
    public class GraphicsDriver
    {
        public Bitmap Screen { get; }
        private IliView _activeView;

        public GraphicsDriver(Bitmap screen)
        {
            Screen = screen;
        }

        public void SetView(IliView view)
        {
            _activeView?.Dispose();
            _activeView = view;
            _activeView.Render(Screen);
            Screen.Flush();
        }
    }
}
