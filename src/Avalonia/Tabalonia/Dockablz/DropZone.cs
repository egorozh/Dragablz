using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Dragablz.Dockablz;

namespace Tabalonia.Dockablz;

public class DropZone : Control, IStyleable
{
    #region IStyleable

    Type IStyleable.StyleKey => typeof(DropZone);

    #endregion

    #region Avalonia Properties
    
    public static readonly StyledProperty<DropZoneLocation> LocationProperty =
        AvaloniaProperty.Register<Branch, DropZoneLocation>(nameof(Location));


    public static readonly DirectProperty<DropZone, bool> IsOfferedProperty =
        AvaloniaProperty.RegisterDirect<DropZone, bool>(nameof(IsOffered), 
            o => o.IsOffered, (o, v) => o.IsOffered = v);
    

    private bool _isOffered;

    #endregion

    #region Public Properties

    public DropZoneLocation Location
    {
        get => GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }


    public bool IsOffered
    {
        get => GetValue(IsOfferedProperty);
        internal set => SetAndRaise(IsOfferedProperty, ref _isOffered, value);
    }

    #endregion
}