using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Layout;
using Dragablz;

namespace Tabalonia;

/// <summary>
/// A linear position monitor simplifies the montoring of the order of items, where they are laid out
/// horizontally or vertically (typically via a <see cref="StackOrganiser"/>.
/// </summary>
public abstract class StackPositionMonitor : PositionMonitor
{
    private readonly Func<DragablzItem, double> _getLocation;

    protected StackPositionMonitor(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.Horizontal:
                _getLocation = item => item.X;
                break;
            case Orientation.Vertical:
                _getLocation = item => item.Y;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(orientation));
        }
    }

    public event EventHandler<OrderChangedEventArgs> OrderChanged;

    internal virtual void OnOrderChanged(OrderChangedEventArgs e)
    {
        var handler = OrderChanged;
        handler?.Invoke(this, e);
    }

    internal IEnumerable<DragablzItem> Sort(IEnumerable<DragablzItem> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        return items.OrderBy(i => _getLocation(i));
    }
}