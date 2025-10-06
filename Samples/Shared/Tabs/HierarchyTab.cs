using Prowl.PaperUI;

namespace Shared.Tabs
{
    public class HierarchyTab : Tab
    {
        public HierarchyTab(Paper gui) : base(gui)
        {
            title = "Hierarchy";
            id = "hierarchy";
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

            HierarchyItem(rootItem);
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

        private void HierarchyItem(Item item)
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
    }
}