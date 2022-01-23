using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Tabalonia;

public class EqualityToVisibilityConverter : BaseValueConverter
{
    //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //{
    //    return Equals(value, parameter)
    //        ? Visibility.Visible
    //        : Visibility.Collapsed;
    //}

    //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //{
    //    return Binding.DoNothing;
    //}
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Equals(value, parameter);
    }
}