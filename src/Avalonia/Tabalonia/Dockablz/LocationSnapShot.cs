﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Tabalonia.Dockablz;

/// <summary>
/// Initially needed to restore MDI dragablz items styles after a max then restore,
/// as the trigger which binds the item width to the canvas width sets the  Width back to the default
/// (e.g double.NaN) when the trigger is unset.  so we need to re-apply sizes manually
/// </summary>
internal class LocationSnapShot
{
    private readonly double _width;
    private readonly double _height;

    public static LocationSnapShot Take(ILayoutable frameworkElement)
    {
        if (frameworkElement == null) throw new ArgumentNullException(nameof(frameworkElement));
            
        return new LocationSnapShot(frameworkElement.Width, frameworkElement.Height);
    }

    private LocationSnapShot(double width, double height)
    {
        _width = width;
        _height = height;
    }

    public void Apply(Control frameworkElement)
    {
        if (frameworkElement == null) throw new ArgumentNullException(nameof(frameworkElement));
            
        //frameworkElement.SetCurrentValue(FrameworkElement.WidthProperty, _width);
        //frameworkElement.SetCurrentValue(FrameworkElement.HeightProperty, _height);
    }
}