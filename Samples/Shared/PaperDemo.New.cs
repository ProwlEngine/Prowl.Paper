using Prowl.PaperUI;

namespace Shared
{
    public static partial class PaperDemo
    {
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

        public static TabsManager tabsManager;

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

            // create the tabs
            tabsManager = new TabsManager(paper);
        }

        public static void RenderUI()
        {
            using (Gui.Box("App").BackgroundColor(Themes.base100).Enter())
            {
                using (Gui.Column("EditorContainer").Margin(8).Enter())
                {
                    TitleBarUI();

                    using (Gui.Row("3 Columns Editor Layout").RowBetween(6).Enter())
                    {
                        using (Gui.Column("Left Panel").ColBetween(8).Width(250).Enter())
                        {
                            using (WindowContainer("Scene Tree Window").Enter())
                            {
                                // var tabs = new Tab[]
                                // {
                                //     new Tab { id = "hierarchy", title = "Hierarchy", width = 83 },
                                //     new Tab { id = "assets", title = "Assets", width = 65 },
                                // };

                                tabsManager.DrawGroup(["hierarchy", "assets"]);
                            }

                            using (WindowContainer("Files Window Container").Enter())
                            {
                                // var tabs = new Tab[]
                                // {
                                //     new Tab { id = "files", title = "Files", width = 52 },
                                //     new Tab { id = "settings", title = "Settings", width = 70 },
                                // };

                                tabsManager.DrawGroup(["files", "settings"]);
                            }
                        }

                        using (Gui.Column("Center Panel").Enter())
                        {
                            using (WindowContainer("Game and Scene Window").Enter())
                            {
                                tabsManager.DrawGroup(["game"]);
                            }
                        }

                        using (Gui.Column("Right Panel").Width(250).Enter())
                        {
                            using (WindowContainer("Inspector Window").Enter())
                            {
                                tabsManager.DrawGroup(["inspector"]);
                            }
                        }
                    }
                }
            }
        }

        public static void HierarchyItem(Item item)
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
                    Gui.Box("indicator").Height(28).Width(3).Rounded(1).BackgroundColor(Themes.primary);
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

        private static void TitleBarUI()
        {
            using (Gui.Row("Header").Height(28).Bottom(8).Enter())
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

                Gui.Box("tab 4")
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

                Gui.Box("Play / Pause")
                   .Width(64).Height(28)
                   .BackgroundColor(Themes.base200)
                   .Text(Icons.Play + "    " + Icons.Pause, Fonts.arial)
                   .TextColor(Themes.baseContent)
                   .Rounded(5)
                   .Hovered
                       .BackgroundColor(Themes.base250)
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