using Avalonia.Controls;

namespace Tabalonia.Demo;

public class BasicExampleInterTabClient : IInterTabClient
{
    public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
    {
        var view = new BasicExampleTemplateWindow();
        var model = new BasicExampleTemplateModel(interTabClient, partition);
        view.DataContext = model;
        return new NewTabHost<Window>(view, view.TabablzControl);
    }

    public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
    {
        return TabEmptiedResponse.CloseWindowOrLayoutBranch;
    }
}