using System;
using System.Collections.Generic;
using System.Linq;
using SP2.Services;
using SP2.Models;

namespace SP2
{
    public class OptimizerTest
    {
        public static void RunScenario1()
        {
            Console.WriteLine("Starting optimization test...");
            
            // Store results for validation
            var winterResults = new List<ResultData>();
            var summerResults = new List<ResultData>();
            
            // Run the optimization
            Optimiser.OptimizeScenario1();
            
            // Get results from ResultDataManager
            winterResults = ResultDataManager.GetWinterOptimizedData();
            summerResults = ResultDataManager.GetSummerOptimizedData();
            
            // Validate results
            Console.WriteLine("\n=== Validation Results ===");
            
            // Check if we got both winter and summer results
            Console.WriteLine($"\nWinter periods processed: {winterResults?.Count}");
            Console.WriteLine($"Summer periods processed: {summerResults?.Count}");
            
            if (winterResults.Any() && summerResults.Any())
            {
                // Validate a winter period
                var winterSample = winterResults.First();
                Console.WriteLine("\nValidating winter period sample:");
                ValidateResultData(winterSample, isWinter: true);
                
                // Validate a summer period
                var summerSample = summerResults.First();
                Console.WriteLine("\nValidating summer period sample:");
                ValidateResultData(summerSample, isWinter: false);
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        public static void RunScenario2()
        {
            Console.WriteLine("Starting Scenario 2 optimization test...");
            
            // Store results for validation
            var winterResults = new List<ResultData>();
            var summerResults = new List<ResultData>();
            
            // Run the optimization
            Optimiser.OptimizeScenario2();
            
            // Get results from ResultDataManager
            winterResults = ResultDataManager.GetWinterOptimizedData();
            summerResults = ResultDataManager.GetSummerOptimizedData();
            
            // Validate results
            Console.WriteLine("\n=== Validation Results ===");
            
            // Check if we got both winter and summer results
            Console.WriteLine($"\nWinter periods processed: {winterResults?.Count}");
            Console.WriteLine($"Summer periods processed: {summerResults?.Count}");
            
            if (winterResults.Any() && summerResults.Any())
            {
                // Validate a winter period
                var winterSample = winterResults.First();
                Console.WriteLine("\nValidating winter period sample:");
                ValidateResultData(winterSample, isWinter: true);
                
                // Validate a summer period
                var summerSample = summerResults.First();
                Console.WriteLine("\nValidating summer period sample:");
                ValidateResultData(summerSample, isWinter: false);

                // Additional Scenario 2 specific validations
                Console.WriteLine("\nValidating Scenario 2 specific aspects:");
                ValidateScenario2Specifics(winterSample, summerSample);
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
        private static void ValidateResultData(ResultData result, bool isWinter)
        {
            // Check data types and basic value constraints
            Console.WriteLine($"Time period: {result.TimeFrom} to {result.TimeTo}");
            
            // Heat production should match demand and be positive
            Console.WriteLine($"Heat production: {result.HeatProduction} MW");
            if (result.HeatProduction <= 0)
                Console.WriteLine("WARNING: Heat production should be positive");
            
            // Expenses should be positive
            Console.WriteLine($"Expenses: {result.Expenses} DKK");
            if (result.Expenses <= 0)
                Console.WriteLine("WARNING: Expenses should be positive");
            
            // Consumption values should be non-negative
            Console.WriteLine($"Gas consumption: {result.GasConsumption} MWh");
            if (result.GasConsumption < 0)
                Console.WriteLine("WARNING: Gas consumption should not be negative");
            
            Console.WriteLine($"Oil consumption: {result.OilConsumption} MWh");
            if (result.OilConsumption < 0)
                Console.WriteLine("WARNING: Oil consumption should not be negative");
            
            // CO2 emissions should be positive
            Console.WriteLine($"CO2 emissions: {result.CO2Emissions} tons");
            if (result.CO2Emissions <= 0)
                Console.WriteLine("WARNING: CO2 emissions should be positive");
            
            // Check if units were used
            Console.WriteLine($"Number of units used: {result.ProductionUnitsUsed.Count}");
            if (result.ProductionUnitsUsed.Count == 0)
                Console.WriteLine("WARNING: No production units were used");
            
            // Validate each unit used
            foreach (var unit in result.ProductionUnitsUsed)
            {
                Console.WriteLine($"\nValidating unit: {unit.Name}");
                Console.WriteLine($"Type: {unit.Type}");
                Console.WriteLine($"Fuel type: {unit.FuelType}");
                Console.WriteLine($"Max heat: {unit.MaxHeat} MW");
                
                // Check unit properties
                if (unit.MaxHeat <= 0)
                    Console.WriteLine("WARNING: Unit max heat should be positive");
                if (unit.ProductionCosts <= 0)
                    Console.WriteLine("WARNING: Unit production costs should be positive");
                if (unit.CO2Emissions <= 0)
                    Console.WriteLine("WARNING: Unit CO2 emissions should be positive");
                if (unit.FuelConsumption <= 0)
                    Console.WriteLine("WARNING: Unit fuel consumption should be positive");
            }
            
            // Check if the results make sense for the season
            if (isWinter)
            {
                // Winter typically has higher heat demand
                if (result.HeatProduction < 1.0)
                    Console.WriteLine("WARNING: Winter heat production seems unusually low");
            }
            else
            {
                // Summer typically has lower heat demand
                if (result.HeatProduction > 10.0)
                    Console.WriteLine("WARNING: Summer heat production seems unusually high");
            }
        }

        private static void ValidateScenario2Specifics(ResultData winterSample, ResultData summerSample)
        {
            Console.WriteLine("\nValidating electricity-related aspects:");
            
            // Check electricity production
            Console.WriteLine($"Winter electricity production: {winterSample.ElectricityProduction} MWh");
            Console.WriteLine($"Summer electricity production: {summerSample.ElectricityProduction} MWh");
            
            // Check electricity consumption
            Console.WriteLine($"Winter electricity consumption: {winterSample.ElectricityConsumption} MWh");
            Console.WriteLine($"Summer electricity consumption: {summerSample.ElectricityConsumption} MWh");
            
            // Check profit
            Console.WriteLine($"Winter profit: {winterSample.Profit} DKK");
            Console.WriteLine($"Summer profit: {summerSample.Profit} DKK");

            // Validate that electricity-producing units are used
            bool hasElectricityProducingUnits = winterSample.ProductionUnitsUsed.Any(u => u.Type == UnitType.ElectricityProducing) ||
                                              summerSample.ProductionUnitsUsed.Any(u => u.Type == UnitType.ElectricityProducing);
            Console.WriteLine($"\nElectricity-producing units used: {hasElectricityProducingUnits}");

            // Validate that electricity-consuming units are used
            bool hasElectricityConsumingUnits = winterSample.ProductionUnitsUsed.Any(u => u.Type == UnitType.ElectricityConsuming) ||
                                              summerSample.ProductionUnitsUsed.Any(u => u.Type == UnitType.ElectricityConsuming);
            Console.WriteLine($"Electricity-consuming units used: {hasElectricityConsumingUnits}");

            // Check if the optimization is considering electricity prices
            if (winterSample.Profit != 0 || summerSample.Profit != 0)
            {
                Console.WriteLine("\nElectricity price consideration:");
                Console.WriteLine("- Profit values indicate electricity price consideration");
                Console.WriteLine("- Different profit values between winter and summer suggest seasonal price differences");
            }
        }
    }
} 