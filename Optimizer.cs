using System;
using System.Collections.Generic;
using System.Linq;
using SP2.Models;

namespace SP2.Services
{
    public class Optimiser
    {
        private readonly List<ProductionUnit> _productionUnits;
        private readonly double _costWeight;
        private readonly double _emissionsWeight;
        private readonly double _efficiencyWeight;

        public Optimiser(List<ProductionUnit> productionUnits, double costWeight = 0.4, double emissionsWeight = 0.3, double efficiencyWeight = 0.3)
        {
            _productionUnits = productionUnits ?? throw new ArgumentNullException(nameof(productionUnits));
            _costWeight = costWeight;
            _emissionsWeight = emissionsWeight;
            _efficiencyWeight = efficiencyWeight;
        }

        public class OptimisationResult
        {
            public List<(ProductionUnit Unit, double Utilization)> UnitUtilizations { get; set; } = new();
            public double TotalCost { get; set; }
            public double TotalEmissions { get; set; }
            public double TotalEfficiency { get; set; }
            public double TotalHeatOutput { get; set; }
        }
        
        public OptimisationResult Optimise(double targetHeatDemand)
        {
            if (targetHeatDemand <= 0)
                throw new ArgumentException("Target heat demand must be positive", nameof(targetHeatDemand));

            var result = new OptimisationResult();
            
            var sortedUnits = _productionUnits
                .Where(u => u.MaxHeat > 0)
                .OrderByDescending(u => CalculateUnitScore(u))
                .ToList();

            double remainingDemand = targetHeatDemand;

            foreach (var unit in sortedUnits)
            {
                if (remainingDemand <= 0) break;

                double utilization = Math.Min(1.0, remainingDemand / unit.MaxHeat);
                double unitHeatOutput = unit.MaxHeat * utilization;

                result.UnitUtilizations.Add((unit, utilization));
                result.TotalCost += unit.ProductionCosts * utilization;
                result.TotalEmissions += (unit.CO2Emissions ?? 0) * utilization;
                result.TotalHeatOutput += unitHeatOutput;
                remainingDemand -= unitHeatOutput;
            }

            // Calculate overall efficiency
            if (result.UnitUtilizations.Any())
            {
                result.TotalEfficiency = result.UnitUtilizations.Average(u => 
                    u.Unit.MaxHeat * u.Utilization / (u.Unit.FuelConsumption ?? 1));
            }

            return result;
        }

        private double CalculateUnitScore(ProductionUnit unit)
        {
            double costScore = 1 - (unit.ProductionCosts / (unit.MaxHeat * 1000));
            double emissionsScore = unit.CO2Emissions.HasValue ? 
                1 - (unit.CO2Emissions.Value / (unit.MaxHeat * 1000)) : 0;
            double efficiencyScore = unit.MaxHeat / (unit.FuelConsumption ?? 1);

            return (costScore * _costWeight) + 
                   (emissionsScore * _emissionsWeight) + 
                   (efficiencyScore * _efficiencyWeight);
        }
    }
}