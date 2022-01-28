using System;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Tabalonia;

public class DragablzDragCompletedEventArgs : RoutedEventArgs
{
    public DragablzDragCompletedEventArgs(DragablzItem dragablzItem, VectorEventArgs dragCompletedEventArgs)
    {
        DragablzItem = dragablzItem ?? throw new ArgumentNullException(nameof(dragablzItem));
        DragCompletedEventArgs = dragCompletedEventArgs ?? throw new ArgumentNullException(nameof(dragCompletedEventArgs));
    }

    public DragablzDragCompletedEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem, VectorEventArgs dragCompletedEventArgs)
        : base(routedEvent)
    {
        DragablzItem = dragablzItem ?? throw new ArgumentNullException(nameof(dragablzItem));            
        DragCompletedEventArgs = dragCompletedEventArgs ?? throw new ArgumentNullException(nameof(dragCompletedEventArgs));
    }

    public DragablzDragCompletedEventArgs(RoutedEvent routedEvent, IInteractive source, DragablzItem dragablzItem, VectorEventArgs dragCompletedEventArgs)
        : base(routedEvent, source)
    {
        DragablzItem = dragablzItem ?? throw new ArgumentNullException(nameof(dragablzItem));
        DragCompletedEventArgs = dragCompletedEventArgs ?? throw new ArgumentNullException(nameof(dragCompletedEventArgs));
    }

    public DragablzItem DragablzItem { get; }

    public VectorEventArgs DragCompletedEventArgs { get; }
}