using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tabalonia.Demo;

public partial class BasicExampleTemplateWindow : Window
{
    public BasicExampleTemplateWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}