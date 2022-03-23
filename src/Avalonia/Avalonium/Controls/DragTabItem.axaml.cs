using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Avalonium;

public class DragTabItem : TabItem
{
    private readonly IDisposable _sizeChanged;
    private Point _prevPoint;
    private bool _isDrag;
    private int _prevZIndex;
    private IReadOnlyList<DragTabItem>? _items;

    protected TabsControl TabsControl => Parent as TabsControl ?? throw new Exception("Parent is not TabsControl");

    public DragTabItem()
    {
        _sizeChanged = BoundsProperty.Changed.Subscribe(BoundChangedHandler);
    }

    public void InitPosition()
    {
        if (_isDrag)
            return;

        _items = TabsControl.ItemsPresenter.Panel.Children.OfType<DragTabItem>().ToList();

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
        _items = TabsControl.ItemsPresenter.Panel.Children.OfType<DragTabItem>().ToList();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_isDrag)
            return;

        var point = e.GetPosition(TabsControl.ItemsPresenter.Panel);

        var (dX, dY) = point - _prevPoint;

        _items = TabsControl.ItemsPresenter.Panel.Children.OfType<DragTabItem>().ToList();

        var prevWidth = 0.0;

        for (var i = 0; i < TabIndex; i++)
        {
            prevWidth += _items[i].Bounds.Width;
        }

        Canvas.SetLeft(this, prevWidth + dX - GetOffset());
        //Canvas.SetTop(_tabItem, dY);

        var left = Bounds.Left;
        var right = Bounds.Right;

        if (_items != null)
            foreach (var tabItem in _items)
            {
                if (ReferenceEquals(tabItem, this))
                    continue;

                var center = tabItem.Bounds.Left + tabItem.Bounds.Width / 2;

                if (left >= center)
                {
                    //var index = tabItem.TabIndex;

                    ////tabItem turn on right
                    //this.TabIndex--;
                    //TabsControl.ItemsPresenter.Panel.Children.Move(index, index + 1);
                    //_items = TabsControl.ItemsPresenter.Panel.Children.OfType<ExTabItem>().ToList();

                    //tabItem.TabIndex = index + 1;
                    //tabItem.InitPosition();
                    //_isDrag = false;
                    //ZIndex = _prevZIndex;

                    tabItem.Background = Brushes.Red;
                    break;
                }
                else if (right >= center)
                {
                    //tabItem turn on left

                    var index = tabItem.TabIndex;

                    if (index > 0)
                    {
                        this.TabIndex++;
                        TabsControl.ItemsPresenter.Panel.Children.Move(index, index - 1);
                        _items = TabsControl.ItemsPresenter.Panel.Children.OfType<DragTabItem>().ToList();

                        tabItem.TabIndex = index - 1;
                        tabItem.InitPosition();
                        _isDrag = false;
                        ZIndex = _prevZIndex;
                        break;
                    }

                    tabItem.Background = Brushes.Blue;
                }
                else
                {
                    tabItem.Background = Background;
                }
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
        if (e.Sender is DragTabItem tabItem)
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