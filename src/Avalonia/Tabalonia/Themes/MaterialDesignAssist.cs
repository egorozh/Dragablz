using Avalonia;
using Avalonia.Media;

namespace Tabalonia.Themes;

/// <summary>
/// Helper propries for configuring the material design style.
/// </summary>
public class MaterialDesignAssist
{
    /// <summary>
    /// Framework use only.
    /// </summary>
    public static readonly AttachedProperty<Brush> IndicatorBrushProperty =
        AvaloniaProperty.RegisterAttached<MaterialDesignAssist, IAvaloniaObject, Brush>("IndicatorBrush");

    /// <summary>
    /// The indicator (underline) brush.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetIndicatorBrush(IAvaloniaObject element, Brush value) => element.SetValue(IndicatorBrushProperty, value);

    /// <summary>
    /// The indicator (underline) brush.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Brush GetIndicatorBrush(IAvaloniaObject element) => element.GetValue(IndicatorBrushProperty);
}