using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia.Threading;
using Tabalonia.Core;

namespace Tabalonia;

public class DragablzItem : ContentControl, IStyleable
{
    #region Constants

    public const string ThumbPartName = "PART_Thumb";

    #endregion

    #region Private Fields

    private readonly SerialDisposable _templateSubscriptions = new();
    private readonly SerialDisposable _rightMouseUpCleanUpDisposable = new();

    private Thumb? _customThumb;
    private Thumb? _thumb;
    private bool _seizeDragWithTemplate;
    private Action<DragablzItem>? _dragSeizedContinuation;
    private bool _isTemplateThumbWithMouseAfterSeize;
    private bool _isDragging;
    private int _logicalIndex;
    private bool _isSiblingDragging;

    #endregion

    #region IStyleable

    Type IStyleable.StyleKey => typeof(DragablzItem);

    #endregion

    #region Avalonia Properties

    public static readonly StyledProperty<double> XProperty =
        AvaloniaProperty.Register<DragablzItem, double>(nameof(X));

    public static readonly StyledProperty<double> YProperty =
        AvaloniaProperty.Register<DragablzItem, double>(nameof(Y));

    public static readonly DirectProperty<DragablzItem, bool> IsDraggingProperty =
        AvaloniaProperty.RegisterDirect<DragablzItem, bool>(nameof(IsDragging),
            o => o.IsDragging, (o, v) => o.IsDragging = v);

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<DragablzItem, bool>(nameof(IsSelected), false,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<DragablzItem, int> LogicalIndexProperty =
        AvaloniaProperty.RegisterDirect<DragablzItem, int>(nameof(LogicalIndex),
            o => o.LogicalIndex, (o, v) => o.LogicalIndex = v);

    public static readonly DirectProperty<DragablzItem, bool> IsSiblingDraggingProperty =
        AvaloniaProperty.RegisterDirect<DragablzItem, bool>(nameof(IsSiblingDragging),
            o => o.IsSiblingDragging, (o, v) => o.IsSiblingDragging = v);

    #endregion

    #region Routed Events

    public static readonly RoutedEvent<DragablzDragCompletedEventArgs> DragCompleted =
        RoutedEvent.Register<DragablzItem, DragablzDragCompletedEventArgs>("DragCompleted",
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedPropertyChangedEventArgs<double>> XChangedEvent =
        RoutedEvent.Register<DragablzItem, RoutedPropertyChangedEventArgs<double>>("XChanged",
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedPropertyChangedEventArgs<double>> YChangedEvent =
        RoutedEvent.Register<DragablzItem, RoutedPropertyChangedEventArgs<double>>("YChanged",
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedPropertyChangedEventArgs<int>> LogicalIndexChangedEvent =
        RoutedEvent.Register<DragablzItem, RoutedPropertyChangedEventArgs<int>>("LogicalIndexChanged",
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedPropertyChangedEventArgs<bool>> IsDraggingChangedEvent =
        RoutedEvent.Register<DragablzItem, RoutedPropertyChangedEventArgs<bool>>("IsDraggingChanged",
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DragablzDragStartedEventArgs> DragStarted =
        RoutedEvent.Register<DragablzItem, DragablzDragStartedEventArgs>("DragStarted",
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DragablzDragDeltaEventArgs> DragDelta =
        RoutedEvent.Register<DragablzItem, DragablzDragDeltaEventArgs>("DragDelta",
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DragablzDragDeltaEventArgs> PreviewDragDelta =
        RoutedEvent.Register<DragablzItem, DragablzDragDeltaEventArgs>("PreviewDragDelta",
            RoutingStrategies.Tunnel);

    public static readonly RoutedEvent IsSiblingDraggingChangedEvent =
        RoutedEvent.Register<DragablzItem, RoutedPropertyChangedEventArgs<bool>>("IsSiblingDraggingChanged",
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DragablzItemEventArgs> MouseDownWithinEvent =
        RoutedEvent.Register<DragablzItem, DragablzItemEventArgs>("MouseDownWithin",
            RoutingStrategies.Bubble);

    #endregion

    #region Attached Properties

    public static readonly AttachedProperty<SizeGrip> SizeGripProperty =
        AvaloniaProperty.RegisterAttached<DragablzItem, Control, SizeGrip>("SizeGrip");

    public static SizeGrip GetSizeGrip(Control element)
        => element.GetValue(SizeGripProperty);

    public static void SetSizeGrip(Control element, SizeGrip value)
        => element.SetValue(SizeGripProperty, value);

    private static void SizeGripPropertyChangedCallback(IAvaloniaObject dependencyObject)
    {
        if (dependencyObject is not Thumb thumb)
            return;

        thumb.DragDelta += SizeThumbOnDragDelta;
    }

    private static void SizeThumbOnDragDelta(object? sender, VectorEventArgs dragDeltaEventArgs)
    {
        var thumb = ((Thumb) sender);
        var dragablzItem = thumb.VisualTreeAncestory().OfType<DragablzItem>().FirstOrDefault();
        if (dragablzItem == null) return;

        var sizeGrip = thumb.GetValue(SizeGripProperty);
        var width = dragablzItem.Bounds.Width;
        var height = dragablzItem.Bounds.Height;
        var x = dragablzItem.X;
        var y = dragablzItem.Y;
        switch (sizeGrip)
        {
            case SizeGrip.NotApplicable:
                break;
            case SizeGrip.Left:
                width += -dragDeltaEventArgs.Vector.X;
                x += dragDeltaEventArgs.Vector.X;
                break;
            case SizeGrip.TopLeft:
                width += -dragDeltaEventArgs.Vector.X;
                height += -dragDeltaEventArgs.Vector.Y;
                x += dragDeltaEventArgs.Vector.X;
                y += dragDeltaEventArgs.Vector.Y;
                break;
            case SizeGrip.Top:
                height += -dragDeltaEventArgs.Vector.Y;
                y += dragDeltaEventArgs.Vector.Y;
                break;
            case SizeGrip.TopRight:
                height += -dragDeltaEventArgs.Vector.Y;
                width += dragDeltaEventArgs.Vector.X;
                y += dragDeltaEventArgs.Vector.Y;
                break;
            case SizeGrip.Right:
                width += dragDeltaEventArgs.Vector.X;
                break;
            case SizeGrip.BottomRight:
                width += dragDeltaEventArgs.Vector.X;
                height += dragDeltaEventArgs.Vector.Y;
                break;
            case SizeGrip.Bottom:
                height += dragDeltaEventArgs.Vector.Y;
                break;
            case SizeGrip.BottomLeft:
                height += dragDeltaEventArgs.Vector.Y;
                width += -dragDeltaEventArgs.Vector.X;
                x += dragDeltaEventArgs.Vector.X;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        dragablzItem.SetValue(XProperty, x);
        dragablzItem.SetValue(YProperty, y);
        dragablzItem.SetValue(WidthProperty, Math.Max(width, thumb.DesiredSize.Width));
        dragablzItem.SetValue(HeightProperty, Math.Max(height, thumb.DesiredSize.Height));
    }

    /// <summary>
    /// <see cref="DragablzItem" /> templates contain a thumb, which is used to drag the item around.
    /// For most scenarios this is fine, but by setting this flag to <value>true</value> you can define
    /// a custom thumb in your content, without having to override the template.  This can be useful if you
    /// have extra content; such as a custom button that you want the user to be able to interact with (as usually
    /// the default thumb will handle mouse interaction).
    /// </summary>
    public static readonly AttachedProperty<bool> IsCustomThumbProperty =
        AvaloniaProperty.RegisterAttached<DragablzItem, Thumb, bool>("IsCustomThumb");


    private static void IsCustomThumbPropertyChangedCallback(IAvaloniaObject dependencyObject)
    {
        if (dependencyObject is not Thumb thumb)
            throw new ApplicationException("IsCustomThumb can only be applied to a thumb");

        if (thumb.IsArrangeValid)
            ApplyCustomThumbSetting(thumb);
        //else
        //    thumb.Loaded += CustomThumbOnLoaded;
    }

    /// <summary>
    /// <see cref="DragablzItem" /> templates contain a thumb, which is used to drag the item around.
    /// For most scenarios this is fine, but by setting this flag to <value>true</value> you can define
    /// a custom thumb in your content, without having to override the template.  This can be useful if you
    /// have extra content; such as a custom button that you want the user to be able to interact with (as usually
    /// the default thumb will handle mouse interaction).
    /// </summary>
    public static void SetIsCustomThumb(Thumb element, bool value)
        => element.SetValue(IsCustomThumbProperty, value);

    public static bool GetIsCustomThumb(Thumb element)
        => element.GetValue(IsCustomThumbProperty);

    /// <summary>
    /// Allows item content to be rotated (in suppported templates), typically for use in a vertical/side tab.
    /// </summary>
    public static readonly AttachedProperty<double> ContentRotateTransformAngleProperty =
        AvaloniaProperty.RegisterAttached<DragablzItem, IAvaloniaObject, double>("ContentRotateTransformAngle",
            inherits: true);

    /// <summary>
    /// Allows item content to be rotated (in suppported templates), typically for use in a vertical/side tab.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetContentRotateTransformAngle(IAvaloniaObject element, double value)
        => element.SetValue(ContentRotateTransformAngleProperty, value);

    /// <summary>
    /// Allows item content to be rotated (in suppported templates), typically for use in a vertical/side tab.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static double GetContentRotateTransformAngle(IAvaloniaObject element)
        => element.GetValue(ContentRotateTransformAngleProperty);

    #endregion

    #region Internal Properties

    internal Point MouseAtDragStart { get; set; }

    internal string PartitionAtDragStart { get; set; }

    internal bool IsDropTargetFound { get; set; }

    internal object? UnderlyingContent { get; set; }

    #endregion

    #region Public Properties

    public double X
    {
        get => GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    public double Y
    {
        get => GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    public int LogicalIndex
    {
        get => GetValue(LogicalIndexProperty);
        internal set => SetAndRaise(LogicalIndexProperty, ref _logicalIndex, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool IsDragging
    {
        get => GetValue(IsDraggingProperty);
        internal set => SetAndRaise(IsDraggingProperty, ref _isDragging, value);
    }

    public bool IsSiblingDragging
    {
        get => GetValue(IsSiblingDraggingProperty);
        internal set => SetAndRaise(IsSiblingDraggingProperty, ref _isSiblingDragging, value);
    }

    #endregion

    #region Events

    public event EventHandler<RoutedPropertyChangedEventArgs<double>> XChanged
    {
        add => AddHandler(XChangedEvent, value);
        remove => RemoveHandler(XChangedEvent, value);
    }

    public event EventHandler<RoutedPropertyChangedEventArgs<double>> YChanged
    {
        add => AddHandler(YChangedEvent, value);
        remove => RemoveHandler(YChangedEvent, value);
    }

    public event EventHandler<RoutedPropertyChangedEventArgs<int>> LogicalIndexChanged
    {
        add => AddHandler(LogicalIndexChangedEvent, value);
        remove => RemoveHandler(LogicalIndexChangedEvent, value);
    }

    public event EventHandler<RoutedPropertyChangedEventArgs<bool>> IsDraggingChanged
    {
        add => AddHandler(IsDraggingChangedEvent, value);
        remove => RemoveHandler(IsDraggingChangedEvent, value);
    }

    public event EventHandler<RoutedPropertyChangedEventArgs<bool>> IsSiblingDraggingChanged
    {
        add => AddHandler(IsSiblingDraggingChangedEvent, value);
        remove => RemoveHandler(IsSiblingDraggingChangedEvent, value);
    }

    #endregion

    #region Constructors

    static DragablzItem()
    {
        SizeGripProperty.Changed.Subscribe(x => SizeGripPropertyChangedCallback(x.Sender));
        IsCustomThumbProperty.Changed.Subscribe(x => IsCustomThumbPropertyChangedCallback(x.Sender));
        AffectsMeasure<DragablzItem>(IsSelectedProperty);
    }

    public DragablzItem()
    {
        AddHandler(PointerPressedEvent, PointerPressedHandler, handledEventsToo: true);
    }

    #endregion

    #region Internal Methods

    internal void InstigateDrag(Action<DragablzItem> continuation)
    {
        _dragSeizedContinuation = continuation;
        //var thumb = GetTemplateChild(ThumbPartName) as Thumb;
        //if (thumb != null)
        //{
        //    thumb.CaptureMouse();
        //}
        //else
        //    _seizeDragWithTemplate = true;
    }

    #endregion

    #region Protected Methods

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == XProperty)
        {
            var args = new RoutedPropertyChangedEventArgs<T>(
                change.OldValue.Value,
                change.NewValue.Value)
            {
                RoutedEvent = XChangedEvent
            };

            RaiseEvent(args);
        }
        else if (change.Property == YProperty)
        {
            var args = new RoutedPropertyChangedEventArgs<T>(
                change.OldValue.Value,
                change.NewValue.Value)
            {
                RoutedEvent = YChangedEvent
            };
            RaiseEvent(args);
        }
        else if (change.Property == IsDraggingProperty)
        {
            var args = new RoutedPropertyChangedEventArgs<T>(
                    change.OldValue.Value,
                    change.NewValue.Value)
                {RoutedEvent = IsDraggingChangedEvent};
            RaiseEvent(args);
        }
        else if (change.Property == IsSiblingDraggingProperty)
        {
            var args = new RoutedPropertyChangedEventArgs<T>(
                change.OldValue.Value,
                change.NewValue.Value)
            {
                RoutedEvent = IsSiblingDraggingChangedEvent
            };
            RaiseEvent(args);
        }
        else if (change.Property == LogicalIndexProperty)
        {
            var args = new RoutedPropertyChangedEventArgs<T>(
                change.OldValue.Value,
                change.NewValue.Value)
            {
                RoutedEvent = LogicalIndexChangedEvent
            };
            RaiseEvent(args);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            if (_thumb != null)
            {
                var currentThumbIsHitTestVisible = _thumb.IsHitTestVisible;
                _thumb.SetValue(IsHitTestVisibleProperty, false);
                _rightMouseUpCleanUpDisposable.Disposable = Disposable.Create(() =>
                {
                    _thumb.SetValue(IsHitTestVisibleProperty, currentThumbIsHitTestVisible);
                });
            }
            else
            {
                _rightMouseUpCleanUpDisposable.Disposable = Disposable.Empty;
            }
        }

        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            _rightMouseUpCleanUpDisposable.Disposable = Disposable.Empty;
        }

        base.OnPointerReleased(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var (thumb, disposable) = SelectAndSubscribeToThumb(e);
        _templateSubscriptions.Disposable = disposable;

        if (_seizeDragWithTemplate)
        {
            _isTemplateThumbWithMouseAfterSeize = true;
            //Mouse.AddLostMouseCaptureHandler(this, LostMouseAfterSeizeHandler);
            _dragSeizedContinuation?.Invoke(this);
            _dragSeizedContinuation = null;

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                //var eventArgs = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice,
                //    0,
                //    MouseButton.Left)
                //{
                //    RoutedEvent = MouseLeftButtonDownEvent
                //};

                var eventArgs = new PointerEventArgs(PointerPressedEvent, this, new Pointer(0, PointerType.Mouse, true),
                    this, new Point(), 0, PointerPointProperties.None, KeyModifiers.None);

                thumb.RaiseEvent(eventArgs);
            });
        }

        _seizeDragWithTemplate = false;
    }

    protected void OnDragStarted(DragablzDragStartedEventArgs e) => RaiseEvent(e);

    protected void OnPreviewDragDelta(DragablzDragDeltaEventArgs e) => RaiseEvent(e);

    protected void OnDragDelta(DragablzDragDeltaEventArgs e) => RaiseEvent(e);

    protected void OnDragCompleted(VectorEventArgs e)
    {
        var args = new DragablzDragCompletedEventArgs(DragCompleted, this, e);
        RaiseEvent(args);

        //OK, this is a cheeky bit.  A completed drag may have occured after a tab as been pushed
        //intom a new window, which means we may have reverted to the template thumb.  So, let's
        //refresh the thumb in case the user has a custom one
        _customThumb = FindCustomThumb();
        //_templateSubscriptions.Disposable = SelectAndSubscribeToThumb().Item2;
    }

    #endregion

    #region Private Methods

    private Tuple<Thumb, IDisposable> SelectAndSubscribeToThumb(TemplateAppliedEventArgs e)
    {
        var templateThumb = e.Find<Thumb>(ThumbPartName);
        templateThumb.SetValue(IsHitTestVisibleProperty, _customThumb == null);

        _thumb = _customThumb ?? templateThumb;

        _thumb.DragStarted += ThumbOnDragStarted;
        _thumb.DragDelta += ThumbOnDragDelta;
        _thumb.DragCompleted += ThumbOnDragCompleted;

        var tidyUpThumb = _thumb;

        var disposable = Disposable.Create(() =>
        {
            tidyUpThumb.DragStarted -= ThumbOnDragStarted;
            tidyUpThumb.DragDelta -= ThumbOnDragDelta;
            tidyUpThumb.DragCompleted -= ThumbOnDragCompleted;
        });

        return new Tuple<Thumb, IDisposable>(_thumb, disposable);
    }

    private void ThumbOnDragCompleted(object? sender, VectorEventArgs dragCompletedEventArgs)
    {
        OnDragCompleted(dragCompletedEventArgs);
        MouseAtDragStart = new Point();
    }

    private void ThumbOnDragDelta(object? sender, VectorEventArgs dragDeltaEventArgs)
    {
        var thumb = (Thumb) sender;

        var previewEventArgs = new DragablzDragDeltaEventArgs(PreviewDragDelta, this, dragDeltaEventArgs);
        OnPreviewDragDelta(previewEventArgs);
        if (previewEventArgs.Cancel)
        {
            //thumb.CancelDrag();
        }

        if (!previewEventArgs.Handled)
        {
            var eventArgs = new DragablzDragDeltaEventArgs(DragDelta, this, dragDeltaEventArgs);
            OnDragDelta(eventArgs);

            if (eventArgs.Cancel)
            {
                //thumb.CancelDrag();
            }
        }
    }

    private void ThumbOnDragStarted(object? sender, VectorEventArgs args)
    {
        //MouseAtDragStart = Mouse.GetPosition(this);
        MouseAtDragStart = new MouseDevice().GetPosition(this);
        OnDragStarted(new DragablzDragStartedEventArgs(DragStarted, this, args));
    }

    private static void ApplyCustomThumbSetting(Thumb thumb)
    {
        var dragablzItem = thumb.VisualTreeAncestory().OfType<DragablzItem>().FirstOrDefault();
        if (dragablzItem == null) throw new ApplicationException("Cannot find parent DragablzItem for custom thumb");

        var enableCustomThumb = (bool) thumb.GetValue(IsCustomThumbProperty);
        dragablzItem._customThumb = enableCustomThumb ? thumb : null;
        dragablzItem._templateSubscriptions.Disposable = dragablzItem._templateSubscriptions.Disposable;

        //if (dragablzItem._customThumb != null && dragablzItem._isTemplateThumbWithMouseAfterSeize)
        //    dragablzItem.Dispatcher.BeginInvoke(new Action(() => dragablzItem._customThumb.RaiseEvent(new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice,
        //            0,
        //            MouseButton.Left)
        //        { RoutedEvent = MouseLeftButtonDownEvent })));
    }

    private void PointerPressedHandler(object? sender, PointerPressedEventArgs e)
    {
        RaiseEvent(new DragablzItemEventArgs(MouseDownWithinEvent, this));
    }

    private static void CustomThumbOnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
        var thumb = (Thumb) sender;
        //thumb.Loaded -= CustomThumbOnLoaded;
        ApplyCustomThumbSetting(thumb);
    }

    private Thumb FindCustomThumb()
    {
        return this.VisualTreeDepthFirstTraversal().OfType<Thumb>().FirstOrDefault(GetIsCustomThumb);
    }

    #endregion


    //private void LostMouseAfterSeizeHandler(object sender, MouseEventArgs mouseEventArgs)
    //{
    //    _isTemplateThumbWithMouseAfterSeize = false;
    //    Mouse.RemoveLostMouseCaptureHandler(this, LostMouseAfterSeizeHandler);
    //}
}