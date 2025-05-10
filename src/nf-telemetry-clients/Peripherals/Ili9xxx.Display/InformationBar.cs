using nanoFramework.UI;
using System.Drawing;

namespace TelemetryStash.IliDisplay
{
    public static class InformationBar
    {
        private static Bitmap _logo = null;

        public static void DrawInformationBar(Bitmap screen, Font font, string text)
        {
            screen.DrawText("0x0", font, Color.Black, 10, 0);
            screen.DrawText("480x0", font, Color.Black, 420, 0);
            screen.DrawText("480x320", font, Color.Black, 405, 300);
            screen.DrawText("0x320", font, Color.Black, 10, 300);
            screen.DrawText(text, font, Color.WhiteSmoke, 240, 160);
        }

        public static void DrawLogo(Bitmap screen)
        {
            _logo ??= Resource.GetBitmap(Resource.BitmapResources.ripTideCartoon);

            //var img = new Bitmap(Resource.GetBytes(Resource.BinaryResources.nanoFrameworkLogo), Bitmap.BitmapImageType.Bmp);

            screen.DrawImage(0, 0, _logo, 0, 0, 480, 320);
        }
    }
}
