using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Layout;
using Dragablz;
using Tabalonia.Dockablz;

namespace Tabalonia.Demo;

public class TabablzControlProxy : INotifyPropertyChanged
{
    private readonly TabablzControl _tabablzControl;
    private double _splitRatio;

    public TabablzControlProxy(TabablzControl tabablzControl)
    {
        _tabablzControl = tabablzControl;

        SplitHorizontallyCommand = new AnotherCommandImplementation(_ => Branch(Orientation.Horizontal));
        SplitVerticallyCommand = new AnotherCommandImplementation(_ => Branch(Orientation.Vertical));
        SplitRatio = 5;
    }

    public ICommand SplitHorizontallyCommand { get; }

    public ICommand SplitVerticallyCommand { get; }

    public double SplitRatio
    {
        get => _splitRatio;
        set
        {
            _splitRatio = value;
            OnPropertyChanged("SplitRatio");
        }
    }

    private void Branch(Orientation orientation)
    {
        var branchResult = Layout.Branch(_tabablzControl, orientation, false, SplitRatio/10);

        var newItem = new HeaderedItemViewModel
        {
            Header = "Code-Wise",
            Content = "This item was added in via code, using Layout.Branch, and TabablzControl.AddToSource"
        };

        branchResult.TabablzControl.AddToSource(newItem);
        branchResult.TabablzControl.SelectedItem = newItem;
    }

    public event PropertyChangedEventHandler PropertyChanged;
        
    protected virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
}