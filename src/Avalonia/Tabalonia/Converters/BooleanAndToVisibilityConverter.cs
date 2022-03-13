using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace Tabalonia;

public class BooleanAndToVisibilityConverter : BaseMultiValueConverter
{
    public override object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null)
            return false;

        return values.Select(GetBool).All(b => b);
    }

    private static bool GetBool(object value)
    {
        if (value is bool)
        {
            return (bool) value;
        }

        return false;
    }
}