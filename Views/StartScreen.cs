using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SP2.ViewModels;

namespace SP2.Views;

public partial class StartScreen : UserControl
{
    public StartScreen()
    {
        InitializeComponent();
        DataContext = new StartScreenViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
