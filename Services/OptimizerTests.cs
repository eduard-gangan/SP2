using System;
using SP2.Models;
using SP2.Services;

namespace SP2.Services
{
    public class OptimizerTests
    {
        public static void RunTest()
        {
            // Get production units using static AssetManager
            var units = AssetManager.GetProdUnits();

            // Create optimizer with default weights
            var optimizer = new Optimiser(units);

            // Test with different heat demands
            Console.WriteLine("Testing with 5 MW demand:");
            var result5MW = optimizer.Optimise(5.0);
            PrintResults(result5MW);

            Console.WriteLine("\nTesting with 10 MW demand:");
            var result10MW = optimizer.Optimise(10.0);
            PrintResults(result10MW);

            Console.WriteLine("\nTesting with 15 MW demand:");
            var result15MW = optimizer.Optimise(15.0);
            PrintResults(result15MW);
        }

        private static void PrintResults(Optimiser.OptimisationResult result)
        {
            Console.WriteLine("\nOptimization Results:");
            Console.WriteLine($"Total Heat Output: {result.TotalHeatOutput:F2} MW");
            Console.WriteLine($"Total Cost: {result.TotalCost:F2} EUR/MW");
            Console.WriteLine($"Total CO2 Emissions: {result.TotalEmissions:F2} kg/MW");
            Console.WriteLine($"Overall Efficiency: {result.TotalEfficiency:F2}");
            
            Console.WriteLine("\nUnit Utilization:");
            foreach (var (unit, utilization) in result.UnitUtilizations)
            {
                Console.WriteLine($"{unit.Name}: {utilization:P2} ({unit.MaxHeat * utilization:F2} MW)");
            }
        }
    }
} 