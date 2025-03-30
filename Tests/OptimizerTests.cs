using System;
using System.Collections.Generic;
using SP2.Models;
using SP2.Services;

namespace SP2.Tests
{
    public class OptimizerTests
    {
        public static void RunTest()
        {
            var electricityPrices = new Dictionary<DateTime, double>
            {
                { DateTime.Now, 50.0 }
            };

            var units = AssetManager.GetProdUnits();

            var optimizer = new Optimiser(units, electricityPrices);

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

        private static void PrintResults(List<(ProductionUnit Unit, double Utilization)> result)
        {
            Console.WriteLine("\nOptimization Results:");
            Console.WriteLine("Unit Utilization:");
            foreach (var (unit, utilization) in result)
            {
                Console.WriteLine($"{unit.Name}: {utilization:P2} ({unit.MaxHeat * utilization:F2} MW)");
            }
        }
    }
} 