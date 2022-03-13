using System.Collections.ObjectModel;

namespace DragablzDemo;

public class TreeNode
{
    public object Content { get; set; }

    public ObservableCollection<TreeNode> Children { get; } = new ObservableCollection<TreeNode>();
}