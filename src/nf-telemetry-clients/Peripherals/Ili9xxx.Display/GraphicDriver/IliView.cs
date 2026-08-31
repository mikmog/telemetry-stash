using nanoFramework.UI;

namespace TelemetryStash.IliDisplay.GraphicDriver
{
    public abstract class IliView
    {
        public abstract void Render(Bitmap screen);

        public abstract void Dispose();
    }
}
