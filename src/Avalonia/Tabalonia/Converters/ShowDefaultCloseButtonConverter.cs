using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Tabalonia;

public class ShowDefaultCloseButtonConverter : BaseMultiValueConverter
{
    /// <summary>
    /// [0] is owning tabcontrol ShowDefaultCloseButton value.
    /// [1] is owning tabcontrol FixedHeaderCount value.
    /// [2] is item LogicalIndex
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    //public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //{
    //    return ((values[0] == DependencyProperty.UnsetValue ? false : (bool)values[0]) && 
    //            (values[2] == DependencyProperty.UnsetValue ? 0 : (int)values[2]) >= 
    //            (values[1] == DependencyProperty.UnsetValue ? 0 : (int)values[1])) ? Visibility.Visible : Visibility.Collapsed;
    //}

    //public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //{
    //    return null;            
    //}

    public override object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return ((values[0] == AvaloniaProperty.UnsetValue ? false : (bool)values[0]) &&
                (values[2] == AvaloniaProperty.UnsetValue ? 0 : (int)values[2]) >= 
                (values[1] == AvaloniaProperty.UnsetValue ? 0 : (int)values[1]));
    }
}