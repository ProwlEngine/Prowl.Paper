using Prowl.PaperUI;

namespace Shared
{
    public static partial class PaperDemo
    {
        static double time = 0;

        public static Paper Gui;

        public static void Initialize(Paper paper)
        {
            Gui = paper;
            Fonts.Initialize(Gui);
            Themes.Initialize();
        }

        public static void RenderUI()
        {
            // Update time for animations
            time += 0.016f; // Assuming ~60fps

            // Main container with light gray background
            using (Gui.Column("EditorContainer").BackgroundColor(Themes.backgroundColor).Enter())
            {
                using (Gui.Box("First Tab").BackgroundColor(Themes.backgroundColor).Text("Scene Tree", Fonts.arial).Enter())
                {
                }
            }
        }
    }
}