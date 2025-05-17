using Xunit;
using SP2.Services;
using SP2.Models;

namespace SP2Tests
{
    public class AMUnitTests
    {
        [Fact]
        public void GetProdUnits_ShouldReturnAllProductionUnits()
        {
            // Act
            var productionUnits = AssetManager.GetProdUnits();

            // Assert
            Assert.NotNull(productionUnits);
            Assert.Equal(5, productionUnits.Count);
        }

        [Fact]
        public void GetProdUnit_ShouldReturnCorrectUnit_WhenNameExists()
        {
            // Arrange
            var unitName = "Gas Boiler 1";

            // Act
            var productionUnit = AssetManager.GetProdUnit(unitName);

            // Assert
            Assert.NotNull(productionUnit);
            Assert.Equal(unitName, productionUnit.Name);
        }

        [Fact]
        public void GetProdUnit_ShouldReturnNull_WhenNameDoesNotExist()
        {
            // Arrange
            var unitName = "Nonexistent Unit";

            // Act
            var productionUnit = AssetManager.GetProdUnit(unitName);

            // Assert
            Assert.Null(productionUnit);
        }

        [Fact]
        public void GetHeatGrid_ShouldReturnHeatGrid()
        {
            // Act
            var heatGrid = AssetManager.GetHeatGrid();

            // Assert
            Assert.NotNull(heatGrid);
            Assert.Equal("Heatington", heatGrid.Name);
            Assert.Equal("Assets/HeatGrid.png", heatGrid.Image);
        }
    }
}