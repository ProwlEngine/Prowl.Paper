using System.ComponentModel;
using System.Data.Common;

using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;
using Prowl.PaperUI.Themes.Origami;

namespace Shared
{
    public static partial class PaperDemo
    {
        public struct Tab
        {
            public string id;
            public string title;
            public double width;
            public bool active;
        }

        public static Paper Gui;
        static double value = 0;

        public static void Initialize(Paper paper)
        {
            Gui = paper;
            Fonts.Initialize(Gui);
            Themes.Initialize();
        }

        public static void RenderUI()
        {
            using (Gui.Column("EditorContainer").BackgroundColor(Themes.base100).Enter())
            {
                TitleBarUI();

                using (Gui.Row("3 Columns Editor Layout").Enter())
                {
                    using (Gui.Column("Left Panel").Width(250).Enter())
                    {
                        using (WindowContainer("Scene Tree Window").Enter())
                        {
                            var tabs = new Tab[]
                            {
                                new Tab { id = "hierarchy", title = "Hierarchy", width = 83, active = true },
                                new Tab { id = "assets", title = "Assets", width = 65, active = false },
                            };

                            using (TabsContainer("Body", tabs).Enter())
                            {
                                using (Gui.Box("Search Box").Height(28).Margin(5).Top(8).Bottom(8).Rounded(5).BackgroundColor(Themes.base300).Enter())
                                {
                                    Gui.Box("Search").Text("Filter...", Fonts.arial)
                                        .TextColor(Themes.baseContent)
                                        .Left(8)
                                        .Alignment(TextAlignment.MiddleLeft);
                                }

                                Gui.Box("Weeee1")
                                    .Text(" Entity 1", Fonts.arial).TextColor(Themes.baseContent).Alignment(TextAlignment.MiddleLeft)
                                    .Height(28).Left(5).Right(5).Bottom(5)
                                    .Rounded(5).BackgroundColor(Themes.base200)
                                    .Hovered
                                        .BackgroundColor(Themes.base300)
                                    .End();
                                Gui.Box("Weeee2").Text("    Child Entity A", Fonts.ariali).TextColor(Themes.baseContent).Height(28).Margin(5);
                                Gui.Box("Weeee3").Text("    Child Entity B", Fonts.ariali).TextColor(Themes.baseContent).Height(28).Margin(5);
                                Gui.Box("Weeee4").Text(" Entity 2", Fonts.arial).TextColor(Themes.baseContent).Height(28).Margin(5);
                                Gui.Box("Weeee5").Text(" Entity 3", Fonts.arial).TextColor(Themes.baseContent).Height(28).Margin(5);
                            }
                        }

                        using (WindowContainer("Scene Tree Window COntainer").Enter())
                        {
                            var tabs = new Tab[]
                            {
                                new Tab { id = "files", title = "Files", width = 65, active = true },
                                new Tab { id = "assets", title = "Settings", width = 80, active = false },
                            };

                            using (TabsContainer("Body", tabs).Enter())
                            {
                                Gui.Box("Weeee").Text("Scene", Fonts.arialb).TextColor(Themes.primary).Height(28).Margin(5);
                                Gui.Box("Weeee1").Text("- Entity 1", Fonts.arial).TextColor(Themes.baseContent).Height(28).Margin(5);
                                Gui.Box("Weeee2").Text("  - Child Entity A", Fonts.ariali).TextColor(Themes.baseContent).Height(28).Margin(5);
                                Gui.Box("Weeee3").Text("  - Child Entity B", Fonts.ariali).TextColor(Themes.baseContent).Height(28).Margin(5);
                                Gui.Box("Weeee4").Text("- Entity 2", Fonts.arial).TextColor(Themes.baseContent).Height(28).Margin(5);
                                Gui.Box("Weeee5").Text("- Entity 3", Fonts.arial).TextColor(Themes.baseContent).Height(28).Margin(5);
                            }
                        }
                    }

                    using (Gui.Column("Center Panel").BackgroundColor(Themes.base300).Left(2.5).Bottom(5).Right(2.5).Rounded(5).Enter())
                    {
                        Button.Secondary("Primary Button").OnClick((_) => Console.WriteLine("Primary Button Clicked")).Margin(5);
                    }

                    using (Gui.Column("Right Panel").Width(250).Enter())
                    {
                        Button.Secondary("Primary Button").OnClick((_) => Console.WriteLine("Primary Button Clicked")).Margin(5);
                    }
                }
            }
        }

        public struct Item
        {
            public string id;
            public string title;
            public int depth;
            public Item[] children;
            public bool expanded;
            public bool selected;
            public Action<Item> onClick; // Open inspector etc
        }

