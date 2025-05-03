using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SP2.ViewModels;

namespace SP2.Views;

public partial class EconEnvironment : UserControl
{
    public EconEnvironment()
    {
        InitializeComponent();
        DataContext = new EconEnvironmentViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
