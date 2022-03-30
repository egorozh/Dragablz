using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonium.Events;

namespace Avalonium;

public class TabsControl : TabControl
{
    private DragTabItem _draggedItem;
    private int _prevZIndex;
    private List<DragTabItem> _items1;

    #region Internal Fields

    internal ItemsPresenter ItemsPresenter = null!;

    #endregion

    #region Avalonia Properties

    public static readonly StyledProperty<double> AdjacentHeaderItemOffsetProperty
        = AvaloniaProperty.Register<TabsControl, double>(nameof(AdjacentHeaderItemOffset), 8.0);


    #endregion

    #region Public Properties

    public double AdjacentHeaderItemOffset
    {
        get => GetValue(AdjacentHeaderItemOffsetProperty);
        set => SetValue(AdjacentHeaderItemOffsetProperty, value);
    }

    #endregion

    #region Constructor

    public TabsControl()
    {
        ItemsPanel = new FuncTemplate<IPanel>(() => new Canvas());
        AddHandler(DragTabItem.DragDelta, ItemDragDelta);
        AddHandler(DragTabItem.DragCompleted, ItemDragCompleted);
        AddHandler(DragTabItem.DragStarted, ItemDragStarted);
    }

    #endregion

    #region Protected Methods

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        ItemsPresenter = e.NameScope.Get<ItemsPresenter>("PART_ItemsPresenter");
    }

    protected override IItemContainerGenerator CreateItemContainerGenerator()
        => new DragTabItemContainerGenerator(this);

    protected override void OnContainersMaterialized(ItemContainerEventArgs e)
    {
        base.OnContainersMaterialized(e);

        if (e.Containers.FirstOrDefault() is {ContainerControl: DragTabItem exTabItem} info)
        {
            exTabItem.TabIndex = info.Index;
        }
    }

    #endregion

    #region Private Methods

    private void ItemDragStarted(object? sender, DragablzDragStartedEventArgs eventArgs)
    {
        _draggedItem = eventArgs.DragablzItem;

        _prevZIndex = _draggedItem.ZIndex;
        _draggedItem.ZIndex = int.MaxValue;
        _items1 = ItemsPresenter.Panel.Children.OfType<DragTabItem>().ToList();

        eventArgs.Handled = true;
    }
    
    private void ItemDragDelta(object? sender, DragablzDragDeltaEventArgs eventArgs)
    {
        var (dX, dY) = eventArgs.DragDeltaEventArgs.Vector;

        _items1 = ItemsPresenter.Panel.Children.OfType<DragTabItem>().ToList();

        var prevWidth = 0.0;

        for (var i = 0; i < _draggedItem.TabIndex; i++)
        {
            prevWidth += _items1[i].Bounds.Width;
        }

        Canvas.SetLeft(_draggedItem, prevWidth + dX - _draggedItem.GetOffset());
        //Canvas.SetTop(_tabItem, dY);

        var left = _draggedItem.Bounds.Left;
        var right = _draggedItem.Bounds.Right;

        if (_items1 != null)
            foreach (var tabItem in _items1)
            {
                if (ReferenceEquals(tabItem, _draggedItem))
                    continue;

                var center = tabItem.Bounds.Left + tabItem.Bounds.Width / 2;

                if (left >= center)
                {
                    //var index = tabItem.TabIndex;

                    ////tabItem turn on right
                    //this.TabIndex--;
                    //TabsControl.ItemsPresenter.Panel.Children.Move(index, index + 1);
                    //_items = TabsControl.ItemsPresenter.Panel.Children.OfType<ExTabItem>().ToList();

                    //tabItem.TabIndex = index + 1;
                    //tabItem.InitPosition();
                    //_isDrag = false;
                    //ZIndex = _prevZIndex;

                    tabItem.Background = Brushes.Red;
                    // break;
                }
                else if (right >= center)
                {
                    //tabItem turn on left

                    var index = tabItem.TabIndex;

                    if (index > 0)
                    {
                        _draggedItem.TabIndex++;
                        ItemsPresenter.Panel.Children.Move(index, index - 1);
                        _items1 = ItemsPresenter.Panel.Children.OfType<DragTabItem>().ToList();

                        tabItem.TabIndex = index - 1;
                        tabItem.InitPosition();
                        _draggedItem.ZIndex = _prevZIndex;
                        break;
                    }

                    tabItem.Background = Brushes.Blue;
                }
                else
                {
                    tabItem.Background = Background;
                }
            }


        eventArgs.Handled = true;
    }

    private void ItemDragCompleted(object? sender, DragablzDragCompletedEventArgs eventArgs)
    {
        _draggedItem.InitPosition();
        _draggedItem.ZIndex = _prevZIndex;

        eventArgs.Handled = true;
    }

    #endregion
}