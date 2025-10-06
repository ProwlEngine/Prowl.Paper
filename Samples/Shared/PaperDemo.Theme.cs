using System.Drawing;
using Prowl.PaperUI;

// SHADCN discord theme colors
// discovered through https://ui.jln.dev/
// @layer base {
//     
//     .dark {
//       --background: 217.5 9.09% 17.25%; /* 40, 43, 48 */
//       --foreground: 334 34% 98%; /* 255, 247, 252 */
//       --muted: 210 9.09% 12.94%; /* 30, 32, 36 */
//       --muted-foreground: 334 0% 60.77%; /* 155, 155, 155 */
//       --popover: 210 9.09% 12.94%; /* 30, 32, 36 */
//       --popover-foreground: 334 34% 98%; /* 255, 247, 252 */
//       --card: 210 9.09% 12.94%; /* 30, 32, 36 */
//       --card-foreground: 334 34% 98%; /* 255, 247, 252 */
//       --border: 334 0% 18.46%; /* 47, 47, 47 */
//       --input: 214.29 5.04% 27.25%; /* 66, 68, 72 */
//       --primary: 226.73 58.43% 65.1%; /* 69, 135, 235 */
//       --primary-foreground: 0 0% 100%; /* 255, 255, 255 */
//       --secondary: 214.29 5.04% 27.25%; /* 66, 68, 72 */
//       --secondary-foreground: 334 0% 100%; /* 255, 255, 255 */
//       --accent: 217.5 9.09% 17.25%; /* 40, 43, 48 */
//       --accent-foreground: 226.73 58.43% 65.1%; /* 69, 135, 235 */
//       --destructive: 358.16 68.78% 53.53%; /* 230, 43, 52 */
//       --destructive-foreground: 0 0% 100%; /* 255, 255, 255 */
//       --ring: 217.5 9.09% 17.25%; /* 40, 43, 48 */
//       --chart-1: 226.73 58.43% 65.1%; /* 69, 135, 235 */
//       --chart-2: 214.29 5.04% 27.25%; /* 66, 68, 72 */
//       --chart-3: 217.5 9.09% 17.25%; /* 40, 43, 48 */
//       --chart-4: 214.29 5.04% 30.25%; /* 72, 74, 79 */
//       --chart-5: 226.73 61.43% 65.1%; /* 64, 135, 239 */
//     }
//
//     :root {
//       --background: 0 0% 97.69%; /* 249, 249, 249 */
//       --foreground: 334 55% 1%; /* 4, 1, 3 */
//       --muted: 0 0% 93.85%; /* 239, 239, 239 */
//       --muted-foreground: 0 0% 10.2%; /* 26, 26, 26 */
//       --popover: 0 0% 100%; /* 255, 255, 255 */
//       --popover-foreground: 0 0% 10.2%; /* 26, 26, 26 */
//       --card: 0 0% 100%; /* 255, 255, 255 */
//       --card-foreground: 0 0% 13.73%; /* 35, 35, 35 */
//       --border: 0 0% 80.78%; /* 206, 206, 206 */
//       --input: 0 0% 80.78%; /* 206, 206, 206 */
//       --primary: 211.29 100% 50%; /* 0, 149, 255 */
//       --primary-foreground: 0 0% 100%; /* 255, 255, 255 */
//       --secondary: 0 0% 85.49%; /* 218, 218, 218 */
//       --secondary-foreground: 334 0% 10%; /* 26, 26, 26 */
//       --accent: 211.29 100% 50%; /* 0, 149, 255 */
//       --accent-foreground: 334 0% 100%; /* 255, 255, 255 */
//       --destructive: 3.19 100% 59.41%; /* 303, 1, 0 */
//       --destructive-foreground: 18 0% 100%; /* 255, 255, 255 */
//       --ring: 0 0% 60%; /* 153, 153, 153 */
//       --chart-1: 211.29 100% 50%; /* 0, 149, 255 */
//       --chart-2: 0 0% 85.49%; /* 218, 218, 218 */
//       --chart-3: 211.29 100% 50%; /* 0, 149, 255 */
//       --chart-4: 0 0% 88.49%; /* 226, 226, 226 */
//       --chart-5: 211.29 103% 50%; /* 0, 152, 255 */
//       --radius: 0.5rem;
//     }
//   }

namespace Shared
{
    public static class Themes
    {
        //Theme
        public static Color base100;
        public static Color base200;
        public static Color base250;
        public static Color base300;
        public static Color baseContent;
        public static Color primary;
        public static Color primaryContent;
        // public static Color lightTextColor;
        public static Color[] colorPalette;

        public static void Initialize()
        {
            //Dark
            base100 = Color.FromArgb(255, 23, 23, 23);
            base200 = Color.FromArgb(255, 42, 42, 42);
            base250 = Color.FromArgb(255, 52, 52, 52);
            base300 = Color.FromArgb(255, 64, 64, 64);
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