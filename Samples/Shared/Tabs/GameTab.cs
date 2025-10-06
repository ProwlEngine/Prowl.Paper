using Prowl.PaperUI;

namespace Shared.Tabs
{
    public class GameTab : Tab
    {
        public GameTab(Paper gui) : base(gui)
        {
            title = "Game";
            id = "game";
            width = 55;
        }

        public override void Body()
        {
            using (Gui.Row("Tools").RowBetween(5).Margin(4).Top(5).Height(20).Enter())
            {
                Gui.Box("Tool 1")
                    .Text(Icons.ArrowsTurnToDots, Fonts.arial).FontSize(12)
                    .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                    .Hovered.BackgroundColor(Themes.base250).End();
                Gui.Box("Tool 2")
                    .Text(Icons.ArrowsDownToLine, Fonts.arial).FontSize(12)
                    .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                    .Hovered.BackgroundColor(Themes.base250).End();
                Gui.Box("Tool 3")
                    .Text(Icons.Anchor, Fonts.arial).FontSize(12)
                    .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                    .Hovered.BackgroundColor(Themes.base250).End();
                Gui.Box("Tool 4")
                    .Text(Icons.AngleUp, Fonts.arial).FontSize(12)
                    .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                    .Hovered.BackgroundColor(Themes.base250).End();

                Gui.Box("Spacer");

                Gui.Box("Tool 4")
                .Text(Icons.ArrowsLeftRight, Fonts.arial).FontSize(12)
                .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                .Hovered.BackgroundColor(Themes.base250).End();
            }

            Gui.Box("Game View").Margin(2).Rounded(5).BackgroundColor(System.Drawing.Color.Black);
        }
    }
}