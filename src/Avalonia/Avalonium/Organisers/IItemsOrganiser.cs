using Avalonia;

namespace Avalonium.Organisers;

public interface IItemsOrganiser
{
    void OrganiseOnDragStarted(TabsItemsPresenter requestor, Rect measureBounds, IEnumerable<DragTabItem> siblingItems,
        DragTabItem dragItem);

    void OrganiseOnDrag(TabsItemsPresenter requestor, Rect measureBounds, IEnumerable<DragTabItem> siblingItems,
        DragTabItem dragItem);

    void OrganiseOnDragCompleted( IEnumerable<DragTabItem> siblingsItems, DragTabItem dragItem);

    IEnumerable<DragTabItem> Sort(IEnumerable<DragTabItem> items);

    Point ConstrainLocation(TabsItemsPresenter requestor, Rect measureBounds, Point itemCurrentLocation,
        Rect itemCurrentSize, Point itemDesiredLocation, Size itemDesiredSize);

    void Organise(TabsItemsPresenter requestor, Size maxConstraint, IEnumerable<DragTabItem> dragablzItems);

    void Organise(TabsItemsPresenter requestor, Size maxConstraint, IOrderedEnumerable<DragTabItem> dragablzItems);

    Size Measure(TabsItemsPresenter requestor, Rect bounds, IEnumerable<DragTabItem> dragablzItems);
}