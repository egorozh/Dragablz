using Avalonia.Controls;

namespace Tabalonia.Demo;

public class BoundExampleInterTabClient : IInterTabClient
{
    public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
    {
        var view = new BoundExampleWindow();
        var model = new BoundExampleModel();            
        view.DataContext = model;            
        //return new NewTabHost<Window>(view, view.InitialTabablzControl);            
        return new NewTabHost<Window>(view, null);            
    }

    public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
    {
        return TabEmptiedResponse.CloseWindowOrLayoutBranch;
    }
}