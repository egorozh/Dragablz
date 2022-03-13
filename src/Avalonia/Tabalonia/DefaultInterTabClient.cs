using Avalonia.Controls;
using System;

namespace Tabalonia;

public class DefaultInterTabClient : IInterTabClient
{        
    public virtual INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        
        //var sourceWindow = Window.GetWindow(source);
        //if (sourceWindow == null) throw new ApplicationException("Unable to ascertain source window.");
        //var newWindow = (Window)Activator.CreateInstance(sourceWindow.GetType());

        //newWindow.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.DataBind);

        //var newTabablzControl = newWindow.LogicalTreeDepthFirstTraversal().OfType<TabablzControl>().FirstOrDefault();
        //if (newTabablzControl == null) throw new ApplicationException("Unable to ascertain tab control.");

        //if (newTabablzControl.ItemsSource == null)
        //    newTabablzControl.Items.Clear();

        //return new NewTabHost<Window>(newWindow, newTabablzControl);            
        return new NewTabHost<Window>(null, null);
    }

    public virtual TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
    {
        return TabEmptiedResponse.CloseWindowOrLayoutBranch;
    }
}