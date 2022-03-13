using System;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Tabalonia;

public class DragablzDragDeltaEventArgs : DragablzItemEventArgs
{
    public DragablzDragDeltaEventArgs(DragablzItem dragablzItem, VectorEventArgs dragDeltaEventArgs)
        : base(dragablzItem)
    {
        DragDeltaEventArgs = dragDeltaEventArgs ?? throw new ArgumentNullException(nameof(dragDeltaEventArgs));
    }

    public DragablzDragDeltaEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem, VectorEventArgs dragDeltaEventArgs) 
        : base(routedEvent, dragablzItem)
    {
        DragDeltaEventArgs = dragDeltaEventArgs ?? throw new ArgumentNullException(nameof(dragDeltaEventArgs));
    }

    public DragablzDragDeltaEventArgs(RoutedEvent routedEvent, IInteractive source, DragablzItem dragablzItem, VectorEventArgs dragDeltaEventArgs) 
        : base(routedEvent, source, dragablzItem)
    {
        DragDeltaEventArgs = dragDeltaEventArgs ?? throw new ArgumentNullException(nameof(dragDeltaEventArgs));
    }

    public VectorEventArgs DragDeltaEventArgs { get; }

    public bool Cancel { get; set; }        
}