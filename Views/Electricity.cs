using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SP2.Views;

public partial class Electricity : UserControl
{
    public Electricity()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
