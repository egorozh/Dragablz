using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Dragablz.Dockablz;
using Tabalonia.Dockablz;

namespace Tabalonia.Demo;

public class LayoutManagementExampleViewModel
{
    private readonly TreeNode _rootNode;

    public LayoutManagementExampleViewModel()
    {
        QueryLayoutsCommand = new AnotherCommandImplementation(x => QueryLayouts());
        _rootNode = new TreeNode
        {
            Content = "Application"
        };
    }

    public ICommand QueryLayoutsCommand { get; }

    public IEnumerable<TreeNode> RootNodes
    {
        get { return new[] {_rootNode}; }
    }

    private void QueryLayouts()
    {
        _rootNode.Children.Clear();

        if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime lifetime)
            return;

        foreach (var layout in lifetime.Windows.OfType<BoundExampleWindow>().Select(w => w.RootLayout))
        {
            var layoutAccessor = layout.Query();
            var layoutNode = new TreeNode
            {
                Content = "Layout"
            };
            _rootNode.Children.Add(layoutNode);

            FloatingItemsVisitor(layoutNode, layoutAccessor);
            layoutAccessor.Visit(layoutNode, BranchAccessorVisitor, TabablzControlVisitor);
        }
    }

    private static void FloatingItemsVisitor(TreeNode layoutNode, LayoutAccessor layoutAccessor)
    {
        var floatingItems = layoutAccessor.FloatingItems.ToList();
        var floatingItemsNode = new TreeNode {Content = "Floating Items " + floatingItems.Count};
        foreach (var floatingItemNode in floatingItems.Select(floatingItem => new TreeNode
                 {
                     Content =
                         $"Floating Item {floatingItem.X}, {floatingItem.Y} : {floatingItem.Bounds.Width}, {floatingItem.Bounds.Height}"
                 }))
        {
            floatingItemsNode.Children.Add(floatingItemNode);
        }

        layoutNode.Children.Add(floatingItemsNode);
    }

    private static void TabablzControlVisitor(TreeNode treeNode, TabablzControl tabablzControl)
    {
        treeNode.Children.Add(new TreeNode {Content = new TabablzControlProxy(tabablzControl)});
    }

    private static void BranchAccessorVisitor(TreeNode treeNode, BranchAccessor branchAccessor)
    {
        var branchNode = new TreeNode {Content = "Branch " + branchAccessor.Branch.Orientation};
        treeNode.Children.Add(branchNode);

        var firstBranchNode = new TreeNode
            {Content = "Branch Item 1. Ratio=" + branchAccessor.Branch.GetFirstProportion()};
        branchNode.Children.Add(firstBranchNode);
        var secondBranchNode = new TreeNode
            {Content = "Branch Item 2. Ratio=" + (1 - branchAccessor.Branch.GetFirstProportion())};
        branchNode.Children.Add(secondBranchNode);

        branchAccessor
            .Visit(firstBranchNode, BranchItem.First, BranchAccessorVisitor, TabablzControlVisitor)
            .Visit(secondBranchNode, BranchItem.Second, BranchAccessorVisitor, TabablzControlVisitor);
    }
}