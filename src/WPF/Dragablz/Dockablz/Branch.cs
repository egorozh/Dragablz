﻿using System.Windows;
using System.Windows.Controls;

namespace Dragablz.Dockablz;

[TemplatePart(Name = FirstContentPresenterPartName, Type=typeof(ContentPresenter))]
[TemplatePart(Name = SecondContentPresenterPartName, Type = typeof(ContentPresenter))]
public class Branch : Control
{
    private const string FirstContentPresenterPartName = "PART_FirstContentPresenter";
    private const string SecondContentPresenterPartName = "PART_SecondContentPresenter";

    static Branch()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Branch), new FrameworkPropertyMetadata(typeof(Branch)));
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        "Orientation", typeof (Orientation), typeof (Branch), new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation) GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty FirstItemProperty = DependencyProperty.Register(
        "FirstItem", typeof(object), typeof(Branch), new PropertyMetadata(default(object)));

    public object FirstItem
    {
        get => GetValue(FirstItemProperty);
        set => SetValue(FirstItemProperty, value);
    }

    public static readonly DependencyProperty FirstItemLengthProperty = DependencyProperty.Register(
        "FirstItemLength", typeof (GridLength), typeof (Branch), new FrameworkPropertyMetadata(new GridLength(0.49999, GridUnitType.Star), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public GridLength FirstItemLength
    {
        get => (GridLength) GetValue(FirstItemLengthProperty);
        set => SetValue(FirstItemLengthProperty, value);
    }

    public static readonly DependencyProperty SecondItemProperty = DependencyProperty.Register(
        "SecondItem", typeof(object), typeof(Branch), new PropertyMetadata(default(object)));

    public object SecondItem
    {
        get => GetValue(SecondItemProperty);
        set => SetValue(SecondItemProperty, value);
    }

    public static readonly DependencyProperty SecondItemLengthProperty = DependencyProperty.Register(
        "SecondItemLength", typeof(GridLength), typeof(Branch), new FrameworkPropertyMetadata(new GridLength(0.50001, GridUnitType.Star), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public GridLength SecondItemLength
    {
        get => (GridLength) GetValue(SecondItemLengthProperty);
        set => SetValue(SecondItemLengthProperty, value);
    }        

    /// <summary>
    /// Gets the proportional size of the first item, between 0 and 1, where 1 would represent the entire size of the branch.
    /// </summary>
    /// <returns></returns>
    public double GetFirstProportion()
    {
        return (1/(FirstItemLength.Value + SecondItemLength.Value))*FirstItemLength.Value;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        FirstContentPresenter = GetTemplateChild(FirstContentPresenterPartName) as ContentPresenter;
        SecondContentPresenter = GetTemplateChild(SecondContentPresenterPartName) as ContentPresenter;
    }

    internal ContentPresenter FirstContentPresenter { get; private set; }
    internal ContentPresenter SecondContentPresenter { get; private set; }
}