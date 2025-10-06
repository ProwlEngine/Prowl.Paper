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

        public class Item
        {
            public string id;
            public string title;
            public int depth;
            public Item[] children;
            public bool expanded;
            public bool selected;
            public Action<Item> onClick; // Open inspector etc
        }

        public static string selectedItemId = "";


        public static Paper Gui;
        static double value = 0;

        public static Item rootItem = new Item
        {
            id = "1",
            title = "Main Entity",
            depth = 0,
            expanded = true,
            selected = false,
            onClick = (item) => Console.WriteLine($"Clicked {item.title}"),
            children = new[] {
                new Item {
                    id = "1.1",
                    title = "Child A",
                    depth = 1,
                    expanded = true,
                    selected = true,
                    onClick = (item) => Console.WriteLine($"Clicked {item.title}")
                },
                new Item {
                    id = "1.2",
                    title = "Child B",
                    depth = 1,
                    expanded = false,
                    selected = false,
                    onClick = (item) => Console.WriteLine($"Clicked {item.title}"),
                    children = new[] {
                        new Item {
                            id = "1.2.1",
                            title = "Subchild 1",
                            depth = 2,
                            onClick = (item) => Console.WriteLine($"Clicked {item.title}")
                        }
                    }
                }
            }
        }; 

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

                using (Gui.Row("3 Columns Editor Layout").Bottom(6).Left(6).Right(6).RowBetween(6).Enter())
                {
                    using (Gui.Column("Left Panel").ColBetween(8).Width(250).Enter())
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

                                HierarchyItem(rootItem);
                            }
                        }

                        using (WindowContainer("Files Window Container").Enter())
                        {
                            var tabs = new Tab[]
                            {
                                new Tab { id = "files", title = "Files", width = 65, active = true },
                                new Tab { id = "assets", title = "Settings", width = 80, active = false },
                            };

                            using (TabsContainer("Body", tabs).BorderTop(8).Enter())
                            {
                                HierarchyItem(rootItem);
                            }
                        }
                    }

                    using (Gui.Column("Center Panel").BackgroundColor(Themes.base300).Rounded(5).Enter())
                    {
                        
                    }

                    using (Gui.Column("Right Panel").Width(250).Enter())
                    {
                        using (WindowContainer("Scene Tree Window").Enter())
                        {
                            var tabs = new Tab[]
                            {
                                new Tab { id = "inspector", title = "Inspector", width = 83, active = true },
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

                                HierarchyItem(rootItem);
                            }
                        }
                    }
                }
            }
        }

        private static void HierarchyItem(Item item)
        {
            var isSelected = selectedItemId == item.id;

            // ensure proper padding with parent
            using (Gui.Row(item.id).Height(28).Margin(5).Top(0).Bottom(0).Rounded(5)
                .BackgroundColor(isSelected ? Themes.base250 : Themes.base200)
                .Hovered
                    .BackgroundColor(Themes.base250)
                .End()
                .OnClick((_) =>
                {
                    selectedItemId = item.id;
                    item.onClick?.Invoke(item);
                })
                .Enter())
            {
                if (item.children != null && item.children.Length > 0)
                {
                    Gui.Box("toggle" + item.id).Text(item.expanded ? Icons.ChevronDown : Icons.ChevronRight, Fonts.arial)
                        .Width(28)
                        .Alignment(TextAlignment.MiddleCenter)
                        .FontSize(8)
                        .OnClick((_) => item.expanded = !item.expanded);
                }
                else
                {
                    Gui.Box("icon" + item.id).Text(Icons.Cube, Fonts.arial)
                        .Width(28)
                        .Alignment(TextAlignment.MiddleCenter)
                        .FontSize(10)
                        .OnClick((_) => item.expanded = !item.expanded);
                }

                Gui.Box("box" + item.id).Text(item.title, Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Alignment(TextAlignment.MiddleLeft);

                if (isSelected)
                {
                    Gui.Box("indicator").Height(28).Width(3).Rounded(2).BackgroundColor(Themes.primary);
                }
            }

            if (item.expanded && item.children != null)
            {
                using (Gui.Box("Children of" + item.id).Left(10).Enter())
                {
                    foreach (var child in item.children)
                    {
                        HierarchyItem(child);
                    }
                }
            }
        }

        private static ElementBuilder WindowContainer(string id)
        {
            return Gui.Box(id)
                .BackgroundColor(Themes.base100)
                .Rounded(5)
                .BorderColor(Themes.base200)
                .BorderWidth(1);
        }

        private static ElementBuilder TabsContainer(string id, Tab[] tabs)
        {
            using (Gui.Row("Tabs").Height(28).Left(5).Enter())
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
                    .Width(24).Height(24)
                    .BackgroundColor(Themes.base100)
                    .Text("+", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Alignment(TextAlignment.MiddleCenter)
                    .Rounded(5)
                    .Margin(2)
                    .Hovered
                        .BackgroundColor(Themes.base300)
                    .End();

                Gui.Box("Spacer"); // automatically grows

                Gui.Box("Plus Tab")
                    .Width(24).Height(24)
                    .BackgroundColor(Themes.base100)
                    .Text(Icons.Grip, Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Alignment(TextAlignment.MiddleCenter)
                    .Rounded(5)
                    .Margin(2)
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
                    .Hovered
                        .BackgroundColor(Themes.base300)
                    .End()
                    .Rounded(5)
                    .Alignment(TextAlignment.MiddleCenter);

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

                Gui.Box("Spacer");

                Gui.Box("tab 3")
                   .Width(64).Height(28)
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