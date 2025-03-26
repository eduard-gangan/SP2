using System;
using System.IO;
using SP2.Services;

namespace SP2.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            // Run the optimization tests when the program starts
            OptimizerTests.RunTest();
        }
    }
}
