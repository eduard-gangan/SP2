using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SP2.Views;

public partial class SystemConfig : UserControl
{
    public SystemConfig()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
