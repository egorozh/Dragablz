using System;
using Avalonia.Interactivity;

namespace Tabalonia;

public class DragablzItemEventArgs : RoutedEventArgs
{
    public DragablzItemEventArgs(DragablzItem dragablzItem)
    {
        DragablzItem = dragablzItem ?? throw new ArgumentNullException(nameof(dragablzItem));
    }

    public DragablzItemEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem)
        : base(routedEvent)
    {
        DragablzItem = dragablzItem;
    }

    public DragablzItemEventArgs(RoutedEvent routedEvent, IInteractive source, DragablzItem dragablzItem)
        : base(routedEvent, source) 
    {
        DragablzItem = dragablzItem;
    }

    public DragablzItem DragablzItem { get; }
}