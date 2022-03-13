using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Tabalonia.Themes;

public enum SystemCommandType
{
    CloseWindow,
    MaximizeWindow,
    MinimzeWindow,
    RestoreWindow
}

public class SystemCommandIcon : Control, IStyleable
{
    #region IStyleable

    Type IStyleable.StyleKey => typeof(SystemCommandIcon);

    #endregion

    public static readonly StyledProperty<SystemCommandType> SystemCommandTypeProperty =
        AvaloniaProperty.Register<SystemCommandIcon, SystemCommandType>(nameof(SystemCommandType));
    
    public SystemCommandType SystemCommandType
    {
        get => GetValue(SystemCommandTypeProperty);
        set => SetValue(SystemCommandTypeProperty, value);
    }
}