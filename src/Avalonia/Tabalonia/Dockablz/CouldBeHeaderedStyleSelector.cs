using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace Tabalonia.Dockablz;

public class CouldBeHeaderedStyleSelector 
    //: StyleSelector
{
    public Style NonHeaderedStyle { get; set; }

    public Style HeaderedStyle { get; set; }

    //public override Style SelectStyle(object item, IAvaloniaObject container)
    //{
    //    return container is HeaderedDragablzItem or HeaderedContentControl
    //        ? HeaderedStyle
    //        : NonHeaderedStyle;
    //}
}