using Prowl.PaperUI;

namespace Shared.Tabs
{
    public class InspectorTab : Tab
    {
        public InspectorTab(Paper gui) : base(gui)
        {
            title = "Inspector";
            id = "inspector";
            width = 83;
        }

        public override void Body()
        {
            using (Gui.Box("Search Box").Height(28).Margin(5).Top(8).Bottom(8).Rounded(5).BackgroundColor(Themes.base300).Enter())
            {
                Gui.Box("Search").Text("Filter...", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Left(8)
                    .Alignment(TextAlignment.MiddleLeft);
            }

            PaperDemo.HierarchyItem(PaperDemo.rootItem);
        }
    }
}