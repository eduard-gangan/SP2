using Avalonia;
using SP2.Models;
using SP2.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SP2;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        //ignoring co2 emission
        //1,717,833.8199999998
        //927,458.89999999991

        //considering co2 emissions below a certain electricity price range
        //below 700
        //1,661,440.0399999998
        //652,385.8

        //below 800
        //1,665,591.159999999
        //510,935.89999999997

        //below 900
        //1,688,487.0799999994
        //424,070.8

        //below 880.01 (average electricity price)
        //1,679,298.2399999993
        //451,195.8

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
