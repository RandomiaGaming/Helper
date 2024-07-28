using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Helper.Functions
{
    public static class WallpaperFunctions
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [RegisterFunction("Sets the wallpaper of the current user to a randomly generated cat.")]
        public static void SetWallpaperToCat()
        {
            //Download a cat
            Assembly assembly = Assembly.GetCallingAssembly();
            WebClient webClient = new WebClient();
            byte[] catBytes = webClient.DownloadData("http://www.thiscatdoesnotexist.com/");
            MemoryStream catStream = new MemoryStream(catBytes);
            Image img = Image.FromStream(catStream);
            string catBPMPath = Path.GetDirectoryName(assembly.Location) + "\\Cat.bmp";
            img.Save(catBPMPath, ImageFormat.Bmp);

            //Set the cat as the desktop wallpaper.
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", "2");
            key.SetValue(@"TileWallpaper", "0");
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, catBPMPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
