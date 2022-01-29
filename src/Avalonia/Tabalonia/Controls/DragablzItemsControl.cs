using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using Tabalonia.Core;

namespace Tabalonia;

/// <summary>
/// Items control which typically uses a canvas and 
/// </summary>
public class DragablzItemsControl : ItemsControl, IStyleable
{
    #region Private Fields

    private object[]? _previousSortQueryResult;
    private double _itemsPresenterWidth;
    private double _itemsPresenterHeight;

    #endregion

    #region IStyleable

    Type IStyleable.StyleKey => typeof(DragablzItemsControl);

    #endregion

    #region Internal Properties

    internal Size? LockedMeasure { get; set; }
    internal ContainerCustomisations ContainerCustomisations { get; set; }

    #endregion

    #region Avalonia Properties

    public static readonly StyledProperty<int> FixedItemCountProperty =
        AvaloniaProperty.Register<DragablzItemsControl, int>(nameof(FixedItemCount));

    public static readonly StyledProperty<IItemsOrganiser?> ItemsOrganiserProperty =
        AvaloniaProperty.Register<DragablzItemsControl, IItemsOrganiser?>(nameof(ItemsOrganiser));

    public static readonly StyledProperty<PositionMonitor?> PositionMonitorProperty =
        AvaloniaProperty.Register<DragablzItemsControl, PositionMonitor?>(nameof(PositionMonitor));

    public static readonly DirectProperty<DragablzItemsControl, double> ItemsPresenterWidthProperty =
        AvaloniaProperty.RegisterDirect<DragablzItemsControl, double>(nameof(ItemsPresenterWidth),
            o => o.ItemsPresenterWidth, (o, v) => o.ItemsPresenterWidth = v);

    public static readonly DirectProperty<DragablzItemsControl, double> ItemsPresenterHeightProperty =
        AvaloniaProperty.RegisterDirect<DragablzItemsControl, double>(nameof(ItemsPresenterHeight),
            o => o.ItemsPresenterHeight, (o, v) => o.ItemsPresenterHeight = v);

    #endregion

    #region Public Properties

    public int FixedItemCount
    {
        get => GetValue(FixedItemCountProperty);
        set => SetValue(FixedItemCountProperty, value);
    }

    public IItemsOrganiser? ItemsOrganiser
    {
        get => GetValue(ItemsOrganiserProperty);
        set => SetValue(ItemsOrganiserProperty, value);
    }

    public PositionMonitor? PositionMonitor
    {
        get => GetValue(PositionMonitorProperty);
        set => SetValue(PositionMonitorProperty, value);
    }

    public double ItemsPresenterWidth
    {
        get => GetValue(ItemsPresenterWidthProperty);
        private set => SetAndRaise(ItemsPresenterWidthProperty, ref _itemsPresenterWidth, value);
    }

    public double ItemsPresenterHeight
    {
        get => GetValue(ItemsPresenterHeightProperty);
        private set => SetAndRaise(ItemsPresenterHeightProperty, ref _itemsPresenterHeight, value);
    }

    #endregion

    #region Constructor

    public DragablzItemsControl()
    {
        //ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
        //ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorOnItemsChanged;
        AddHandler(DragablzItem.XChangedEvent, ItemXChanged);
        AddHandler(DragablzItem.YChangedEvent, ItemYChanged);
        AddHandler(DragablzItem.DragDelta, ItemDragDelta);
        AddHandler(DragablzItem.DragCompleted, ItemDragCompleted);
        AddHandler(DragablzItem.DragStarted, ItemDragStarted);
        AddHandler(DragablzItem.MouseDownWithinEvent, ItemMouseDownWithinHandlerTarget);
    }
    
    #endregion

    #region Public Methods

    /// <summary>
    /// Adds an item to the underlying source, displaying in a specific position in rendered control.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="addLocationHint"></param>
    public void AddToSource(object item, AddLocationHint addLocationHint)
        => AddToSource(item, null, addLocationHint);

    /// <summary>
    /// Adds an item to the underlying source, displaying in a specific position in rendered control.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="nearItem"></param>
    /// <param name="addLocationHint"></param>
    public void AddToSource(object item, object? nearItem, AddLocationHint addLocationHint)
    {
        //if (CollectionTeaser.TryCreate(ItemsSource, out var collectionTeaser))
        //    collectionTeaser.Add(item);
        //else
        //    Items.Add(item);

        MoveItem(new MoveItemRequest(item, nearItem, addLocationHint));
    }