        // TODO recursive hierarchy
        private static void HierarchyItem(Item item)
        {

        } 

        private static ElementBuilder WindowContainer(string id)
        {
            return Gui.Box(id)
                .BackgroundColor(Themes.base100)
                .Left(6).Bottom(6).Right(3.5)
                .Rounded(5)
                .BorderColor(Themes.base200)
                .BorderWidth(1);
        }

        private static ElementBuilder TabsContainer(string id, Tab[] tabs)
        {
            using (Gui.Row("Tabs").Height(28).Left(5).Right(5).Enter())
            {
                foreach (var tab in tabs)
                {
                    if (tab.active)
                    {
                        using (Gui.Box("Active Tab").Width(tab.width).Height(28).Enter())
                        {
                            Gui.Box("Highlight").RoundedTop(2).BackgroundColor(Themes.primary).Height(3);
                            Gui.Box("tab 1")
                                .BackgroundColor(Themes.base200)
                                .Text(tab.title, Fonts.arial)
                                .TextColor(Themes.baseContent)
                                .Alignment(TextAlignment.MiddleCenter);
                        }
                    }
                    else
                    {
                        Gui.Box("tab 2")
                            .RoundedTop(3)
                            .Width(tab.width).Height(28)
                            .BackgroundColor(Themes.base100)
                            .Text(tab.title, Fonts.arial)
                            .TextColor(Themes.baseContent)
                            .Hovered
                                .BackgroundColor(Themes.base300)
                            .End()
                            .Alignment(TextAlignment.MiddleCenter)
                            .Left(5);
                    }
                }

                Gui.Box("Plus Tab")
                    .Width(28).Height(28)
                    .BackgroundColor(Themes.base100)
                    .Text("+", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Alignment(TextAlignment.MiddleCenter)
                    .Hovered
                        .BackgroundColor(Themes.base300)
                    .End();
            }

            return Gui.Column("Body").BackgroundColor(Themes.base200);
        }

        private static void TitleBarUI()
        {
            using (Gui.Row("Header").Height(28).Margin(5).Enter())
            {
                Gui.Box("tab 1")
                    .Width(80).Height(28)
                    .BackgroundColor(Themes.base100)
                    .Text(Icons.Hammer + "  Bevy", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Alignment(TextAlignment.MiddleCenter)
                    .Left(5);

                Gui.Box("tab 2")
                    .Width(45).Height(28)
                    .BackgroundColor(Themes.base100)
                    .Text("File", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Rounded(5)
                    .Hovered
                        .BackgroundColor(Themes.base300)
                    .End()
                    .Alignment(TextAlignment.MiddleCenter)
                    .Left(5);

                Gui.Box("tab 3")
                    .Width(45).Height(28)
                    .BackgroundColor(Themes.base100)
                    .Text("Edit", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Rounded(5)
                    .Hovered
                        .BackgroundColor(Themes.base300)
                    .End()
                    .Alignment(TextAlignment.MiddleCenter)
                    .Left(5);

                Gui.Box("tab 3")
                    .Width(65).Height(28)
                    .BackgroundColor(Themes.base100)
                    .Text("Debug", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Rounded(5)
                    .Hovered
                        .BackgroundColor(Themes.base300)
                    .End()
                    .Alignment(TextAlignment.MiddleCenter)
                    .Left(5);

                Gui.Box("tab 3")
                   .Width(64).Height(28)
                   .PositionType(PositionType.SelfDirected)
                   .BackgroundColor(Themes.base300)
                   .Text(Icons.Play + "    " + Icons.Pause, Fonts.arial)
                   .TextColor(Themes.baseContent)
                   .Rounded(5)
                   .Hovered
                       .BackgroundColor(Themes.base200)
                   .End()
                   .Alignment(TextAlignment.MiddleCenter)
                   .Left(500);
            }
        }

        private static void ComponentDemo()
        {
            using (Gui.Column("EditorContainer").Margin(5).Enter())
            {
                Button.Secondary("Primary Button").OnClick((_) => Console.WriteLine("Primary Button Clicked")).Margin(5);
                Button.Secondary("Secondary Button").OnClick((_) => Console.WriteLine("Secondary Button Clicked")).Margin(5);

                TextArea.Secondary("TextArea", "This is a secondary text area. It can hold multiple lines of text and is styled differently from the primary text area.").Margin(5);
                Slider.Primary("Slider 1", value, (v) => { value = v; });
                Slider.Secondary("Slider 1", value, (v) => { value = v; });
                PieChart.Primary("Pie Chart", new double[] { 10, 20, 30, 40 }, value).Height(200).Margin(5);
            }
        }
    }
}