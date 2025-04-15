using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SP2.ViewModels;

namespace SP2.Views;

public partial class Heat : UserControl
{
    public Heat()
    {
        InitializeComponent();
        
        // Set the DataContext to our HeatViewModel
        DataContext = new HeatViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
