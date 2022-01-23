using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Styling;
using Tabalonia.Core;
using Tabalonia.Dockablz;

namespace Tabalonia;

public enum SizeGrip
{
    NotApplicable,
    Left,
    TopLeft,
    Top,
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft
}

public class DragablzItem : ContentControl, IStyleable
{
    public const string ThumbPartName = "PART_Thumb";

    private readonly SerialDisposable _templateSubscriptions = new SerialDisposable();
    private readonly SerialDisposable _rightMouseUpCleanUpDisposable = new SerialDisposable();

    private Thumb _customThumb;
    private Thumb _thumb;
    private bool _seizeDragWithTemplate;
    private Action<DragablzItem> _dragSeizedContinuation;
    private bool _isTemplateThumbWithMouseAfterSeize = false;

    #region IStyleable

    Type IStyleable.StyleKey => typeof(DragablzItem);

    #endregion

    static DragablzItem()
    {
        SizeGripProperty.Changed.Subscribe(x => SizeGripPropertyChangedCallback(x.Sender));
        IsCustomThumbProperty.Changed.Subscribe(x => IsCustomThumbPropertyChangedCallback(x.Sender));
    }

    public DragablzItem()
    {
        //AddHandler(MouseDownEvent, new RoutedEventHandler(MouseDownHandler), true);
    }
    
    #region Avalonia Properties

    public static readonly StyledProperty<double> XProperty =
        AvaloniaProperty.Register<DragablzItem, double>(nameof(X));

    public static readonly StyledProperty<double> YProperty =
        AvaloniaProperty.Register<DragablzItem, double>(nameof(Y));

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

