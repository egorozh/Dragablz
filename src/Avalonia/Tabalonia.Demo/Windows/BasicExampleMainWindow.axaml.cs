using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tabalonia.Demo;

public partial class BasicExampleMainWindow : Window
{
    public BasicExampleMainWindow()
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