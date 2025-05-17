using Xunit;
using SP2.Models;
using SP2.Services;

namespace SP2Tests
{
    public class OptimizerUnitTests
    {
        [Fact]
        public void OptimizeScenario1_ShouldUseCorrectProductionUnits()
        {
            // Act
            Optimiser.OptimizeScenario1();

            // Assert
            var winterResult = ResultDataManager.GetWinterOptimizedData("Scenario1")?.FirstOrDefault();
            Assert.NotNull(winterResult);
            Assert.Contains(winterResult.ProductionUnitsUsed, u => u.Name == "Gas Boiler 1");
            Assert.Contains(winterResult.ProductionUnitsUsed, u => u.Name == "Gas Boiler 2");
        }

        [Fact]
        public void OptimizeScenario1_ShouldCalculateCorrectExpenses()
        {
            // Act
            Optimiser.OptimizeScenario1();

            // Assert
            var winterResult = ResultDataManager.GetSummerOptimizedData("Scenario1")?.FirstOrDefault();
            Assert.NotNull(winterResult);
            
            // Verify that expenses are calculated correctly based on the production units used
            foreach (var unit in winterResult.ProductionUnitsUsed)
            {
                Assert.True(winterResult.Expenses <= unit.ProductionCosts * unit.MaxHeat, 
                    $"Expenses should be less than {unit.ProductionCosts * unit.MaxHeat} for {unit.Name}");
            }
        }

        [Fact]
        public void OptimizeScenario1_ShouldCalculateCorrectEmissions()
        {
            // Act
            Optimiser.OptimizeScenario1();

            // Assert
            var winterResult = ResultDataManager.GetSummerOptimizedData("Scenario1")?.FirstOrDefault();
            Assert.NotNull(winterResult);
            
            // Verify that emissions are calculated correctly based on the production units used
            foreach (var unit in winterResult.ProductionUnitsUsed)
            {
                Assert.True(winterResult.CO2Emissions <= unit.CO2Emissions * unit.MaxHeat,
                    $"CO2 emissions should be less than {unit.CO2Emissions * unit.MaxHeat} for {unit.Name}");
            }
        }

        [Fact]
        public void OptimizeScenario2_ShouldCalculateCorrectElectricityProduction()
        {
            // Act
            Optimiser.OptimizeScenario2();

            // Assert
            var summerResult = ResultDataManager.GetSummerOptimizedData("Scenario2")?.FirstOrDefault();
            Assert.NotNull(summerResult);
            
            // Verify that electricity production is calculated correctly
            var gasMotor = summerResult.ProductionUnitsUsed.FirstOrDefault(u => u.Name == "Gas Motor 1");
            if (gasMotor != null)
            {
                Assert.True(summerResult.ElectricityProduction <= gasMotor.ElectricityProduction * gasMotor.MaxHeat,
                    $"Electricity production should be less than {gasMotor.ElectricityProduction * gasMotor.MaxHeat} for Gas Motor");
            }
        }

        [Fact]
        public void OptimizeScenario2_ShouldCalculateCorrectProfit()
        {
            // Act
            Optimiser.OptimizeScenario2();

            // Assert
            var summerResult = ResultDataManager.GetSummerOptimizedData("Scenario2")?.FirstOrDefault();
            var sumerresult = ResultDataManager.GetSummerOptimizedData("Scenario2");
            Assert.NotNull(summerResult);
            
            // Verify that profit is calculated correctly based on electricity production
            var gasMotor = summerResult.ProductionUnitsUsed.FirstOrDefault(u => u.Name == "Gas Motor 1");
            if (gasMotor != null)
            {
                Assert.True(summerResult.Profit >= 0, "Profit should be non-negative");
                Assert.True(summerResult.Profit <= summerResult.ElectricityProduction * 752.03, 
                    "Profit should not exceed electricity production * price");
            }
        }

        [Fact]
        public void OptimizeScenario2_ShouldCalculateCorrectElectricityConsumption()
        {
            // Act
            Optimiser.OptimizeScenario2();

            // Assert
            var summerResult = ResultDataManager.GetSummerOptimizedData("Scenario2")?.FirstOrDefault();
            var res = ResultDataManager.GetWinterOptimizedData("Scenario2")?.FirstOrDefault(r => r.HeatProduction > 7.5);
            Assert.NotNull(summerResult);
            
            // Verify that electricity consumption is calculated correctly
            var heatPump = summerResult.ProductionUnitsUsed.FirstOrDefault(u => u.Name == "Heat Pump 1");
            if (heatPump != null)
            {
                Assert.True(summerResult.ElectricityConsumption >= heatPump.ElectricityConsumption * heatPump.MaxHeat,
                    $"Electricity consumption should be at least {heatPump.ElectricityConsumption * heatPump.MaxHeat} for Heat Pump");
            }
        }

        [Fact]
        public void OptimizeScenario2_ShouldOptimizeBasedOnNetCosts()
        {
            // Act
            Optimiser.OptimizeScenario2();

            // Assert
            var summerResult = ResultDataManager.GetSummerOptimizedData("Scenario2")?.FirstOrDefault();
            Assert.NotNull(summerResult);

            // Verify that units are used in order of their net costs
            var productionUnits = summerResult.ProductionUnitsUsed.ToList();
            for (int i = 0; i < productionUnits.Count - 1; i++)
            {
                var currentUnit = productionUnits[i];
                var nextUnit = productionUnits[i + 1];

                double currentNetCost = CalculateNetCost(currentUnit);
                double nextNetCost = CalculateNetCost(nextUnit);

                Assert.True(currentNetCost <= nextNetCost,
                    $"Unit {currentUnit.Name} should have lower or equal net cost than {nextUnit.Name}");
            }
        }

        private double CalculateNetCost(ProductionUnit unit)
        {
            if (unit.FuelType == "Gas" && unit.Type == UnitType.HeatOnly)
            {
                return unit.ProductionCosts;
            }
            else if (unit.FuelType == "Oil")
            {
                return unit.ProductionCosts;
            }
            else if (unit.FuelType == "Gas" && unit.Type == UnitType.ElectricityProducing)
            {
                return unit.ProductionCosts - (unit.MaxElectricity * 100.0); // Using a fixed electricity price for testing
            }
            else if (unit.Type == UnitType.ElectricityConsuming)
            {
                return unit.ProductionCosts + (unit.MaxHeat * 100.0); 
            }
            return double.MaxValue;
        }
    }
}
