using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Tabalonia;

public abstract class BaseMultiValueConverter : MarkupExtension, IMultiValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
    
    public abstract object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture);
}