    /// <summary>
    /// Move an item in the rendered layout.
    /// </summary>
    /// <param name="moveItemRequest"></param>
    public void MoveItem(MoveItemRequest moveItemRequest)
    {
        if (moveItemRequest == null) throw new ArgumentNullException(nameof(moveItemRequest));

        if (ItemsOrganiser == null) return;

        var dragablzItem = moveItemRequest.Item as DragablzItem
            //??
            //ItemContainerGenerator.ContainerFromItem(
            //    moveItemRequest.Item) as DragablzItem
            ;

        var contextDragablzItem = moveItemRequest.Context as DragablzItem
            //??
            //ItemContainerGenerator.ContainerFromItem(
            //    moveItemRequest.Context) as DragablzItem
            ;

        if (dragablzItem == null) return;

        var sortedItems = DragablzItems().OrderBy(di => di.LogicalIndex).ToList();
        sortedItems.Remove(dragablzItem);

        switch (moveItemRequest.AddLocationHint)
        {
            case AddLocationHint.First:
                sortedItems.Insert(0, dragablzItem);
                break;
            case AddLocationHint.Last:
                sortedItems.Add(dragablzItem);
                break;
            case AddLocationHint.Prior:
            case AddLocationHint.After:
                if (contextDragablzItem == null)
                    return;

                var contextIndex = sortedItems.IndexOf(contextDragablzItem);
                sortedItems.Insert(
                    moveItemRequest.AddLocationHint == AddLocationHint.Prior ? contextIndex : contextIndex + 1,
                    dragablzItem);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //TODO might not be too great for perf on larger lists
        var orderedEnumerable = sortedItems.OrderBy(di => sortedItems.IndexOf(di));

        ItemsOrganiser.Organise(this, new Size(ItemsPresenterWidth, ItemsPresenterHeight), orderedEnumerable);
    }

    #endregion

    #region Protected Methods

    protected override Size MeasureOverride(Size constraint)
    {
        if (ItemsOrganiser == null)
            return base.MeasureOverride(constraint);

        if (LockedMeasure.HasValue)
        {
            ItemsPresenterWidth = LockedMeasure.Value.Width;
            ItemsPresenterHeight = LockedMeasure.Value.Height;
            return LockedMeasure.Value;
        }

        var dragablzItems = DragablzItems().ToList();
        var maxConstraint = new Size(double.PositiveInfinity, double.PositiveInfinity);

        ItemsOrganiser.Organise(this, maxConstraint, dragablzItems);
        (ItemsPresenterWidth, ItemsPresenterHeight) = ItemsOrganiser
            .Measure(this, new Size(Bounds.Width, Bounds.Height), dragablzItems);

        var width = double.IsInfinity(constraint.Width) ? ItemsPresenterWidth : constraint.Width;
        var height = double.IsInfinity(constraint.Height) ? ItemsPresenterHeight : constraint.Height;

        return new Size(width, height);
    }

    //protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    //{
    //    if (ContainerCustomisations != null && ContainerCustomisations.ClearingContainerForItemOverride != null)
    //        ContainerCustomisations.ClearingContainerForItemOverride(element, item);

    //    base.ClearContainerForItemOverride(element, item);

    //    ((DragablzItem)element).SizeChanged -= ItemSizeChangedEventHandler;

    //    Dispatcher.BeginInvoke(new Action(() =>
    //    {
    //        var dragablzItems = DragablzItems().ToList();
    //        if (ItemsOrganiser == null) return;
    //        ItemsOrganiser.Organise(this, new Size(ItemsPresenterWidth, ItemsPresenterHeight), dragablzItems);
    //        var measure = ItemsOrganiser.Measure(this, new Size(ActualWidth, ActualHeight), dragablzItems);
    //        ItemsPresenterWidth = measure.Width;
    //        ItemsPresenterHeight = measure.Height;
    //    }), DispatcherPriority.Input);
    //}

    //protected override bool IsItemItsOwnContainerOverride(object item)
    //{
    //    var dragablzItem = item as DragablzItem;
    //    if (dragablzItem == null) return false;

    //    return true;
    //}

    //protected override DependencyObject GetContainerForItemOverride()
    //{
    //    var result = ContainerCustomisations != null && ContainerCustomisations.GetContainerForItemOverride != null
    //        ? ContainerCustomisations.GetContainerForItemOverride()
    //        : new DragablzItem();

    //    result.SizeChanged += ItemSizeChangedEventHandler;

    //    return result;
    //}

    //protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    //{
    //    if (ContainerCustomisations != null && ContainerCustomisations.PrepareContainerForItemOverride != null)
    //        ContainerCustomisations.PrepareContainerForItemOverride(element, item);

    //    base.PrepareContainerForItemOverride(element, item);
    //}

    #endregion

    #region Internal Methods

    internal IEnumerable<DragablzItem> DragablzItems() => this.Containers<DragablzItem>().ToList();

    internal void InstigateDrag(object item, Action<DragablzItem> continuation)
    {
        //var dragablzItem = (DragablzItem)ItemContainerGenerator.ContainerFromItem(item);

        //dragablzItem.InstigateDrag(continuation);
    }

    #endregion

    #region Private Methods

    private void ItemMouseDownWithinHandlerTarget(object? sender, DragablzItemEventArgs e)
    {
        if (ItemsOrganiser == null) return;

        var bounds = new Size(Bounds.Width, Bounds.Height);
        ItemsOrganiser.OrganiseOnMouseDownWithin(this, bounds,
            DragablzItems().Except(e.DragablzItem).ToList(),
            e.DragablzItem);
    }

    private void ItemDragStarted(object? sender, DragablzDragStartedEventArgs eventArgs)
    {
        if (ItemsOrganiser != null)
        {
            var bounds = new Size(Bounds.Width, Bounds.Height);
            ItemsOrganiser.OrganiseOnDragStarted(this, bounds,
                DragablzItems().Except(new[] { eventArgs.DragablzItem }).ToList(),
                eventArgs.DragablzItem);
        }

        eventArgs.Handled = true;

        //Dispatcher.BeginInvoke(new Action(InvalidateMeasure), DispatcherPriority.Loaded);
    }

    private void ItemDragCompleted(object? sender, DragablzDragCompletedEventArgs eventArgs)
    {
        var dragablzItems = DragablzItems()
            .Select(i =>
            {
                i.IsDragging = false;
                i.IsSiblingDragging = false;
                return i;
            })
            .ToList();

        if (ItemsOrganiser != null)
        {
            var bounds = new Size(Bounds.Width, Bounds.Height);
            ItemsOrganiser.OrganiseOnDragCompleted(this, bounds,
                dragablzItems.Except(eventArgs.DragablzItem),
                eventArgs.DragablzItem);
        }

        eventArgs.Handled = true;

        //wowsers
        //Dispatcher.BeginInvoke(new Action(InvalidateMeasure));
        //Dispatcher.BeginInvoke(new Action(InvalidateMeasure), DispatcherPriority.Loaded);
    }

    private void ItemDragDelta(object? sender, DragablzDragDeltaEventArgs eventArgs)
    {
        var bounds = new Size(ItemsPresenterWidth, ItemsPresenterHeight);
        var desiredLocation = new Point(
            eventArgs.DragablzItem.X + eventArgs.DragDeltaEventArgs.Vector.X,
            eventArgs.DragablzItem.Y + eventArgs.DragDeltaEventArgs.Vector.Y
        );
        if (ItemsOrganiser != null)
        {
            if (FixedItemCount > 0 &&
                ItemsOrganiser.Sort(DragablzItems()).Take(FixedItemCount).Contains(eventArgs.DragablzItem))
            {
                eventArgs.Handled = true;
                return;
            }

            desiredLocation = ItemsOrganiser.ConstrainLocation(this, bounds,
                new Point(eventArgs.DragablzItem.X, eventArgs.DragablzItem.Y),
                new Size(eventArgs.DragablzItem.Bounds.Width, eventArgs.DragablzItem.Bounds.Height),
                desiredLocation, eventArgs.DragablzItem.DesiredSize);
        }

        foreach (var dragableItem in DragablzItems()
                     .Except(new[] { eventArgs.DragablzItem })) // how about Linq.Where() ?
        {
            dragableItem.IsSiblingDragging = true;
        }
        eventArgs.DragablzItem.IsDragging = true;

        eventArgs.DragablzItem.X = desiredLocation.X;
        eventArgs.DragablzItem.Y = desiredLocation.Y;

        if (ItemsOrganiser != null)
            ItemsOrganiser.OrganiseOnDrag(
                this,
                bounds,
                DragablzItems().Except(new[] { eventArgs.DragablzItem }), eventArgs.DragablzItem);

        eventArgs.DragablzItem.BringIntoView();

        eventArgs.Handled = true;
    }
    
    private void ItemXChanged(object? sender, RoutedPropertyChangedEventArgs<double> e) => UpdateMonitor(e);

    private void ItemYChanged(object? sender, RoutedPropertyChangedEventArgs<double> e) => UpdateMonitor(e);

    private void UpdateMonitor(RoutedPropertyChangedEventArgs<double> e)
    {
        if (PositionMonitor == null) return;

        var dragablzItem = e.Source as DragablzItem;

        //if (!Equals(ItemsControlFromItemContainer(dragablzItem), this)) 
        //    return;

        PositionMonitor.OnLocationChanged(new LocationChangedEventArgs(dragablzItem.Content, 
            new Point(dragablzItem.X, dragablzItem.Y)));

        if (PositionMonitor is not StackPositionMonitor linearPositionMonitor) 
            return;

        var sortedItems = linearPositionMonitor
            .Sort(this.Containers<DragablzItem>())
            .Select(di => di.Content)
            .ToArray();

        if (_previousSortQueryResult == null || !_previousSortQueryResult.SequenceEqual(sortedItems))
            linearPositionMonitor.OnOrderChanged(
                new OrderChangedEventArgs(_previousSortQueryResult, sortedItems));

        _previousSortQueryResult = sortedItems;
    }

    /*

  private void ItemContainerGeneratorOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs)
  {
      //throw new NotImplementedException();
  }

  private void ItemContainerGeneratorOnStatusChanged(object sender, EventArgs eventArgs)
  {            
      if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) return;

      InvalidateMeasure();
      //extra kick
      Dispatcher.BeginInvoke(new Action(InvalidateMeasure), DispatcherPriority.Loaded);
  }
         

  
    private void ItemSizeChangedEventHandler(object sender, SizeChangedEventArgs e)
    {
        InvalidateMeasure();
        //extra kick
        Dispatcher.BeginInvoke(new Action(InvalidateMeasure), DispatcherPriority.Loaded);
    }
  
  */


    #endregion
}