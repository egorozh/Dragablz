using System.Collections.ObjectModel;

namespace Tabalonia.Demo;

public class TreeNode
{
    public object Content { get; set; }

    public ObservableCollection<TreeNode> Children { get; } = new();
}