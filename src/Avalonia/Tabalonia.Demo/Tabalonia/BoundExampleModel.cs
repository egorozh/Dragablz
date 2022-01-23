using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Dragablz;

namespace Tabalonia.Demo;

public class BoundExampleModel
{
    public BoundExampleModel()
    {
        Items = new ObservableCollection<HeaderedItemViewModel>();
    }

    public BoundExampleModel(params HeaderedItemViewModel[] items)
    {
        Items = new ObservableCollection<HeaderedItemViewModel>(items);
    }

    public ObservableCollection<HeaderedItemViewModel> Items { get; }

    public static Guid TabPartition => new Guid("2AE89D18-F236-4D20-9605-6C03319038E6");

    public IInterTabClient InterTabClient { get; } = new BoundExampleInterTabClient();

    public ObservableCollection<HeaderedItemViewModel> ToolItems { get; } = new ObservableCollection<HeaderedItemViewModel>();

    public ItemActionCallback ClosingTabItemHandler => ClosingTabItemHandlerImpl;

    /// <summary>
    /// Callback to handle tab closing.
    /// </summary>        
    private static void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
    {
        //in here you can dispose stuff or cancel the close

        //here's your view model:
        var viewModel = args.DragablzItem.DataContext as HeaderedItemViewModel;
        Debug.Assert(viewModel != null);

        //here's how you can cancel stuff:
        //args.Cancel(); 
    }

    public ClosingFloatingItemCallback ClosingFloatingItemHandler => ClosingFloatingItemHandlerImpl;

    /// <summary>
    /// Callback to handle floating toolbar/MDI window closing.
    /// </summary>        
    private static void ClosingFloatingItemHandlerImpl(ItemActionCallbackArgs<Layout> args)
    {
        //in here you can dispose stuff or cancel the close

        //here's your view model: 
        var disposable = args.DragablzItem.DataContext as IDisposable;
        if (disposable != null)
            disposable.Dispose();

        //here's how you can cancel stuff:
        //args.Cancel(); 
    }
}