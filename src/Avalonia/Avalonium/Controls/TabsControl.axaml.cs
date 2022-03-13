using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Avalonium;

public class TabsControl : TabControl
{
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
    }

    #endregion

    #region Protected Methods

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        ItemsPresenter = e.NameScope.Get<ItemsPresenter>("PART_ItemsPresenter");
    }

    protected override IItemContainerGenerator CreateItemContainerGenerator()
        => new ExTabItemContainerGenerator(this);

    protected override void OnContainersMaterialized(ItemContainerEventArgs e)
    {
        base.OnContainersMaterialized(e);

        if (e.Containers.FirstOrDefault() is { ContainerControl: ExTabItem exTabItem } info)
        {
            exTabItem.TabIndex = info.Index;
        }
    }

    #endregion
}