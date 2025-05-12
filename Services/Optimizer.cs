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
        private static readonly List<TimeSeriesData> CSVData = SourceDataManager.LoadData("../../../Assets/2025 Heat Production Optimization - Danfoss Deliveries - Source Data Manager.csv");
        private static readonly List<ProductionUnit> ProductionUnits1 = AssetManager.GetProdUnits();
        private static readonly List<ProductionUnit> ProductionUnits2 = AssetManager.GetProdUnits();


        public static void OptimizeScenario1()
        {
            bool isWinter = true;
            
            foreach (var data in CSVData)
            {
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

                if (isWinter)
                {
                    ResultDataManager.SetWinterOptimizedData(result);
                }
                else
                {
                    ResultDataManager.SetSummerOptimizedData(result);
                }

                isWinter = !isWinter;
            }
        }
        public static void OptimizeScenario2()
        {
            bool isSummer = true;
            Dictionary<string, double> netCosts = new Dictionary<string, double>();
            List<ProductionUnit> productionUnits = ProductionUnits2;

            foreach (var data in CSVData)
            {
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
                foreach (var unit in productionUnits)
                {
                    double netCost;
                    if (unit.FuelType == "Gas" && unit.Type == UnitType.HeatOnly)
                    {
                        // Gas boiler - heat only
                        netCost = unit.ProductionCosts;
                    }
                    else if (unit.FuelType == "Oil")
                    {
                        // Oil boiler - heat only
                        netCost = unit.ProductionCosts;
                    }
                    else if (unit.FuelType == "Gas" && unit.Type == UnitType.ElectricityProducing)
                    {
                        // Gas motor - electricity producing
                        netCost = unit.ProductionCosts - data.ElectricityPrice;
                    }
                    else if (unit.Type == UnitType.ElectricityConsuming)
                    {
                        // Heat pump - electricity consuming
                        netCost = unit.ProductionCosts + (unit.MaxHeat * data.ElectricityPrice);
                    }
                    else
                    {
                        netCost = double.MaxValue; 
                    }

                    netCosts[unit.Name] = netCost;
                }

                // Sort units from lowest to highest net cost
                var sortedUnits = productionUnits.OrderBy(u => netCosts[u.Name]).ToList();

                // Optimize production
                foreach (var unit in sortedUnits)
                {
                    if (targetHeatDemand <= 0) break;
    
                    double heatToProduce = Math.Min(unit.MaxHeat, targetHeatDemand);
                    targetHeatDemand -= heatToProduce;
        
                    
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
                    }
                    else if (unit.Type == UnitType.ElectricityConsuming)
                    {
                        electricityConsumption += heatToProduce * unit.ElectricityConsumption;
                        expenses += heatToProduce * data.ElectricityPrice;
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

                if (isSummer)
                {
                    ResultDataManager.SetSummerOptimizedData(result);
                }
                else
                {
                    ResultDataManager.SetWinterOptimizedData(result);
                }

                isSummer = !isSummer;
            }
        }
    }
}