using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Dragablz.Dockablz;

namespace Tabalonia.Dockablz;

internal class Tiler
{
    public static void Tile(IEnumerable<DragablzItem> dragablzItems, Size bounds)
    {
        if (dragablzItems == null) throw new ArgumentNullException(nameof(dragablzItems));            

        var items = new Queue<DragablzItem>(dragablzItems.OrderBy(di => di.ZIndex));

        var cellCountPerColumn = TilerCalculator.GetCellCountPerColumn(items.Count());
        var x = 0d;
        var cellWidth = bounds.Width / cellCountPerColumn.Length;
        foreach (var cellCount in cellCountPerColumn)
        {
            var y = 0d;
            var cellHeight = bounds.Height / cellCount;
            for (var cell = 0; cell < cellCount; cell++)
            {
                var item = items.Dequeue();
                Layout.SetFloatingItemState(item, WindowState.Normal);
                item.SetValue(DragablzItem.XProperty, x);
                item.SetValue(DragablzItem.YProperty, y);
                item.SetValue(Layoutable.WidthProperty, cellWidth);
                item.SetValue(Layoutable.HeightProperty, cellHeight);

                y += cellHeight;
            }

            x += cellWidth;
        }
    }

    public static void TileHorizontally(IEnumerable<DragablzItem> dragablzItems, Size bounds)
    {
        if (dragablzItems == null) throw new ArgumentNullException(nameof(dragablzItems));

        var items = dragablzItems.ToList();

        var x = 0.0;
        var width = bounds.Width/items.Count;
        foreach (var dragablzItem in items)
        {
            Layout.SetFloatingItemState(dragablzItem, WindowState.Normal);
            dragablzItem.SetValue(DragablzItem.XProperty, x);
            dragablzItem.SetValue(DragablzItem.YProperty, 0d);
            x += width;
            dragablzItem.SetValue(Layoutable.WidthProperty, width);
            dragablzItem.SetValue(Layoutable.HeightProperty, bounds.Height);
        }
    }

    public static void TileVertically(IEnumerable<DragablzItem> dragablzItems, Size bounds)
    {
        if (dragablzItems == null) throw new ArgumentNullException(nameof(dragablzItems));

        var items = dragablzItems.ToList();

        var y = 0.0;
        var height = bounds.Height / items.Count;
        foreach (var dragablzItem in items)
        {
            Layout.SetFloatingItemState(dragablzItem, WindowState.Normal);
            dragablzItem.SetValue(DragablzItem.YProperty, y);
            dragablzItem.SetValue(DragablzItem.XProperty, 0d);
            y += height;
            dragablzItem.SetValue(Layoutable.HeightProperty, height);
            dragablzItem.SetValue(Layoutable.WidthProperty, bounds.Width);
        }
    }

}