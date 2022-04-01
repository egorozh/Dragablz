using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Avalonium.Organisers;

public abstract class StackOrganiser : IItemsOrganiser
{
    #region Private Fields

    private readonly Orientation _orientation;
    private readonly double _itemOffset;
    private readonly Func<DragTabItem, double> _getDesiredSize;
    private readonly Func<DragTabItem, double> _getLocation;
    private readonly Action<DragTabItem, double> _setLocation;
    private readonly AvaloniaProperty _canvasProperty;

    private readonly Dictionary<DragTabItem, double> _activeStoryboardTargetLocations = new();
    private IDictionary<DragTabItem, LocationInfo>? _siblingItemLocationOnDragStart;

    #endregion

    #region Constructor

    protected StackOrganiser(Orientation orientation, double itemOffset = 0)
    {
        _orientation = orientation;
        _itemOffset = itemOffset;

        _canvasProperty = orientation == Orientation.Horizontal ? Canvas.LeftProperty : Canvas.TopProperty;

        if (orientation == Orientation.Horizontal)
        {
            _getDesiredSize = item => item.DesiredSize.Width;
            _getLocation = item => item.X;
            _setLocation = (item, coord) =>
            {
                item.SetValue(DragTabItem.XProperty, coord);
            };
        }
        else
        {
            _getDesiredSize = item => item.DesiredSize.Height;
            _getLocation = item => item.Y;
            _setLocation = (item, coord) =>
            {
                item.SetValue(DragTabItem.YProperty, coord);
            };
        }
    }

    #endregion
    
    #region LocationInfo

    private class LocationInfo
    {
        public LocationInfo(DragTabItem item, double start, double mid, double end)
        {
            Item = item;
            Start = start;
            Mid = mid;
            End = end;
        }

        public double Start { get; }

        public double Mid { get; }

        public double End { get; }

        public DragTabItem Item { get; }
    }

    #endregion
    
    public virtual void Organise(TabsItemsPresenter requestor, Size measureBounds, IEnumerable<DragTabItem> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        OrganiseInternal(
            requestor, 
            measureBounds,
            items.Select((di, idx) => new Tuple<int, DragTabItem>(idx, di))
                .OrderBy(tuple => tuple,
                    MultiComparer<Tuple<int, DragTabItem>>.Ascending(tuple => _getLocation(tuple.Item2))
                        .ThenAscending(tuple => tuple.Item1))
                .Select(tuple => tuple.Item2));            
    }

    public virtual void Organise(TabsItemsPresenter requestor, Size measureBounds, IOrderedEnumerable<DragTabItem> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        OrganiseInternal(requestor, measureBounds, items);
    }

    private void OrganiseInternal(TabsItemsPresenter requestor, Size measureBounds,
        IEnumerable<DragTabItem> items)
    {
        var currentCoord = 0.0;
        var z = int.MaxValue;
        var logicalIndex = 0;
        foreach (var newItem in items)
        {
            newItem.ZIndex = newItem.IsSelected ? int.MaxValue : --z;
            SetLocation(newItem, currentCoord);
            newItem.LogicalIndex = logicalIndex++;
            newItem.Measure(measureBounds);
            var desiredSize = _getDesiredSize(newItem);
            if (desiredSize == 0.0) desiredSize = 1.0; //no measure? create something to help sorting
            currentCoord += desiredSize + _itemOffset;
        }
    }
    
    public virtual void OrganiseOnDragStarted(TabsItemsPresenter requestor, Rect measureBounds, IEnumerable<DragTabItem> siblingItems, DragTabItem dragItem)
    {
        if (siblingItems == null) throw new ArgumentNullException(nameof(siblingItems));
        if (dragItem == null) throw new ArgumentNullException(nameof(dragItem));

        _siblingItemLocationOnDragStart = siblingItems.Select(GetLocationInfo).ToDictionary(loc => loc.Item);
    }

    public virtual void OrganiseOnDrag(TabsItemsPresenter requestor, Rect measureBounds,
        IEnumerable<DragTabItem> siblingItems, DragTabItem dragItem)
    {
        if (siblingItems == null) throw new ArgumentNullException(nameof(siblingItems));
        if (dragItem == null) throw new ArgumentNullException(nameof(dragItem));

        var currentLocations = siblingItems
            .Select(GetLocationInfo)
            .Union(new[] {GetLocationInfo(dragItem)})
            .OrderBy(loc => loc.Item == dragItem ? loc.Start : _siblingItemLocationOnDragStart[loc.Item].Mid);

        var currentCoord = 0.0;
        var zIndex = int.MaxValue;
        foreach (var location in currentLocations)
        {
            if (!Equals(location.Item, dragItem))
            {
                SendToLocation(location.Item, currentCoord);
                location.Item.ZIndex = --zIndex;
            }
            currentCoord += _getDesiredSize(location.Item) + _itemOffset;                
        }

        dragItem.ZIndex = int.MaxValue;
    }

