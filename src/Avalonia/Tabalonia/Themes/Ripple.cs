using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace Tabalonia.Themes;

//[TemplateVisualState(GroupName = "CommonStates", Name = TemplateStateNormal)]
//[TemplateVisualState(GroupName = "CommonStates", Name = TemplateStateMousePressed)]
//[TemplateVisualState(GroupName = "CommonStates", Name = TemplateStateMouseOut)]
public class Ripple : ContentControl, IStyleable
{
    public const string TemplateStateNormal = "Normal";
    public const string TemplateStateMousePressed = "MousePressed";
    public const string TemplateStateMouseOut = "MouseOut";

    private static readonly HashSet<Ripple> PressedInstances = new();


    #region IStyleable

    Type IStyleable.StyleKey => typeof(Ripple);

    #endregion

    #region Avalonia Properties

    public static readonly StyledProperty<Brush> FeedbackProperty =
        AvaloniaProperty.Register<Ripple, Brush>(nameof(Feedback));

    public static readonly DirectProperty<Ripple, double> RippleSizeProperty =
        AvaloniaProperty.RegisterDirect<Ripple, double>(nameof(RippleSize),
            o => o.RippleSize, (o, v) => o.RippleSize = v);

    public static readonly DirectProperty<Ripple, double> RippleXProperty =
        AvaloniaProperty.RegisterDirect<Ripple, double>(nameof(RippleX),
            o => o.RippleX, (o, v) => o.RippleX = v);


    public static readonly DirectProperty<Ripple, double> RippleYProperty =
        AvaloniaProperty.RegisterDirect<Ripple, double>(nameof(RippleY),
            o => o.RippleY, (o, v) => o.RippleY = v);
    
    /// <summary>
    ///   The DependencyProperty for the RecognizesAccessKey property. 
    ///   Default Value: false 
    /// </summary> 
    public static readonly StyledProperty<bool> RecognizesAccessKeyProperty =
        AvaloniaProperty.Register<Ripple, bool>(nameof(RecognizesAccessKey));

    private double _rippleSize;
    private double _rippleX;
    private double _rippleY;

    #endregion

    #region Public Properties

    public Brush Feedback
    {
        get => GetValue(FeedbackProperty);
        set => SetValue(FeedbackProperty, value);
    }

    public double RippleSize
    {
        get => GetValue(RippleSizeProperty);
        private set => SetAndRaise(RippleSizeProperty, ref _rippleSize, value);
    }

    public double RippleX
    {
        get => GetValue(RippleXProperty);
        private set => SetAndRaise(RippleXProperty, ref _rippleX, value);
    }

    public double RippleY
    {
        get => GetValue(RippleYProperty);
        private set => SetAndRaise(RippleYProperty, ref _rippleY, value);
    }

    /// <summary> 
    ///   Determine if Ripple should use AccessText in its style
    /// </summary> 
    public bool RecognizesAccessKey
    {
        get => GetValue(RecognizesAccessKeyProperty);
        set => SetValue(RecognizesAccessKeyProperty, value);
    }

    #endregion


    static Ripple()
    {
        //EventManager.RegisterClassHandler(typeof(Window), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
        //EventManager.RegisterClassHandler(typeof(Window), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMouveEventHandler), true);
        //EventManager.RegisterClassHandler(typeof(UserControl), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
        //EventManager.RegisterClassHandler(typeof(UserControl), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMouveEventHandler), true);
    }

    public Ripple()
    {
        //SizeChanged += OnSizeChanged;
    }

    //private static void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
    //{
    //    foreach (var ripple in PressedInstances)
    //    {
    //        // adjust the transition scale time according to the current animated scale
    //        var scaleTrans = ripple.Template.FindName("ScaleTransform", ripple) as ScaleTransform;
    //        if (scaleTrans != null)
    //        {
    //            double currentScale = scaleTrans.ScaleX;
    //            var newTime = TimeSpan.FromMilliseconds(300 * (1.0 - currentScale));

    //            // change the scale animation according to the current scale
    //            var scaleXKeyFrame =
    //                ripple.Template.FindName("MousePressedToNormalScaleXKeyFrame", ripple) as EasingDoubleKeyFrame;
    //            if (scaleXKeyFrame != null)
    //            {
    //                scaleXKeyFrame.KeyTime = KeyTime.FromTimeSpan(newTime);
    //            }

    //            var scaleYKeyFrame =
    //                ripple.Template.FindName("MousePressedToNormalScaleYKeyFrame", ripple) as EasingDoubleKeyFrame;
    //            if (scaleYKeyFrame != null)
    //            {
    //                scaleYKeyFrame.KeyTime = KeyTime.FromTimeSpan(newTime);
    //            }
    //        }

    //        VisualStateManager.GoToState(ripple, TemplateStateNormal, true);
    //    }

    //    PressedInstances.Clear();
    //}

    //private static void MouseMouveEventHandler(object sender, MouseEventArgs e)
    //{
    //    foreach (var ripple in PressedInstances.ToList())
    //    {
    //        var relativePosition = Mouse.GetPosition(ripple);
    //        if (relativePosition.X < 0
    //            || relativePosition.Y < 0
    //            || relativePosition.X >= ripple.ActualWidth
    //            || relativePosition.Y >= ripple.ActualHeight)

    //        {
    //            VisualStateManager.GoToState(ripple, TemplateStateMouseOut, true);
    //            PressedInstances.Remove(ripple);
    //        }
    //    }
    //}


    //protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    //{
    //    var point = e.GetPosition(this);

    //    if (RippleAssist.GetIsCentered(this))
    //    {
    //        var innerContent = (Content as FrameworkElement);

    //        if (innerContent != null)
    //        {
    //            var position = innerContent.TransformToAncestor(this)
    //                .Transform(new Point(0, 0));

    //            RippleX = position.X + innerContent.ActualWidth / 2 - RippleSize / 2;
    //            RippleY = position.Y + innerContent.ActualHeight / 2 - RippleSize / 2;
    //        }
    //        else
    //        {
    //            RippleX = ActualWidth / 2 - RippleSize / 2;
    //            RippleY = ActualHeight / 2 - RippleSize / 2;
    //        }
    //    }
    //    else
    //    {
    //        RippleX = point.X - RippleSize / 2;
    //        RippleY = point.Y - RippleSize / 2;
    //    }

    //    VisualStateManager.GoToState(this, TemplateStateNormal, false);
    //    VisualStateManager.GoToState(this, TemplateStateMousePressed, true);
    //    PressedInstances.Add(this);

    //    base.OnPreviewMouseLeftButtonDown(e);
    //}


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        //VisualStateManager.GoToState(this, TemplateStateNormal, false);
    }
 
 
    //private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
    //{
    //    var innerContent = (Content as FrameworkElement);

    //    double width, height;

    //    if (RippleAssist.GetIsCentered(this) && innerContent != null)
    //    {
    //        width = innerContent.ActualWidth;
    //        height = innerContent.ActualHeight;
    //    }
    //    else
    //    {
    //        width = sizeChangedEventArgs.NewSize.Width;
    //        height = sizeChangedEventArgs.NewSize.Height;
    //    }

    //    var radius = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));

    //    RippleSize = 2 * radius * RippleAssist.GetRippleSizeMultiplier(this);
    //}
}