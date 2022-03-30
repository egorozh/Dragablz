using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonium.Events;

namespace Avalonium;

public class DragTabItem : TabItem
{
    private readonly IDisposable _sizeChanged;
    private Point _prevPoint;
    private bool _isDrag;
    private int _prevZIndex;
    private IReadOnlyList<DragTabItem>? _items;
    private Thumb _thumb;

    protected TabsControl TabsControl => Parent as TabsControl ?? throw new Exception("Parent is not TabsControl");

    #region Internal Properties

    internal Point MouseAtDragStart { get; set; }

    #endregion

    #region Routed Events

    public static readonly RoutedEvent<DragablzDragStartedEventArgs> DragStarted =
        RoutedEvent.Register<DragTabItem, DragablzDragStartedEventArgs>("DragStarted", RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DragablzDragDeltaEventArgs> DragDelta =
        RoutedEvent.Register<DragTabItem, DragablzDragDeltaEventArgs>("DragDelta", RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DragablzDragCompletedEventArgs> DragCompleted =
        RoutedEvent.Register<DragTabItem, DragablzDragCompletedEventArgs>("DragCompleted", RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DragablzDragDeltaEventArgs> PreviewDragDelta =
        RoutedEvent.Register<DragTabItem, DragablzDragDeltaEventArgs>("PreviewDragDelta", RoutingStrategies.Tunnel);

    #endregion

    #region Events

    #endregion

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var templateThumb = e.Find<Thumb>("PART_Thumb");

        _thumb = templateThumb;
        _thumb.DragStarted += ThumbOnDragStarted;
        _thumb.DragDelta += ThumbOnDragDelta;
        _thumb.DragCompleted += ThumbOnDragCompleted;
    }

    private void ThumbOnDragStarted(object? sender, VectorEventArgs args)
    {
        _isDrag = true;
        MouseAtDragStart = new MouseDevice().GetPosition(this);
        RaiseEvent(new DragablzDragStartedEventArgs(DragStarted, this, args));
    }

    private void ThumbOnDragDelta(object? sender, VectorEventArgs e)
    {
        var thumb = (Thumb)sender;

        var previewEventArgs = new DragablzDragDeltaEventArgs(PreviewDragDelta, this, e);
        RaiseEvent(previewEventArgs);
        //if (previewEventArgs.Cancel)
        //    thumb.CancelDrag();
        if (!previewEventArgs.Handled)
        {
            var eventArgs = new DragablzDragDeltaEventArgs(DragDelta, this, e);
            RaiseEvent(eventArgs);
            //if (eventArgs.Cancel)
            //    thumb.CancelDrag();
        }
    }

    private void ThumbOnDragCompleted(object? sender, VectorEventArgs e)
    {
        _isDrag = false;
        var args = new DragablzDragCompletedEventArgs(DragCompleted, this, e);
        RaiseEvent(args);
        MouseAtDragStart = new Point();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
    }

    private static void BoundChangedHandler(AvaloniaPropertyChangedEventArgs<Rect> e)
    {
        if (e.Sender is DragTabItem tabItem)
        {
            tabItem.InitPosition();
        }
    }

    public double GetOffset()
    {
        double offset = TabsControl.AdjacentHeaderItemOffset;

        offset = TabIndex == 0 ? 0.0 : offset;
        return offset * TabIndex;
    }
}