    public virtual void OrganiseOnDragCompleted(IEnumerable<DragTabItem> siblingItems, DragTabItem dragItem)
    {
        if (siblingItems == null) throw new ArgumentNullException(nameof(siblingItems));

        var currentLocations = siblingItems
            .Select(GetLocationInfo)
            .Union(new[] {GetLocationInfo(dragItem)})
            .OrderBy(loc => loc.Item == dragItem ? loc.Start : _siblingItemLocationOnDragStart[loc.Item].Mid);
//todo fix
        var currentCoord = 0.0;
        var z = int.MaxValue;
        var logicalIndex = 0;
        foreach (var location in currentLocations)
        {
            SetLocation(location.Item, currentCoord);
            currentCoord += _getDesiredSize(location.Item) + _itemOffset;
            location.Item.ZIndex = --z;
            location.Item.LogicalIndex = logicalIndex++;
        }

        dragItem.ZIndex = int.MaxValue;
    }

    public virtual Point ConstrainLocation(TabsItemsPresenter requestor, Rect measureBounds, Point itemCurrentLocation,
        Rect itemCurrentSize, Point itemDesiredLocation, Size itemDesiredSize)
    {
        var fixedItems = requestor.FixedItemCount;
        var lowerBound = fixedItems == 0
            ? -1d
            : GetLocationInfo(requestor.DragablzItems()
                .Take(fixedItems)
                .Last()).End + _itemOffset - 1;

        return new Point(
            _orientation == Orientation.Vertical
                ? 0
                : Math.Min(Math.Max(lowerBound, itemDesiredLocation.X), (measureBounds.Width) + 1),
            _orientation == Orientation.Horizontal
                ? 0
                : Math.Min(Math.Max(lowerBound, itemDesiredLocation.Y), (measureBounds.Height) + 1)
        );
    }

    public virtual Size Measure(TabsItemsPresenter requestor, Rect availableSize, IEnumerable<DragTabItem> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        var size = new Size(double.PositiveInfinity, double.PositiveInfinity);

        double width = 0, height = 0;
        var isFirst = true;

        foreach (var dragTabItem in items)
        {
            dragTabItem.Measure(size);

            //var loaded = dragTabItem.IsLoaded
            var loaded = true;

            if (_orientation == Orientation.Horizontal)
            {
                height = Math.Max(height,
                    !loaded ? dragTabItem.DesiredSize.Height : dragTabItem.Bounds.Height);
                width += !loaded ? dragTabItem.DesiredSize.Width : dragTabItem.Bounds.Width;
                if (!isFirst)
                    width += _itemOffset;
            }
            else
            {
                width = Math.Max(width,
                    !loaded ? dragTabItem.DesiredSize.Width : dragTabItem.Bounds.Width);
                height += !loaded ? dragTabItem.DesiredSize.Height : dragTabItem.Bounds.Height;
                if (!isFirst)
                    height += _itemOffset;
            }

            isFirst = false;
        }

        return new Size(Math.Max(width, 0), Math.Max(height, 0));
    }

    public virtual IEnumerable<DragTabItem> Sort(IEnumerable<DragTabItem> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        return items.OrderBy(i => GetLocationInfo(i).Start);
    }

    private void SetLocation(DragTabItem DragTabItem, double location)
    {                     
        _setLocation(DragTabItem, location);
    }
        
    private void SendToLocation(DragTabItem dragTabItem, double location)
    {
        if (Math.Abs(_getLocation(dragTabItem) - location) < 1.0
            ||
            _activeStoryboardTargetLocations.TryGetValue(dragTabItem, out var activeTarget)
            && Math.Abs(activeTarget - location) < 1.0)
        {             
            return;
        }            

        _activeStoryboardTargetLocations[dragTabItem] = location;

        
        _setLocation(dragTabItem, location);

        //var storyboard = new Storyboard {FillBehavior = FillBehavior.Stop};
        //storyboard.WhenComplete(sb =>
        //{
        //    _setLocation(DragTabItem, location);
        //    sb.Remove(DragTabItem);
        //    _activeStoryboardTargetLocations.Remove(DragTabItem);
        //});

        //var timeline = new DoubleAnimationUsingKeyFrames();
        //timeline.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(_canvasDependencyProperty));
        //timeline.KeyFrames.Add(
        //    new EasingDoubleKeyFrame(location, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)))
        //    {
        //        EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        //    });
        //storyboard.Children.Add(timeline);            
        //storyboard.Begin(DragTabItem, true);            
    }

    private LocationInfo GetLocationInfo(DragTabItem item)
    {
        var size = _getDesiredSize(item);
        if (!_activeStoryboardTargetLocations.TryGetValue(item, out var startLocation))
            startLocation = _getLocation(item);
        var midLocation = startLocation + size / 2;
        var endLocation = startLocation + size;

        return new LocationInfo(item, startLocation, midLocation, endLocation);
    }
}