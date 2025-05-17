using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SkiaSharp;
using SP2.Models;
using SP2.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SP2.Services
{
    public static class Optimiser
    {
        // private static readonly List<TimeSeriesData> CSVData = SourceDataManager.LoadData("../../../Assets/2025 Heat Production Optimization - Danfoss Deliveries - Source Data Manager.csv");
        private static readonly List<TimeSeriesData> CSVData = SourceDataManager.LoadData("/Users/davidskorepa/Desktop/ProjectWork/SP2/Assets/2025 Heat Production Optimization - Danfoss Deliveries - Source Data Manager.csv");
        private static readonly List<ProductionUnit> ProductionUnits1 = AssetManager.GetProdUnits();
        private static readonly List<ProductionUnit> ProductionUnits2 = AssetManager.GetProdUnits();

        public static void OptimizeScenario1()
        {
            Console.WriteLine("\n=== Starting Scenario 1 Optimization ===");
            
            if (CSVData == null || !CSVData.Any())
            {
                Console.WriteLine("ERROR: No CSV data available!");
                throw new InvalidOperationException("No CSV data available for optimization. Please ensure the CSV file exists and contains valid data.");
            }

            Console.WriteLine($"Loaded {CSVData.Count} time series data points");
            Console.WriteLine($"Available production units: {ProductionUnits1.Count}");
            
            bool isWinter = true;
            
            foreach (var data in CSVData)
            {
                Console.WriteLine($"\nProcessing time period: {data.TimeFrom} to {data.TimeTo}");
                Console.WriteLine($"Heat demand: {data.HeatDemand} MW");
                
                double TargetHeatDemand = 0;
                double Expenses = 0;
                double GasConsumption = 0;
                double OilConsumption = 0;
                double CO2Emissions = 0;
                ResultData result = new ResultData();
                List<ProductionUnit> ProductionUnitsUsed = new List<ProductionUnit>();

                TargetHeatDemand = data.HeatDemand;
                int UnitIndex = 0;
                while (TargetHeatDemand > 0)
                {
                    ProductionUnit productionUnit = ProductionUnits1[UnitIndex];
                    Console.WriteLine($"\nUsing unit: {productionUnit.Name}");
                    Console.WriteLine($"Unit type: {productionUnit.Type}, Fuel: {productionUnit.FuelType}");
                    Console.WriteLine($"Max heat: {productionUnit.MaxHeat} MW");
                    
                    TargetHeatDemand -= productionUnit.MaxHeat;
                    if (TargetHeatDemand > 0)
                    {
                        Expenses += productionUnit.ProductionCosts * productionUnit.MaxHeat;
                        CO2Emissions += productionUnit.CO2Emissions * productionUnit.MaxHeat;
                        if (productionUnit.FuelType == "Gas")
                        {
                            GasConsumption += productionUnit.MaxHeat * productionUnit.FuelConsumption;
                        }
                        else
                        {
                            OilConsumption += productionUnit.MaxHeat * productionUnit.FuelConsumption;
                        }
                        Console.WriteLine($"Unit running at full capacity. Remaining demand: {TargetHeatDemand} MW");
                    }
                    else
                    {
                        double temporaryHeat = productionUnit.MaxHeat + TargetHeatDemand;
                        Expenses += productionUnit.ProductionCosts * temporaryHeat;
                        CO2Emissions += productionUnit.CO2Emissions * temporaryHeat;
                        if (productionUnit.FuelType == "Gas")
                        {
                            GasConsumption += temporaryHeat * productionUnit.FuelConsumption;
                        }
                        else
                        {
                            OilConsumption += temporaryHeat * productionUnit.FuelConsumption;
                        }
                        Console.WriteLine($"Unit running at partial capacity: {temporaryHeat} MW");
                    }
                    ProductionUnitsUsed.Add(productionUnit);
                    UnitIndex++;
                }
                
                result.HeatProduction = data.HeatDemand;
                result.ElectricityProduction = 0;
                result.ElectricityConsumption = 0;
                result.Expenses = Math.Round(Expenses, 2);
                result.Profit = 0;
                result.GasConsumption = Math.Round(GasConsumption, 2);
                result.OilConsumption = Math.Round(OilConsumption, 2);
                result.CO2Emissions = Math.Round(CO2Emissions, 2);
                result.ProductionUnitsUsed = ProductionUnitsUsed;
                result.TimeFrom = data.TimeFrom;
                result.TimeTo = data.TimeTo;

                Console.WriteLine("\nResults for this period:");
                Console.WriteLine($"Total expenses: {result.Expenses} DKK");
                Console.WriteLine($"Gas consumption: {result.GasConsumption} MWh");
                Console.WriteLine($"Oil consumption: {result.OilConsumption} MWh");
                Console.WriteLine($"CO2 emissions: {result.CO2Emissions} tons");
                Console.WriteLine($"Units used: {string.Join(", ", result.ProductionUnitsUsed.Select(u => u.Name))}");

                if (isWinter)
                {
                    ResultDataManager.SetWinterOptimizedData(result);
                    Console.WriteLine("Saved as winter data");
                }
                else
                {
                    ResultDataManager.SetSummerOptimizedData(result);
                    Console.WriteLine("Saved as summer data");
                }

                isWinter = !isWinter;
            }
            Console.WriteLine("\n=== Scenario 1 Optimization Complete ===");
        }

        public static void OptimizeScenario2()
        {
            Console.WriteLine("\n=== Starting Scenario 2 Optimization ===");
            
            bool isSummer = true;
            Dictionary<string, double> netCosts = new Dictionary<string, double>();
            List<ProductionUnit> productionUnits = ProductionUnits2;

            Console.WriteLine($"Available production units: {productionUnits.Count}");

            foreach (var data in CSVData)
            {
                Console.WriteLine($"\nProcessing time period: {data.TimeFrom} to {data.TimeTo}");
                Console.WriteLine($"Heat demand: {data.HeatDemand} MW");
                Console.WriteLine($"Electricity price: {data.ElectricityPrice} DKK/MWh");
                
                double targetHeatDemand = data.HeatDemand;
                double expenses = 0;
                double gasConsumption = 0;
                double oilConsumption = 0;
                double co2Emissions = 0;
                double electricityProduction = 0;
                double electricityConsumption = 0;
                double profit = 0;

                ResultData result = new ResultData();
                List<ProductionUnit> productionUnitsUsed = new List<ProductionUnit>();

                // Calculate net costs for each unit
                Console.WriteLine("\nCalculating net costs for each unit:");
                foreach (var unit in productionUnits)
                {
                    double netCost;
                    if (unit.FuelType == "Gas" && unit.Type == UnitType.HeatOnly)
                    {
                        netCost = unit.ProductionCosts;
                    }
                    else if (unit.FuelType == "Oil")
                    {
                        netCost = unit.ProductionCosts;
                    }
                    else if (unit.FuelType == "Gas" && unit.Type == UnitType.ElectricityProducing)
                    {
                        netCost = unit.ProductionCosts - data.ElectricityPrice;
                    }
                    else if (unit.Type == UnitType.ElectricityConsuming)
                    {
                        netCost = unit.ProductionCosts + (unit.MaxHeat * data.ElectricityPrice);
                    }
                    else
                    {
                        netCost = double.MaxValue; 
                    }

                    netCosts[unit.Name] = netCost;
                    Console.WriteLine($"{unit.Name}: {netCost} DKK/MWh");
                }

                // Sort units from lowest to highest net cost
                var sortedUnits = productionUnits.OrderBy(u => netCosts[u.Name]).ToList();
                Console.WriteLine("\nUnits sorted by net cost (lowest to highest):");
                foreach (var unit in sortedUnits)
                {
                    Console.WriteLine($"{unit.Name}: {netCosts[unit.Name]} DKK/MWh");
                }

                // Optimize production
                Console.WriteLine("\nOptimizing production:");
                foreach (var unit in sortedUnits)
                {
                    if (targetHeatDemand <= 0) break;
    
                    double heatToProduce = Math.Min(unit.MaxHeat, targetHeatDemand);
                    targetHeatDemand -= heatToProduce;
                    
                    Console.WriteLine($"\nUsing unit: {unit.Name}");
                    Console.WriteLine($"Heat to produce: {heatToProduce} MW");
                    Console.WriteLine($"Remaining demand: {targetHeatDemand} MW");
        
                    expenses += unit.ProductionCosts * heatToProduce;
                    co2Emissions += unit.CO2Emissions * heatToProduce;

                    if (unit.FuelType == "Gas")
                    {
                        gasConsumption += heatToProduce * unit.FuelConsumption;
                    }
                    else if (unit.FuelType == "Oil")
                    {
                        oilConsumption += heatToProduce * unit.FuelConsumption;
                    }

                    // Handle electricity production and consumption
                    if (unit.Type == UnitType.ElectricityProducing)
                    {
                        electricityProduction += heatToProduce * unit.ElectricityProduction;
                        profit += heatToProduce * data.ElectricityPrice;
                        Console.WriteLine($"Electricity produced: {heatToProduce * unit.ElectricityProduction} MWh");
                    }
                    else if (unit.Type == UnitType.ElectricityConsuming)
                    {
                        electricityConsumption += heatToProduce * unit.ElectricityConsumption;
                        expenses += heatToProduce * data.ElectricityPrice;
                        Console.WriteLine($"Electricity consumed: {heatToProduce * unit.ElectricityConsumption} MWh");
                    }

                    productionUnitsUsed.Add(unit);
                }

                result.HeatProduction = data.HeatDemand;
                result.ElectricityProduction = Math.Round(electricityProduction, 2);
                result.ElectricityConsumption = Math.Round(electricityConsumption, 2);
                result.Expenses = Math.Round(expenses, 2);
                result.Profit = Math.Round(profit, 2);
                result.GasConsumption = Math.Round(gasConsumption, 2);
                result.OilConsumption = Math.Round(oilConsumption, 2);
                result.CO2Emissions = Math.Round(co2Emissions, 2);
                result.ProductionUnitsUsed = productionUnitsUsed;
                result.TimeFrom = data.TimeFrom;
                result.TimeTo = data.TimeTo;

                Console.WriteLine("\nResults for this period:");
                Console.WriteLine($"Total expenses: {result.Expenses} DKK");
                Console.WriteLine($"Electricity production: {result.ElectricityProduction} MWh");
                Console.WriteLine($"Electricity consumption: {result.ElectricityConsumption} MWh");
                Console.WriteLine($"Profit: {result.Profit} DKK");
                Console.WriteLine($"Gas consumption: {result.GasConsumption} MWh");
                Console.WriteLine($"Oil consumption: {result.OilConsumption} MWh");
                Console.WriteLine($"CO2 emissions: {result.CO2Emissions} tons");
                Console.WriteLine($"Units used: {string.Join(", ", result.ProductionUnitsUsed.Select(u => u.Name))}");

                if (isSummer)
                {
                    ResultDataManager.SetSummerOptimizedData(result);
                    Console.WriteLine("Saved as summer data");
                }
                else
                {
                    ResultDataManager.SetWinterOptimizedData(result);
                    Console.WriteLine("Saved as winter data");
                }

                isSummer = !isSummer;
            }
            Console.WriteLine("\n=== Scenario 2 Optimization Complete ===");
        }
    }
}