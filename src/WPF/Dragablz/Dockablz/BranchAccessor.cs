﻿using System;
using System.Linq;
using System.Windows;
using Dragablz.Core;

namespace Dragablz.Dockablz;

public class BranchAccessor
{
    public BranchAccessor(Branch branch)
    {
        Branch = branch ?? throw new ArgumentNullException(nameof(branch));

        var firstChildBranch = branch.FirstItem as Branch;
        if (firstChildBranch != null)
            FirstItemBranchAccessor = new BranchAccessor(firstChildBranch);
        else
            FirstItemTabablzControl = FindTabablzControl(branch.FirstItem, branch.FirstContentPresenter);

        var secondChildBranch = branch.SecondItem as Branch;            
        if (secondChildBranch != null)
            SecondItemBranchAccessor = new BranchAccessor(secondChildBranch);
        else
            SecondItemTabablzControl = FindTabablzControl(branch.SecondItem, branch.SecondContentPresenter);
    }

    private static TabablzControl FindTabablzControl(object item, DependencyObject contentPresenter)
    {
        var result = item as TabablzControl;
        return result ?? contentPresenter.VisualTreeDepthFirstTraversal().OfType<TabablzControl>().FirstOrDefault();
    }

    public Branch Branch { get; }

    public BranchAccessor FirstItemBranchAccessor { get; }

    public BranchAccessor SecondItemBranchAccessor { get; }

    public TabablzControl FirstItemTabablzControl { get; }

    public TabablzControl SecondItemTabablzControl { get; }

    /// <summary>
    /// Visits the content of the first or second side of a branch, according to its content type.  No more than one of the provided <see cref="Action"/>
    /// callbacks will be called.  
    /// </summary>
    /// <param name="childItem"></param>
    /// <param name="childBranchVisitor"></param>
    /// <param name="childTabablzControlVisitor"></param>
    /// <param name="childContentVisitor"></param>
    /// <returns></returns>
    public BranchAccessor Visit(BranchItem childItem,
        Action<BranchAccessor> childBranchVisitor = null,
        Action<TabablzControl> childTabablzControlVisitor = null,
        Action<object> childContentVisitor = null)
    {
        Func<BranchAccessor> branchGetter;
        Func<TabablzControl> tabGetter;
        Func<object> contentGetter;

        switch (childItem)
        {
            case BranchItem.First:
                branchGetter = () => FirstItemBranchAccessor;
                tabGetter = () => FirstItemTabablzControl;
                contentGetter = () => Branch.FirstItem;
                break;
            case BranchItem.Second:
                branchGetter = () => SecondItemBranchAccessor;
                tabGetter = () => SecondItemTabablzControl;
                contentGetter = () => Branch.SecondItem;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(childItem));
        }

        var branchDescription = branchGetter();
        if (branchDescription != null)
        {
            if (childBranchVisitor != null)
                childBranchVisitor(branchDescription);
            return this;
        }
            
        var tabablzControl = tabGetter();
        if (tabablzControl != null)
        {
            if (childTabablzControlVisitor != null)
                childTabablzControlVisitor(tabablzControl);

            return this;
        }

        if (childContentVisitor == null) return this;

        var content = contentGetter();
        if (content != null)
            childContentVisitor(content);

        return this;
    }
}