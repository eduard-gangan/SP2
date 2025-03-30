using System;
using System.Collections.Generic;
using System.Linq;
using SP2.Models;

namespace SP2.Services
{
    public class Optimiser
    {
        private readonly List<ProductionUnit> _productionUnits;
        private readonly Dictionary<DateTime, double> _electricityPrices;

        public Optimiser(List<ProductionUnit> productionUnits, Dictionary<DateTime, double> electricityPrices)
        {
            _productionUnits = productionUnits;
            _electricityPrices = electricityPrices;
        }

        public List<(ProductionUnit Unit, double Utilization)> Optimise(double targetHeatDemand)
        {
            var result = new List<(ProductionUnit Unit, double Utilization)>();
            double remainingDemand = targetHeatDemand;

            // Sort units by net production cost (lowest first)
            var sortedUnits = _productionUnits
                .OrderBy(u => CalculateNetProductionCost(u))
                .ToList();

            foreach (var unit in sortedUnits)
            {
                if (remainingDemand <= 0) break;

                double utilization = Math.Min(1.0, remainingDemand / unit.MaxHeat);
                result.Add((unit, utilization));
                remainingDemand -= unit.MaxHeat * utilization;
            }

            return result;
        }

        private double CalculateNetProductionCost(ProductionUnit unit)
        {
            double baseCost = unit.ProductionCosts;

            switch (unit.Type)
            {
                case UnitType.HeatOnly:
                    return baseCost;

                case UnitType.ElectricityProducing:
                    return baseCost - (unit.ElectricityProduction * _electricityPrices.Values.Average());

                case UnitType.ElectricityConsuming:
                    return baseCost + (unit.ElectricityConsumption * _electricityPrices.Values.Average());

                default:
                    return baseCost;
            }
        }
    }
}