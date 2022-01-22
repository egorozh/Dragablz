﻿using System;

namespace Tabalonia;

/// <summary>
/// Consumers can provide a position monitor to receive updates regarding the location of an item.    
/// </summary>
/// <remarks>
/// A <see cref="PositionMonitor"/> can be used to listen to  changes 
/// instead of routed events, which can be easier in a MVVM scenario.
/// </remarks>
public class PositionMonitor
{
    /// <summary>
    /// Raised when the X,Y coordinate of a <see cref="DragablzItem"/> changes.
    /// </summary>
    public event EventHandler<LocationChangedEventArgs> LocationChanged;

    internal virtual void OnLocationChanged(LocationChangedEventArgs e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));

        var handler = LocationChanged;
        handler?.Invoke(this, e);
    }

    internal virtual void ItemsChanged() { }
}