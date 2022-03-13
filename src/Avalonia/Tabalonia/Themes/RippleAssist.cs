using Avalonia;

namespace Tabalonia.Themes;

public class RippleAssist
{
    #region ClipToBound

    public static readonly AttachedProperty<bool> ClipToBoundsProperty =
        AvaloniaProperty.RegisterAttached<RippleAssist, IAvaloniaObject, bool>("ClipToBounds", true, true);
    
    public static void SetClipToBounds(IAvaloniaObject element, bool value) =>
        element.SetValue(ClipToBoundsProperty, value);

    public static bool GetClipToBounds(IAvaloniaObject element) => element.GetValue(ClipToBoundsProperty);

    #endregion

    #region StayOnCenter

    /// <summary>
    /// Set to <c>true</c> to cause the ripple to originate from the centre of the 
    /// content.  Otherwise the effect will originate from the mouse down position.        
    /// </summary>
    public static readonly AttachedProperty<bool> IsCenteredProperty =
        AvaloniaProperty.RegisterAttached<RippleAssist, IAvaloniaObject, bool>("IsCentered", false, true);
    
    /// <summary>
    /// Set to <c>true</c> to cause the ripple to originate from the centre of the 
    /// content.  Otherwise the effect will originate from the mouse down position.        
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetIsCentered(IAvaloniaObject element, bool value) =>
        element.SetValue(IsCenteredProperty, value);

    /// <summary>
    /// Set to <c>true</c> to cause the ripple to originate from the centre of the 
    /// content.  Otherwise the effect will originate from the mouse down position.        
    /// </summary>
    /// <param name="element"></param>        
    public static bool GetIsCentered(IAvaloniaObject element) => element.GetValue(IsCenteredProperty);

    #endregion

    #region RippleSizeMultiplier

    public static readonly AttachedProperty<double> RippleSizeMultiplierProperty =
        AvaloniaProperty.RegisterAttached<RippleAssist, IAvaloniaObject, double>("RippleSizeMultiplier", 1.0, true);
    
    public static void SetRippleSizeMultiplier(IAvaloniaObject element, double value) =>
        element.SetValue(RippleSizeMultiplierProperty, value);

    public static double GetRippleSizeMultiplier(IAvaloniaObject element) =>
        element.GetValue(RippleSizeMultiplierProperty);

    #endregion
}