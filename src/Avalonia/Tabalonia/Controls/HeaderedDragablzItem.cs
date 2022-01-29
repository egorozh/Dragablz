using Avalonia;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Styling;
using System;

namespace Tabalonia;

public class HeaderedDragablzItem : DragablzItem, IStyleable
{
    #region IStyleable

    Type IStyleable.StyleKey => typeof(HeaderedDragablzItem);

    #endregion

    #region Avalonia Properties

    public static readonly StyledProperty<object> HeaderContentProperty =
        AvaloniaProperty.Register<HeaderedDragablzItem, object>(nameof(HeaderContent));

    public static readonly StyledProperty<string> HeaderContentStringFormatProperty =
        AvaloniaProperty.Register<HeaderedDragablzItem, string>(nameof(HeaderContentStringFormat));

    public static readonly StyledProperty<DataTemplate> HeaderContentTemplateProperty =
        AvaloniaProperty.Register<HeaderedDragablzItem, DataTemplate>(nameof(HeaderContentTemplate));

    //public static readonly StyledProperty<DataTemplateSelector> HeaderContentTemplateSelectorProperty =
    //    AvaloniaProperty.Register<HeaderedDragablzItem, DataTemplateSelector>(nameof(HeaderContentTemplateSelector));

    #endregion

    #region Public Properties

    public object HeaderContent
    {
        get => GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    public string HeaderContentStringFormat
    {
        get => GetValue(HeaderContentStringFormatProperty);
        set => SetValue(HeaderContentStringFormatProperty, value);
    }

    public DataTemplate HeaderContentTemplate
    {
        get => GetValue(HeaderContentTemplateProperty);
        set => SetValue(HeaderContentTemplateProperty, value);
    }

    //public DataTemplateSelector HeaderContentTemplateSelector
    //{
    //    get => (DataTemplateSelector) GetValue(HeaderContentTemplateSelectorProperty);
    //    set => SetValue(HeaderContentTemplateSelectorProperty, value);
    //}

    #endregion
}