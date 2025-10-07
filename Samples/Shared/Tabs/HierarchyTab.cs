using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;

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
            using (Gui.Box("Search Box")
                .OnDragEnd((e) =>
                {
                    Console.WriteLine("drag end", e);
                })
                .OnDragStart((e) =>
                {
                    // e.DataTransfer.setData("tabId", id);
                    // e.DataTransfer.setImage(imageOfThingBeingDragged); // shows an image that moves with your cursor as long as you are dragging
                    Console.WriteLine("drag start", e);
                })
                .OnDragging((e) =>
                {
                    Console.WriteLine("dragging", e);
                })
                // .OnDrop((e) =>
                // {
                //     var data = e.DataTransfer.getData("tabId");
                // })
                .Height(28).Margin(5).Top(8).Bottom(8).Rounded(5)
                .BackgroundColor(Themes.base300).Enter())
            {
                Gui.Box("Search").Text("Filter...", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Left(8)
                    .Alignment(TextAlignment.MiddleLeft);
            }

            using (Gui.Box("Hierarchy container").SetScroll(Scroll.ScrollY).Enter())
            {
                HierarchyItem(rootItem, 0);
                Gui.Box("Spacer"); // take up any additional
            }
        }

        public class Item
        {
            public string id;
            public string title;
            public Item[] children;
            public bool expanded;
            public bool selected;
            public Action<Item> onClick; // Open inspector etc
        }

        public static string selectedItemId = "";

        private void HierarchyItem(Item item, int depth)
        {
            var isSelected = selectedItemId == item.id;

            using (Gui.Column("Hierarchy item " + item.id, depth).MinHeight(item.expanded ? UnitValue.Auto : 28).Left(depth * 8).Enter())
            {
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
                    .OnDragging((e) =>
                    {
                        var source = e.Source;
                        Console.WriteLine("dragged over", item.title);
                    })
                    .Enter())
                {
                    if (isSelected)
                    {
                        Gui.Box("indicator")
                            .PositionType(PositionType.SelfDirected)
                            .Height(28).Width(3).Rounded(1)
                            .BackgroundColor(Themes.primary);
                    }

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

                }

                if (item.expanded && item.children != null)
                {
                    foreach (var child in item.children)
                    {
                        HierarchyItem(child, depth + 1);
                    }
                }
            }
        }

        public static Item rootItem = new Item
        {
            id = "root object",
            title = "Game World",
            expanded = true,
            selected = false,
            onClick = (item) => Console.WriteLine($"Clicked {item.title}"),
            children = new[] {
                new Item {
                    id = "2",
                    title = "Player",
                    expanded = true,
                    children = new[] {
                        new Item { id = "2.1", title = "Inventory"},
                        new Item { id = "2.2", title = "Equipment"},
                        new Item { id = "2.3", title = "Stats" },
                        new Item { id = "2.4", title = "Skills" }
                    }
                },
                new Item {
                    id = "3",
                    title = "Environment",
                    expanded = false,
                    children = new[] {
                        new Item {
                            id = "3.1",
                            title = "Terrain",
                            children = new[] {
                                new Item { id = "3.1.1", title = "Mountains" },
                                new Item { id = "3.1.2", title = "Forest" },
                                new Item { id = "3.1.3", title = "Desert" }
                            }
                        },
                        new Item {
                            id = "3.2",
                            title = "Weather",
                            children = new[] {
                                new Item { id = "3.2.1", title = "Rain System" },
                                new Item { id = "3.2.2", title = "Wind System" }
                            }
                        }
                    }
                },
                new Item {
                    id = "4",
                    title = "NPCs",
                    expanded = false,
                    children = new[] {
                        new Item {
                            id = "4.1",
                            title = "Friendly",
                            children = new[] {
                                new Item { id = "4.1.1", title = "Merchants" },
                                new Item { id = "4.1.2", title = "Quest Givers" },
                                new Item { id = "4.1.3", title = "Villagers" }
                            }
                        },
                        new Item {
                            id = "4.2",
                            title = "Hostile",
                            children = new[] {
                                new Item { id = "4.2.1", title = "Goblins" },
                                new Item { id = "4.2.2", title = "Dragons" },
                                new Item { id = "4.2.3", title = "Bandits" }
                            }
                        }
                    }
                },
                new Item {
                    id = "5",
                    title = "UI Elements",
                    expanded = false,
                    children = new[] {
                        new Item { id = "5.1", title = "Main Menu" },
                        new Item { id = "5.2", title = "HUD" },
                        new Item { id = "5.3", title = "Inventory UI" },
                        new Item { id = "5.4", title = "Quest Log" }
                    }
                },
                new Item {
                    id = "6",
                    title = "Audio",
                    expanded = false,
                    children = new[] {
                        new Item { id = "6.1", title = "Music" },
                        new Item { id = "6.2", title = "Sound Effects" },
                        new Item { id = "6.3", title = "Ambient" }
                    }
                }
            }
        };
    }
}