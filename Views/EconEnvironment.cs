using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SP2.Views;

public partial class EconEnvironment : UserControl
{
    public EconEnvironment()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
