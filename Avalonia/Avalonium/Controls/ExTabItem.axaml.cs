using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Avalonium;

public class ExTabItem : TabItem
{
    private readonly IDisposable _sizeChanged;
    private Point _prevPoint;
    private bool _isDrag;
    private int _prevZIndex;
    private IReadOnlyList<TabItem>? _items;

    protected TabsControl TabsControl => Parent as TabsControl ?? throw new Exception("Parent is not TabsControl");

    public ExTabItem()
    {
        _sizeChanged = BoundsProperty.Changed.Subscribe(BoundChangedHandler);
    }

    public void InitPosition()
    {
        if (_isDrag)
            return;

        _items = TabsControl.ItemsPresenter.Panel.Children.OfType<TabItem>().ToList();

        var prevWidth = 0.0;

        for (var i = 0; i < TabIndex; i++)
        {
            prevWidth += _items[i].Bounds.Width;
        }

        Canvas.SetLeft(this, prevWidth - GetOffset());
        Canvas.SetTop(this, 0);
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _isDrag = true;
        _prevPoint = e.GetPosition(TabsControl.ItemsPresenter.Panel);
        _prevZIndex = ZIndex;
        ZIndex = int.MaxValue;
        _items = TabsControl.ItemsPresenter.Panel.Children.OfType<TabItem>().ToList();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_isDrag)
            return;

        var point = e.GetPosition(TabsControl.ItemsPresenter.Panel);

        var (dX, dY) = point - _prevPoint;
        
        Canvas.SetLeft(this, TabIndex * Bounds.Width + dX - GetOffset());
        //Canvas.SetTop(_tabItem, dY);

        var left = Bounds.Left;
        var right = Bounds.Right;

        if (_items != null)
            foreach (var tabItem in _items)
            {
                if (ReferenceEquals(tabItem, this))
                    continue;

                var center = tabItem.Bounds.Left + tabItem.Bounds.Width / 2;

                //if (left >= center)
                //{
                //    tabItem.Background = Brushes.Red;
                //}
                //else if (right <= center)
                //{
                //    tabItem.Background = Brushes.Blue;
                //}
                //else
                //{
                //    tabItem.Background = Background;
                //}
            }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        _isDrag = false;
        InitPosition();
        ZIndex = _prevZIndex;
    }

    private static void BoundChangedHandler(AvaloniaPropertyChangedEventArgs<Rect> e)
    {
        if (e.Sender is ExTabItem tabItem)
        {
            tabItem.InitPosition();
        }
    }

    private double GetOffset()
    {
        double offset = TabsControl.AdjacentHeaderItemOffset;

        offset = TabIndex == 0 ? 0.0 : offset;
        return offset * TabIndex;
    }
}