using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonium
{
    public partial class DebugView : UserControl
    {
        private static TextBlock _dragDeltaTextBlock;
        private static TextBlock _dragStartTextBlock;

        public DebugView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _dragStartTextBlock = this.FindControl<TextBlock>("DragStartTextBlock");
            _dragDeltaTextBlock = this.FindControl<TextBlock>("DragDeltaTextBlock");
        }

        public static void ShowStartText(object vector)
        {
            _dragStartTextBlock.Text = vector.ToString();
        }
        public static void ShowDeltaText(object vector)
        {
            _dragDeltaTextBlock.Text = vector.ToString();
        }
    }
}