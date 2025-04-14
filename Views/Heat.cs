using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SP2.Views;

public partial class Heat : UserControl
{
    public Heat()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
