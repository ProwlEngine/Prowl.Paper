using Prowl.PaperUI;

namespace Shared.Tabs
{
    public class InspectorTab : Tab
    {
        public InspectorTab(Paper gui) : base(gui)
        {
            title = "Inspector";
            id = "inspector";
            width = 70;
        }
    }
}