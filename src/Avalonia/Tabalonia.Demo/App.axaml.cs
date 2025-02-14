using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Dragablz;

namespace Tabalonia.Demo;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;


        new QuickStartWindow().Show();

        new BasicExampleMainWindow
        {
            DataContext = new BasicExampleMainModel()
        }.Show();

        var boundExampleModel = new BoundExampleModel(
            new HeaderedItemViewModel
            {
                Header = "Fixed",
                Content = "There is a dragablz:DragablzItemsControl.FixedItemCount of 1, so this header is fixed!"
            },
            new HeaderedItemViewModel { Header = "MDI Demo", Content = new MdiExample() },
            new HeaderedItemViewModel
            {
                Header = "Layout Info",
                Content = new LayoutManagementExample { DataContext = new LayoutManagementExampleViewModel() }
            },
            new HeaderedItemViewModel
            {
                Header = new CustomHeaderViewModel { Header = "Header" },
                Content =
                    "This tab illustrates how an individual header can be customized, without having to change the DragablzItem tab header template."
            },
            new HeaderedItemViewModel { Header = "Tues", Content = "Tuesday's child is full of grace" } //,
                                                                                                        //new HeaderedItemViewModel { Header = "Wed", Content = "Wednesday's child is full of woe" }//,
                                                                                                        //new HeaderedItemViewModel { Header = "Thu", Content = "Thursday's child has far to go" },
                                                                                                        //new HeaderedItemViewModel { Header = "Fri", Content = "Friday's child loving and giving" }//,
                                                                                                        //new HeaderedItemViewModel { Header = "Sat", Content = "Saturday's child works hard for a living" },
                                                                                                        //new HeaderedItemViewModel { Header = "Sun", Content = "Sunday's child is awkwardly not fitting into this demo" }                 
        );
        boundExampleModel.ToolItems.Add(
            new HeaderedItemViewModel { Header = "January", Content = "Welcome to the January tool/float item." });
        boundExampleModel.ToolItems.Add(
            new HeaderedItemViewModel
            {
                Header = "July",
                Content =
                    new Border
                    {
                        Background = Brushes.Yellow,
                        Child = new TextBlock { Text = "Welcome to the July tool/float item." },
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    }
            });

        new BoundExampleWindow()
        {
            DataContext = boundExampleModel
        }.Show();


        base.OnFrameworkInitializationCompleted();
    }
}