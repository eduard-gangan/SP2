using System;
using System.Windows.Input;
using Avalonia.Controls;
using SP2.Views;

namespace SP2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static MainWindowViewModel _instance;
        private UserControl _currentPage;
        private int _activePageIndex = 0;
        private bool _isStartScreenActive = true;
        
        public bool IsStartScreenActive
        {
            get => _isStartScreenActive;
            set
            {
                if (_isStartScreenActive != value)
                {
                    _isStartScreenActive = value;
                    OnPropertyChanged(nameof(IsStartScreenActive));
                }
            }
        }
        
        public UserControl CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }
        
        public int ActivePageIndex
        {
            get => _activePageIndex;
            set
            {
                if (_activePageIndex != value)
                {
                    _activePageIndex = value;
                    OnPropertyChanged(nameof(ActivePageIndex));
                    // Update all button active states
                    OnPropertyChanged(nameof(IsPage1Active));
                    OnPropertyChanged(nameof(IsPage2Active));
                    OnPropertyChanged(nameof(IsPage3Active));
                    OnPropertyChanged(nameof(IsPage4Active));
                }
            }
        }
        
        // Properties for button active states
        public bool IsPage1Active => ActivePageIndex == 0;
        public bool IsPage2Active => ActivePageIndex == 1;
        public bool IsPage3Active => ActivePageIndex == 2;
        public bool IsPage4Active => ActivePageIndex == 3;

        public ICommand NavigateToPage1Command { get; }
        public ICommand NavigateToPage2Command { get; }
        public ICommand NavigateToPage3Command { get; }
        public ICommand NavigateToPage4Command { get; }

        public MainWindowViewModel()
        {
            _instance = this;
            
            // Initially show the start screen
            CurrentPage = new StartScreen();
            
            // Set up navigation commands
            NavigateToPage1Command = new RelayCommand(_ => NavigateToPage1());
            NavigateToPage2Command = new RelayCommand(_ => NavigateToPage2());
            NavigateToPage3Command = new RelayCommand(_ => NavigateToPage3());
            NavigateToPage4Command = new RelayCommand(_ => NavigateToPage4());
        }
        
        // Static method to navigate from start screen to main content
        public static void NavigateToMainContent()
        {
            if (_instance != null)
            {
                _instance.IsStartScreenActive = false;
                _instance.NavigateToPage1();
            }
        }

        private void NavigateToPage1()
        {
            CurrentPage = new SystemConfig();
            ActivePageIndex = 0;
        }

        private void NavigateToPage2()
        {
            CurrentPage = new Heat();
            ActivePageIndex = 1;
        }

        private void NavigateToPage3()
        {
            CurrentPage = new Electricity();
            ActivePageIndex = 2;
        }

        private void NavigateToPage4()
        {
            CurrentPage = new EconEnvironment();
            ActivePageIndex = 3;
        }
    }
}
