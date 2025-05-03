using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SP2.ViewModels;

namespace SP2.Views;

public partial class Electricity : UserControl
{
    public Electricity()
    {
        InitializeComponent();
        
        // Set the DataContext to our ElectricityViewModel
        DataContext = new ElectricityViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