    private static void SizeThumbOnDragDelta(object sender, VectorEventArgs dragDeltaEventArgs)
    {
        var thumb = ((Thumb)sender);
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

    #endregion

    #region Internal Properties

    internal Point MouseAtDragStart { get; set; }

    internal string PartitionAtDragStart { get; set; }

    internal bool IsDropTargetFound { get; set; }

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

    #endregion

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == XProperty)
        {
            //var args = new RoutedPropertyChangedEventArgs<double>(
            //    (double)e.OldValue,
            //    (double)e.NewValue)
            //{
            //    RoutedEvent = XChangedEvent
            //};

            //RaiseEvent(args);
        }
        else if (change.Property == YProperty)
        {
            //var args = new RoutedPropertyChangedEventArgs<double>(
            //    (double)e.OldValue,
            //    (double)e.NewValue)
            //{
            //    RoutedEvent = YChangedEvent
            //};
            //RaiseEvent(args);
        }
    }

    /*   
   
    public static readonly RoutedEvent XChangedEvent =
        EventManager.RegisterRoutedEvent(
            "XChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double>),
            typeof(DragablzItem));

    public event RoutedPropertyChangedEventHandler<double> XChanged
    {
        add => AddHandler(XChangedEvent, value);
        remove => RemoveHandler(IsDraggingChangedEvent, value);
    }

    public static readonly RoutedEvent YChangedEvent =
        EventManager.RegisterRoutedEvent(
            "YChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double>),
            typeof(DragablzItem));

    public event RoutedPropertyChangedEventHandler<double> YChanged
    {
        add => AddHandler(YChangedEvent, value);
        remove => RemoveHandler(IsDraggingChangedEvent, value);
    }
    
    private static readonly DependencyPropertyKey LogicalIndexPropertyKey =
        DependencyProperty.RegisterReadOnly(
            "LogicalIndex", typeof (int), typeof (DragablzItem),
            new PropertyMetadata(default(int), OnLogicalIndexChanged));

    public static readonly DependencyProperty LogicalIndexProperty =
        LogicalIndexPropertyKey.DependencyProperty;

    public int LogicalIndex
    {
        get => (int) GetValue(LogicalIndexProperty);
        internal set => SetValue(LogicalIndexPropertyKey, value);
    }

    public static readonly RoutedEvent LogicalIndexChangedEvent =
        EventManager.RegisterRoutedEvent(
            "LogicalIndexChanged",
            RoutingStrategy.Bubble,
            typeof (RoutedPropertyChangedEventHandler<int>),
            typeof (DragablzItem));

    public event RoutedPropertyChangedEventHandler<int> LogicalIndexChanged
    {
        add => AddHandler(LogicalIndexChangedEvent, value);
        remove => RemoveHandler(LogicalIndexChangedEvent, value);
    }

    private static void OnLogicalIndexChanged(
        DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = (DragablzItem) d;
        var args = new RoutedPropertyChangedEventArgs<int>(
            (int) e.OldValue,
            (int) e.NewValue)
        {
            RoutedEvent = DragablzItem.LogicalIndexChangedEvent
        };
        instance.RaiseEvent(args);
    } 

    
    

    /// <summary>
    /// Allows item content to be rotated (in suppported templates), typically for use in a vertical/side tab.
    /// </summary>
    public static readonly DependencyProperty ContentRotateTransformAngleProperty = DependencyProperty.RegisterAttached(
        "ContentRotateTransformAngle", typeof (double), typeof (DragablzItem), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Allows item content to be rotated (in suppported templates), typically for use in a vertical/side tab.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetContentRotateTransformAngle(DependencyObject element, double value)
    {
        element.SetValue(ContentRotateTransformAngleProperty, value);
    }

    /// <summary>
    /// Allows item content to be rotated (in suppported templates), typically for use in a vertical/side tab.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static double GetContentRotateTransformAngle(DependencyObject element)
    {
        return (double) element.GetValue(ContentRotateTransformAngleProperty);
    }

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        "IsSelected", typeof(bool), typeof(DragablzItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    public bool IsSelected
    {
        get => (bool) GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private static readonly DependencyPropertyKey IsDraggingPropertyKey =
        DependencyProperty.RegisterReadOnly(
            "IsDragging", typeof (bool), typeof (DragablzItem),
            new PropertyMetadata(default(bool), OnIsDraggingChanged));

    public static readonly DependencyProperty IsDraggingProperty =
        IsDraggingPropertyKey.DependencyProperty;

    public bool IsDragging
    {
        get => (bool) GetValue(IsDraggingProperty);
        internal set => SetValue(IsDraggingPropertyKey, value);
    }

    public static readonly RoutedEvent IsDraggingChangedEvent =
        EventManager.RegisterRoutedEvent(
            "IsDraggingChanged",
            RoutingStrategy.Bubble,
            typeof (RoutedPropertyChangedEventHandler<bool>),
            typeof (DragablzItem));

    public event RoutedPropertyChangedEventHandler<bool> IsDraggingChanged
    {
        add => AddHandler(IsDraggingChangedEvent, value);
        remove => RemoveHandler(IsDraggingChangedEvent, value);
    }

    internal object UnderlyingContent { get; set; }

    private static void OnIsDraggingChanged(
        DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = (DragablzItem)d;
        var args = new RoutedPropertyChangedEventArgs<bool>(
            (bool)e.OldValue,
            (bool)e.NewValue) { RoutedEvent = IsDraggingChangedEvent };
        instance.RaiseEvent(args);
    }

    public static readonly RoutedEvent MouseDownWithinEvent =
        EventManager.RegisterRoutedEvent(
            "MouseDownWithin",
            RoutingStrategy.Bubble,
            typeof(DragablzItemEventHandler),
            typeof (DragablzItem));

    private static void OnMouseDownWithin(DependencyObject d)
    {
        var instance = (DragablzItem)d;
        instance.RaiseEvent(new DragablzItemEventArgs(MouseDownWithinEvent, instance));
    }

    private static readonly DependencyPropertyKey IsSiblingDraggingPropertyKey =
        DependencyProperty.RegisterReadOnly(
            "IsSiblingDragging", typeof (bool), typeof (DragablzItem),
            new PropertyMetadata(default(bool), OnIsSiblingDraggingChanged));

    public static readonly DependencyProperty IsSiblingDraggingProperty =
        IsSiblingDraggingPropertyKey.DependencyProperty;

    public bool IsSiblingDragging
    {
        get => (bool) GetValue(IsSiblingDraggingProperty);
        internal set => SetValue(IsSiblingDraggingPropertyKey, value);
    }

    public static readonly RoutedEvent IsSiblingDraggingChangedEvent =
        EventManager.RegisterRoutedEvent(
            "IsSiblingDraggingChanged",
            RoutingStrategy.Bubble,
            typeof (RoutedPropertyChangedEventHandler<bool>),
            typeof (DragablzItem));

    public event RoutedPropertyChangedEventHandler<bool> IsSiblingDraggingChanged
    {
        add => AddHandler(IsSiblingDraggingChangedEvent, value);
        remove => RemoveHandler(IsSiblingDraggingChangedEvent, value);
    }

    private static void OnIsSiblingDraggingChanged(
        DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = (DragablzItem) d;
        var args = new RoutedPropertyChangedEventArgs<bool>(
            (bool) e.OldValue,
            (bool) e.NewValue)
        {
            RoutedEvent = IsSiblingDraggingChangedEvent
        };
        instance.RaiseEvent(args);
    }         

    public static readonly RoutedEvent DragStarted =
        EventManager.RegisterRoutedEvent(
            "DragStarted",
            RoutingStrategy.Bubble,
            typeof(DragablzDragStartedEventHandler),
            typeof(DragablzItem));

    protected void OnDragStarted(DragablzDragStartedEventArgs e)
    {            
        RaiseEvent(e);
    }

    public static readonly RoutedEvent DragDelta =
        EventManager.RegisterRoutedEvent(
            "DragDelta",
            RoutingStrategy.Bubble,
            typeof (DragablzDragDeltaEventHandler),
            typeof (DragablzItem));

    protected void OnDragDelta(DragablzDragDeltaEventArgs e)
    {            
        RaiseEvent(e);            
    }

    public static readonly RoutedEvent PreviewDragDelta =
        EventManager.RegisterRoutedEvent(
            "PreviewDragDelta",
            RoutingStrategy.Tunnel,
            typeof(DragablzDragDeltaEventHandler),
            typeof(DragablzItem));

    protected void OnPreviewDragDelta(DragablzDragDeltaEventArgs e)
    {            
        RaiseEvent(e);
    }

    public static readonly RoutedEvent DragCompleted =
        EventManager.RegisterRoutedEvent(
            "DragCompleted",
            RoutingStrategy.Bubble,
            typeof(DragablzDragCompletedEventHandler),
            typeof(DragablzItem));

    protected void OnDragCompleted(DragCompletedEventArgs e)
    {
        var args = new DragablzDragCompletedEventArgs(DragCompleted, this, e);
        RaiseEvent(args);

        //OK, this is a cheeky bit.  A completed drag may have occured after a tab as been pushed
        //intom a new window, which means we may have reverted to the template thumb.  So, let's
        //refresh the thumb in case the user has a custom one
        _customThumb = FindCustomThumb();
        _templateSubscriptions.Disposable = SelectAndSubscribeToThumb().Item2;
    }

    

   
   

    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {            
        if (_thumb != null)
        {
            var currentThumbIsHitTestVisible = _thumb.IsHitTestVisible;
            _thumb.SetCurrentValue(IsHitTestVisibleProperty, false);
            _rightMouseUpCleanUpDisposable.Disposable = Disposable.Create(() =>
            {
                _thumb.SetCurrentValue(IsHitTestVisibleProperty, currentThumbIsHitTestVisible);
            });
        }
        else
        {
            _rightMouseUpCleanUpDisposable.Disposable = Disposable.Empty;
        }            
            
        base.OnPreviewMouseRightButtonDown(e);
    }

    protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
    {
        _rightMouseUpCleanUpDisposable.Disposable = Disposable.Empty;
        base.OnPreviewMouseRightButtonUp(e);
    }

    private void LostMouseAfterSeizeHandler(object sender, MouseEventArgs mouseEventArgs)
    {
        _isTemplateThumbWithMouseAfterSeize = false;
        Mouse.RemoveLostMouseCaptureHandler(this, LostMouseAfterSeizeHandler);
    }

    internal void InstigateDrag(Action<DragablzItem> continuation)
    {
        _dragSeizedContinuation = continuation;
        var thumb = GetTemplateChild(ThumbPartName) as Thumb;
        if (thumb != null)
        {
            thumb.CaptureMouse();             
        }
        else
            _seizeDragWithTemplate = true;
    }

   

   

    private void MouseDownHandler(object sender, RoutedEventArgs routedEventArgs)
    {
        OnMouseDownWithin(this);
    }

    private static void CustomThumbOnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
        var thumb = (Thumb)sender;
        thumb.Loaded -= CustomThumbOnLoaded;
        ApplyCustomThumbSetting(thumb);
    }

    private Thumb FindCustomThumb()
    {
        return this.VisualTreeDepthFirstTraversal().OfType<Thumb>().FirstOrDefault(GetIsCustomThumb);
    }        

  

  

    */

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var (thumb, disposable) = SelectAndSubscribeToThumb(e);
        _templateSubscriptions.Disposable = disposable;

        if (_seizeDragWithTemplate)
        {
            _isTemplateThumbWithMouseAfterSeize = true;
            //Mouse.AddLostMouseCaptureHandler(this, LostMouseAfterSeizeHandler);
            if (_dragSeizedContinuation != null)
                _dragSeizedContinuation(this);
            _dragSeizedContinuation = null;

            //Dispatcher.BeginInvoke(new Action(() => thumb.RaiseEvent(new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice,
            //        0,
            //        MouseButton.Left)
            //{ RoutedEvent = MouseLeftButtonDownEvent })));
        }
        _seizeDragWithTemplate = false;
    }
    
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
        //OnDragCompleted(dragCompletedEventArgs);
        MouseAtDragStart = new Point();
    }

    private void ThumbOnDragDelta(object? sender, VectorEventArgs dragDeltaEventArgs)
    {
        var thumb = (Thumb)sender;

        //var previewEventArgs = new DragablzDragDeltaEventArgs(PreviewDragDelta, this, dragDeltaEventArgs);
        //OnPreviewDragDelta(previewEventArgs);
        //if (previewEventArgs.Cancel)
        //    thumb.CancelDrag();
        //if (!previewEventArgs.Handled)
        //{
        //    var eventArgs = new DragablzDragDeltaEventArgs(DragDelta, this, dragDeltaEventArgs);
        //    OnDragDelta(eventArgs);
        //    if (eventArgs.Cancel)
        //        thumb.CancelDrag();
        //}
    }

    private void ThumbOnDragStarted(object? sender, VectorEventArgs dragStartedEventArgs)
    {
        //MouseAtDragStart = Mouse.GetPosition(this);
        //OnDragStarted(new DragablzDragStartedEventArgs(DragStarted, this, dragStartedEventArgs));
    }

    private static void ApplyCustomThumbSetting(Thumb thumb)
    {
        var dragablzItem = thumb.VisualTreeAncestory().OfType<DragablzItem>().FirstOrDefault();
        if (dragablzItem == null) throw new ApplicationException("Cannot find parent DragablzItem for custom thumb");

        var enableCustomThumb = (bool)thumb.GetValue(IsCustomThumbProperty);
        dragablzItem._customThumb = enableCustomThumb ? thumb : null;
        dragablzItem._templateSubscriptions.Disposable = dragablzItem._templateSubscriptions.Disposable;

        //if (dragablzItem._customThumb != null && dragablzItem._isTemplateThumbWithMouseAfterSeize)
        //    dragablzItem.Dispatcher.BeginInvoke(new Action(() => dragablzItem._customThumb.RaiseEvent(new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice,
        //            0,
        //            MouseButton.Left)
        //        { RoutedEvent = MouseLeftButtonDownEvent })));
    }
}