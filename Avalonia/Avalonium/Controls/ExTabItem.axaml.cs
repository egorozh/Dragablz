using Avalonia;
using Avalonia.Controls;

namespace Avalonium;

public class ExTabItem : TabItem
{
    private readonly IDisposable _sizeChanged;

    public ExTabItem()
    {
        _sizeChanged = Visual.BoundsProperty.Changed.Subscribe(BoundChangedHandler);
    }

    public void InitPosition()
    {
        var offset = 0.0;

        if (Parent is TabsControl tabsControl) 
            offset = tabsControl.AdjacentHeaderItemOffset;

        offset = TabIndex == 0 ? 0.0 : offset;

        Canvas.SetLeft(this, TabIndex * Bounds.Width - offset * TabIndex);
        Canvas.SetTop(this, 0);
    }

    private static void BoundChangedHandler(AvaloniaPropertyChangedEventArgs<Rect> e)
    {
        if (e.Sender is ExTabItem tabItem)
        {
            tabItem.InitPosition();
        }
    }
}