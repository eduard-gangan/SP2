using Avalonia.Controls;
using SP2.ViewModels;

namespace SP2;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
