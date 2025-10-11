using System.Drawing;

namespace Shared
{
    public static class Themes
    {
        //Theme
        public static Color base100;
        public static Color base200;
        public static Color base250;
        public static Color base300;
        public static Color base400;
        public static Color baseContent; // text
        public static Color primary;
        public static Color primaryContent; // text
        public static Color[] colorPalette;

        public static void Initialize()
        {
            //Dark
            base100 = Color.FromArgb(255, 31, 31, 36);
            base200 = Color.FromArgb(255, 42, 42, 46);
            base250 = Color.FromArgb(255, 54, 55, 59);
            base300 = Color.FromArgb(255, 64, 64, 68);
            base400 = Color.FromArgb(255, 70, 71, 76);
            baseContent = Color.FromArgb(255, 230, 230, 230);
            primary = Color.FromArgb(255, 69, 135, 235);
            primaryContent = Color.FromArgb(255, 226, 232, 240);
            colorPalette = [
                Color.FromArgb(255, 94, 234, 212),   // Cyan
                Color.FromArgb(255, 162, 155, 254),  // Purple  
                Color.FromArgb(255, 249, 115, 22),   // Orange
                Color.FromArgb(255, 248, 113, 113),  // Red
                Color.FromArgb(255, 250, 204, 21)    // Yellow
            ];

            Components.DefineStyles();
        }
    }
}