using Prowl.PaperUI;

namespace Shared.Tabs
{
    public class ExampleTab : Tab
    {
        public ExampleTab(Paper gui) : base(gui)
        {
            title = "Example ( minimal example of a tab )";
            id = "example";
            width = 70;
        }
    }
}