using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tabalonia.Demo;

public partial class LayoutManagementExample : UserControl
{
    public LayoutManagementExample()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}