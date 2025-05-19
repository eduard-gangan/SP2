using System.Windows.Input;

namespace SP2.ViewModels;

public class StartScreenViewModel : ViewModelBase
{
    public ICommand ContinueCommand { get; }

    public StartScreenViewModel()
    {
        ContinueCommand = new RelayCommand(ExecuteContinue);
    }

    private void ExecuteContinue(object parameter)
    {
        // This will be handled by the MainWindowViewModel
        MainWindowViewModel.NavigateToMainContent();
    }
}
