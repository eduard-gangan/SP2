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
        Optimiser.OptimizeScenario1();
        var result = ResultDataManager.GetWinterOptimizedData();
        foreach (var data in result)
        {
            Debug.WriteLine($"Time: {data.TimeFrom} - {data.TimeTo}, Heat Production: {data.HeatProduction}, Expenses: {data.Expenses}, {data.CO2Emissions}, {data.GasConsumption}, {data.OilConsumption}");
        }
        Debug.WriteLine(result.Count());

        var result2 = ResultDataManager.GetSummerOptimizedData();
        foreach (var data in result2)
        {
            Debug.WriteLine($"Time: {data.TimeFrom} - {data.TimeTo}, Heat Production: {data.HeatProduction}, Expenses: {data.Expenses}, {data.CO2Emissions}, {data.GasConsumption}, {data.OilConsumption}");
        }
        Debug.WriteLine(result2.Count());

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
