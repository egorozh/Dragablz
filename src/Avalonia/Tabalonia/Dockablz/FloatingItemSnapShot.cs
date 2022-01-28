using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Tabalonia.Dockablz;

/// <summary>
/// experimentational.  might have to puish this back to mvvm only
/// </summary>    
internal class FloatingItemSnapShot
{
    public FloatingItemSnapShot(object content, Rect location, int zIndex, WindowState state)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Location = location;
        ZIndex = zIndex;
        State = state;
    }

    public static FloatingItemSnapShot Take(DragablzItem dragablzItem)
    {
        if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));

        return new FloatingItemSnapShot(
            dragablzItem.Content, 
            new Rect(dragablzItem.X, dragablzItem.Y, dragablzItem.Bounds.Width, dragablzItem.Bounds.Height),
            dragablzItem.GetValue(Visual.ZIndexProperty),
            Layout.GetFloatingItemState(dragablzItem));
    }

    public void Apply(DragablzItem dragablzItem)
    {
        if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));

        dragablzItem.SetValue(DragablzItem.XProperty, Location.Left);
        dragablzItem.SetValue(DragablzItem.YProperty, Location.Top);
        dragablzItem.SetValue(Layoutable.WidthProperty, Location.Width);
        dragablzItem.SetValue(Layoutable.HeightProperty, Location.Height);
        Layout.SetFloatingItemState(dragablzItem, State);
        dragablzItem.SetValue(Visual.ZIndexProperty, ZIndex);
    }

    public object Content { get; }

    public Rect Location { get; }

    public int ZIndex { get; }

    public WindowState State { get; }
}