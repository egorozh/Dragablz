using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace Tabalonia.Core;

internal static class Extensions
{
    public static T Find<T>(this TemplateAppliedEventArgs e, string elementName) where T : class
    {
        var element = e.NameScope.Find<T>(elementName);

        if (element == null)
            throw new ElementNotFoundOnStyleException(elementName);

        return element;
    }

    public static IEnumerable<TContainer> Containers<TContainer>(this ItemsControl itemsControl)
        where TContainer : class
    {
        var itemGen = itemsControl.ItemContainerGenerator;

        foreach (var container in itemGen.Containers)
        {
            if (container is TContainer c)
                yield return c;
        }
    }

    public static IEnumerable<TObject> Except<TObject>(this IEnumerable<TObject> first, params TObject[] second)
    {
        return first.Except((IEnumerable<TObject>) second);
    }

    public static IEnumerable<object> LogicalTreeDepthFirstTraversal(this IAvaloniaObject? node)
    {
        if (node == null) yield break;
        yield return node;

        //foreach (var child in LogicalTreeHelper.GetChildren(node).OfType<IAvaloniaObject>()
        //             .SelectMany(depObj => depObj.LogicalTreeDepthFirstTraversal()))
        //    yield return child;
    }

    public static IEnumerable<object> VisualTreeDepthFirstTraversal(this IVisual? node)
    {
        if (node == null) 
            yield break;

        yield return node;

        foreach (var child in node.VisualChildren)
        {
            foreach (var d in child.VisualTreeDepthFirstTraversal())
            {
                yield return d;
            }
        }
    }

    /// <summary>
    /// Yields the visual ancestory (including the starting point).
    /// </summary>
    /// <param name="dependencyObject"></param>
    /// <returns></returns>
    public static IEnumerable<IVisual> VisualTreeAncestory(this IVisual? dependencyObject)
    {
        if (dependencyObject == null) 
            throw new ArgumentNullException(nameof(dependencyObject));

        while (dependencyObject != null)
        {
            yield return dependencyObject;
            dependencyObject = dependencyObject.GetVisualParent();
        }
    }

    /// <summary>
    /// Yields the logical ancestory (including the starting point).
    /// </summary>
    /// <param name="dependencyObject"></param>
    /// <returns></returns>
    public static IEnumerable<ILogical?> LogicalTreeAncestory(this ILogical? dependencyObject)
    {
        if (dependencyObject == null)
            throw new ArgumentNullException(nameof(dependencyObject));

        while (dependencyObject != null)
        {
            yield return dependencyObject;

            dependencyObject = dependencyObject.LogicalParent;
        }
    }

    /// <summary>
    /// Returns the actual Left of the Window independently from the WindowState
    /// </summary>
    /// <param name="window"></param>
    /// <returns></returns>
    public static double GetActualLeft(this Window window)
    {
        if (window.WindowState == WindowState.Maximized)
        {
            var leftField = typeof(Window).GetField("_actualLeft",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return leftField?.GetValue(window) as double? ?? 0;
        }

        return window.Position.X;
    }

    /// <summary>
    /// Returns the actual Top of the Window independently from the WindowState
    /// </summary>
    /// <param name="window"></param>
    /// <returns></returns>
    public static double GetActualTop(this Window window)
    {
        if (window.WindowState == WindowState.Maximized)
        {
            var topField = typeof(Window).GetField("_actualTop",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return topField?.GetValue(window) as double? ?? 0;
        }

        return window.Position.Y;
    }
}