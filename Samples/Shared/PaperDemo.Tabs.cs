using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;

using Shared.Tabs;

namespace Shared
{
    public abstract class Tab
    {
        public string id;
        public string title;
        public double width;
        protected Paper Gui;

        public Tab(Paper gui)
        {
            this.Gui = gui;
        }
        
        public virtual void Body()
        {
            Gui.Box("Tab Title").Text("[" + this.title + " Tab]", Fonts.arial)
                .TextColor(Themes.baseContent)
                .Left(8)
                .Alignment(TextAlignment.MiddleCenter);
        }
    }

    public class TabsManager
    {
        public string ActiveTabId = "";
        public Dictionary<string, Tab> Entries = new Dictionary<string, Tab>();
        internal Action<string, Tab> _onSelected;

        private Paper Gui;

        public TabsManager(Paper gui)
        {
            this.Gui = gui;

            AddTab("assets", new AssetsTab(gui));
            AddTab("files", new FilesTab(gui));
            AddTab("game", new GameTab(gui));
            AddTab("hierarchy", new HierarchyTab(gui));
            AddTab("inspector", new InspectorTab(gui));
            AddTab("settings", new SettingsTab(gui));
        }

        public void AddTab(string id, Tab tab)
        {
            Entries.Add(id, tab);
            ActiveTabId = id;
        }

        public void OnSelected(Action<string, Tab> callback)
        {
            _onSelected = callback;
        }

        public void DrawGroup(string[] tabs)
        {
            var group = Entries.Values.Where((tab, i) => tabs.Contains(tab.id)).ToArray();

            // Get stored active tab ID or use first tab as default
            var storageKey = string.Join("_", tabs) + "_activeTabId"; // auto invalidate the key if the group changes
            var localGroupTabId = Gui.GetElementStorage(storageKey, tabs[0]);

            var currentElement = Gui.CurrentParent;

            TabsDisplay("Tabs Container", group, storageKey, currentElement, localGroupTabId);

            // If active tab exists in current group, draw its body
            if (Entries.TryGetValue(localGroupTabId, out Tab activeTab))
            {
                using (Gui.Column("Body").BackgroundColor(Themes.base200).Enter())
                {
                    activeTab.Body();
                }
            }
            // Otherwise fallback to first tab
            else if (group.Length > 0)
            {
                using (Gui.Column("Body").BackgroundColor(Themes.base200).Enter())
                {
                    group[0].Body();
                }
                Gui.SetElementStorage(storageKey, group[0].id);
            }

        }

        private void TabsDisplay(string id, Tab[] tabs, string tabIdStorageKey, ElementHandle elementWithTabIdStorage, string localGroupId)
        {
            using (Gui.Row("Tabs " + id).Height(28).Enter())
            {
                foreach (var tab in tabs)
                {
                    if (tab.id == localGroupId)
                    {
                        using (Gui.Box("Active Tab" + tab.id).Width(tab.width).Height(28).Left(5)
                            .OnClick((_) =>
                            {
                                Console.WriteLine("clicked tab " + tab.id);
                                ActiveTabId = tab.id;
                                Gui.SetElementStorage(elementWithTabIdStorage, tabIdStorageKey, tab.id);
                            })
                            .Enter())
                        {
                            if (tab.id == ActiveTabId)
                            {
                                Gui.Box("Highlight").Layer(Layer.Overlay).PositionType(PositionType.SelfDirected).RoundedTop(3).BackgroundColor(Themes.primary).Height(3);
                            }

                            Gui.Box("tab 1")
                                .RoundedTop(3)
                                .BackgroundColor(Themes.base200)
                                .Text(tab.title, Fonts.arial)
                                .TextColor(Themes.baseContent)
                                .Alignment(TextAlignment.MiddleCenter);
                        }
                    }
                    else
                    {
                        Gui.Box("Inactive Tab" + tab.id)
                            .OnClick((_) =>
                            {
                                Gui.SetElementStorage(elementWithTabIdStorage, tabIdStorageKey, tab.id);
                                Console.WriteLine("clicked tab " + tab.id);
                                ActiveTabId = tab.id;
                            })
                            .RoundedTop(3)
                            .Left(5)
                            .Width(tab.width).Height(28)
                            .BackgroundColor(Themes.base100)
                            .Text(tab.title, Fonts.arial)
                            .TextColor(Themes.baseContent)
                            .Alignment(TextAlignment.MiddleCenter)
                            .Hovered
                                .BackgroundColor(Themes.base300)
                            .End();
                    }
                }

                Gui.Box("Plus Tab")
                    .Width(20).Height(20)
                    .BackgroundColor(Themes.base100)
                    .Text("+", Fonts.arial)
                    .TextColor(Themes.baseContent)
                    .Alignment(TextAlignment.MiddleCenter)
                    .Rounded(5)
                    .Margin(4)
                    .Hovered
                        .BackgroundColor(Themes.base300)
                    .End();

                Gui.Box("Spacer"); // automatically grows

                Gui.Box("Options")
                    .Width(20).Height(20)
                    .BackgroundColor(Themes.base100)
                    .FontSize(12)
                    .Text(Icons.Grip, Fonts.arial).TextColor(Themes.baseContent)
                    .Alignment(TextAlignment.MiddleCenter)
                    .Rounded(5)
                    .Margin(4)
                    .Hovered
                        .BackgroundColor(Themes.base300)
                    .End();
            }
        }
    }
}