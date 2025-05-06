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
        private static readonly List<ProductionUnit> ProductionUnits = AssetManager.GetProdUnits();

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
                    ProductionUnit productionUnit = ProductionUnits[UnitIndex];
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
    }
}