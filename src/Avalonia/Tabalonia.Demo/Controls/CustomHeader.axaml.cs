using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tabalonia.Demo;

public partial class CustomHeader : UserControl
{
    public CustomHeader()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}