using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Tabalonia.Dockablz;

namespace Tabalonia.Demo;

public partial class BoundExampleWindow : Window
{
    public Layout RootLayout { get; private set; } = null!;

    public BoundExampleWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        RootLayout = this.FindControl<Layout>("RootLayout");
    }
}