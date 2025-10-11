using Prowl.PaperUI;

namespace Shared.Tabs
{
    public class FilesTab : Tab
    {
        public FilesTab(Paper gui) : base(gui)
        {
            title = "Files";
            id = "files";
            width = 52;
        }

        private string selectedFolderId;

        public override void Draw()
        {
            using (Gui.Box("Search Box").Height(28).Margin(5).Rounded(5).BackgroundColor(Themes.base300).Enter())
            {
                Gui.Box("Search").Text("Filter...", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Left(8)
                    .Alignment(TextAlignment.MiddleLeft);
            }

            using (Gui.Column("Folders view").SetScroll(Scroll.ScrollY).Enter())
            {
                DrawFolderItem("Folders", true, new[] {
                    ("Models", new[] {
                    "Character.fbx",
                    "Tree.fbx",
                    "House.fbx"
                    }),
                    ("Textures", new[] {
                    "Grass.png",
                    "Sky.png"
                    }),
                    ("Scripts", new[] {
                    "Player.cs",
                    "Enemy.cs"
                    })
                });
            }
        }

        private void DrawFolderItem(string name, bool expanded = false, (string name, string[] children)[] subFolders = null)
        {
            var isSelected = selectedFolderId == name;

            using (Gui.Row(name).Height(28).Margin(5).Top(0).Bottom(0).Rounded(5)
                .BackgroundColor(isSelected ? Themes.base250 : Themes.base200)
                .Hovered
                    .BackgroundColor(Themes.base250)
                .End()
                .OnClick((_) => selectedFolderId = name)
                .Enter())
            {
                if (subFolders != null)
                {
                    Gui.Box($"toggle{name}").Text(expanded ? Icons.ChevronDown : Icons.ChevronRight, Fonts.arial)
                    .Width(28)
                    .Alignment(TextAlignment.MiddleCenter)
                    .FontSize(8);
                }

                Gui.Box($"icon{name}").Text(Icons.Folder, Fonts.arial)
                    .Width(28)
                    .Alignment(TextAlignment.MiddleCenter)
                    .FontSize(10);

                Gui.Box($"name{name}").Text(name, Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Alignment(TextAlignment.MiddleLeft);
            }

            if (expanded && subFolders != null)
            {
                using (Gui.Box($"Children{name}").Left(20).Enter())
                {
                    foreach (var (folderName, children) in subFolders)
                    {
                        DrawFolderItem(folderName, false, null);
                        if (children != null)
                        {
                            using (Gui.Box($"Files{folderName}").Left(20).Enter())
                            {
                                foreach (var file in children)
                                {
                                    DrawFolderItem(file);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}