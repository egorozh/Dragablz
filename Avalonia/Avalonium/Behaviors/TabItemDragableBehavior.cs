using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonium.Behaviors;

internal class TabItemDragableBehavior : Behavior<TabItem>
{
    #region Private Fields

    private readonly TabsControl _tabsControl;
    private Point _prevPoint;
    private bool _isDrag;
    private int _prevZIndex;
    private TabItem _tabItem = null!;
    private IReadOnlyList<TabItem>? _items;
    private readonly int _tabIndex;
    private readonly IDisposable _sizeChanged;

    #endregion

    #region Constructor

    public TabItemDragableBehavior(TabsControl tabsControl, int tabIndex)
    {
        _tabsControl = tabsControl;
        _tabIndex = tabIndex;
        _sizeChanged = Visual.BoundsProperty.Changed.Subscribe(BoundChangedHandler);
    }

    #endregion

    #region Protected Methods

    protected override void OnAttached()
    {
        _tabItem = AssociatedObject ?? throw new Exception("AssociatedObject is null");

        SetInitMargin();
        SetInitPosition();

        _tabItem.PointerPressed += AssociatedObjectOnPointerPressed;
        _tabItem.PointerMoved += AssociatedObjectOnPointerMoved;
        _tabItem.PointerReleased += AssociatedObjectOnPointerReleased;
    }

    private static void BoundChangedHandler(AvaloniaPropertyChangedEventArgs<Rect> e)
    {
        if (e.Sender is TabItem tabItem)
        {
            Interaction.GetBehaviors(tabItem)
                .OfType<TabItemDragableBehavior>()
                .First()
                .SizeChanged();
        }
    }

    private void SizeChanged()
    {
        SetInitPosition();
        _sizeChanged.Dispose();
    }

    protected override void OnDetaching()
    {
        _tabItem.PointerPressed -= AssociatedObjectOnPointerPressed;
        _tabItem.PointerMoved -= AssociatedObjectOnPointerMoved;
        _tabItem.PointerReleased -= AssociatedObjectOnPointerReleased;
    }

    #endregion

    #region Private Methods

    private void AssociatedObjectOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDrag = true;
        _prevPoint = e.GetPosition(_tabsControl.ItemsPresenter.Panel);
        _prevZIndex = _tabItem.ZIndex;
        _tabItem.ZIndex = int.MaxValue;
        _items = _tabsControl.ItemsPresenter.Panel.Children.OfType<TabItem>().ToList();
    }

    private void AssociatedObjectOnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDrag)
            return;

        var point = e.GetPosition(_tabsControl.ItemsPresenter.Panel);

        var (dX, dY) = point - _prevPoint;

        //_tabItem.Margin = _isFirstTabItem
        //    ? new Thickness(dX, 0, -dX, 0)
        //    : new Thickness(dX - _tabsControl.AdjacentHeaderItemOffset, 0, -dX, 0);

        Canvas.SetLeft(_tabItem, _tabIndex * _tabItem.Bounds.Width + dX);
        //Canvas.SetTop(_tabItem, dY);

        var left = _tabItem.Bounds.Left;
        var right = _tabItem.Bounds.Right;

        if (_items != null)
            foreach (var tabItem in _items)
            {
                if (ReferenceEquals(tabItem, _tabItem))
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
                //    tabItem.Background = _tabItem.Background;
                //}
            }
    }

    private void AssociatedObjectOnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDrag = false;
        SetInitMargin();
        SetInitPosition();
        _tabItem.ZIndex = _prevZIndex;
    }

    private void SetInitPosition()
    {
        Canvas.SetLeft(_tabItem, _tabIndex * _tabItem.Bounds.Width);
        Canvas.SetTop(_tabItem, 0);
    }

    private void SetInitMargin()
    {
        //_tabItem.Margin = _isFirstTabItem
        //    ? new Thickness()
        //    : new Thickness(-_tabsControl.AdjacentHeaderItemOffset, 0, 0, 0);
    }

    #endregion
}