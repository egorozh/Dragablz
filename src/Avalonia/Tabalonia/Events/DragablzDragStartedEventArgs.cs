using System;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Tabalonia;

public class DragablzDragStartedEventArgs : DragablzItemEventArgs
{
    public DragablzDragStartedEventArgs(DragablzItem dragablzItem, VectorEventArgs dragStartedEventArgs)
        : base(dragablzItem)
    {
        DragStartedEventArgs = dragStartedEventArgs ?? throw new ArgumentNullException(nameof(dragStartedEventArgs));
    }

    public DragablzDragStartedEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem, VectorEventArgs dragStartedEventArgs)
        : base(routedEvent, dragablzItem)
    {
        DragStartedEventArgs = dragStartedEventArgs;
    }

    public DragablzDragStartedEventArgs(RoutedEvent routedEvent, IInteractive source, DragablzItem dragablzItem, VectorEventArgs dragStartedEventArgs)
        : base(routedEvent, source, dragablzItem)
    {
        DragStartedEventArgs = dragStartedEventArgs;
    }

    public VectorEventArgs DragStartedEventArgs { get; }
}