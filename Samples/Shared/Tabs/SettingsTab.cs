using Prowl.PaperUI;

namespace Shared.Tabs
{
    public class SettingsTab : Tab
    {
        public SettingsTab(Paper gui) : base(gui)
        {
            title = "Settings";
            id = "settings";
            width = 70;
        }
    }
}