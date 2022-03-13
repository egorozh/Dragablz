using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;

namespace Tabalonia;

public class BrushToRadialGradientBrushConverter : BaseValueConverter
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush solidColorBrush) 
            return null;

        var stops = new GradientStops
        {
            new GradientStop(solidColorBrush.Color, 0),
            new GradientStop(Colors.Transparent, 1)
        };

        return new RadialGradientBrush()
        {
            GradientStops = stops,
            //Center = new Point(.5, .5),
            //GradientOrigin = new Point(.5, .5),
            //RadiusX = .5,
            //RadiusY = .5,
            Opacity = .39
        };
    }
}