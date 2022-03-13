using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tabalonia.Demo;

public partial class QuickStartWindow : Window
{
    public QuickStartWindow()
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