﻿using Avalonia.Interactivity;

namespace Tabalonia.Dockablz;

public delegate void FloatRequestedEventHandler(object sender, FloatRequestedEventArgs e);

public class FloatRequestedEventArgs : DragablzItemEventArgs
{
    public FloatRequestedEventArgs(RoutedEvent routedEvent, IInteractive source, DragablzItem dragablzItem) 
        : base(routedEvent, source, dragablzItem)
    { }

    public FloatRequestedEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem) 
        : base(routedEvent, dragablzItem)
    { }
}