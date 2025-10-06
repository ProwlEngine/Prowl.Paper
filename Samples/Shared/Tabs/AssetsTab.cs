using Prowl.PaperUI;

namespace Shared.Tabs
{
    public class AssetsTab : Tab
    {
        public AssetsTab(Paper gui) : base(gui)
        {
            title = "Assets";
            id = "assets";
            width = 65;
        }

        public override void Body()
        {
            using (Gui.Box("Search Box").Margin(5).Height(28).Bottom(8).Rounded(5).BackgroundColor(Themes.base300).Enter())
            {
                Gui.Box("Search").Text("Filter...", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Left(8)
                    .Alignment(TextAlignment.MiddleLeft);
            }

            using (Gui.Column("Assets view").Margin(5).SetScroll(Scroll.ScrollY).Enter())
            {
                using (Gui.Row("Folders and assets").RowBetween(8).Enter())
                {
                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);

                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);

                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);

                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);
                }
                using (Gui.Row("Folders and assets").RowBetween(8).Enter())
                {
                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);

                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);

                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);

                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);
                }

                using (Gui.Row("Folders and assets").RowBetween(8).Enter())
                {
                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);

                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);

                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);

                    Gui.Box("Folder")
                        .FontSize(32)
                        .Text(Icons.Folder, Fonts.arial);
                }
            }
        }


    }
}