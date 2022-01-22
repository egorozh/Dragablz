using Avalonia.Controls;

namespace Tabalonia;

public class DragablzIcon : Control
{
    static DragablzIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DragablzIcon), new FrameworkPropertyMetadata(typeof(DragablzIcon)));
    }
}