using Xunit;
using SP2.Services;
using SP2.Models;

namespace SP2Tests
{
    public class SDMUnitTests
    {
        [Fact]
        public void LoadData_ShouldReturnFirstRecord_WhenCsvIsValid()
        {
            // Arrange
            var tempFilePath = "../Assets/2025 Heat Production Optimization - Danfoss Deliveries - Source Data Manager.csv";

            // Act
            var records = SourceDataManager.LoadData(tempFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Equal(672, records.Count);
            Assert.Equal(6.62, records[0].HeatDemand);
            Assert.Equal(1190.94, records[0].ElectricityPrice);
        }

        [Fact]
        public void LoadData_ShouldReturnLastRecord_WhenCsvIsValid()
        {
            // Arrange
            var tempFilePath = "../Assets/2025 Heat Production Optimization - Danfoss Deliveries - Source Data Manager.csv";

            // Act
            var records = SourceDataManager.LoadData(tempFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Equal(1.62, records[671].HeatDemand);
            Assert.Equal(607.05, records[671].ElectricityPrice);
        }

        [Fact]
        public void LoadData_ShouldReturnEmptyList_WhenCsvIsEmpty()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, string.Empty);

            // Act
            var records = SourceDataManager.LoadData(tempFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Empty(records);
        }

        [Fact]
        public void LoadData_ShouldThrowException_WhenCsvPathIsInvalid()
        {
            // Arrange
            var invalidPath = "nonexistent.csv";

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => SourceDataManager.LoadData(invalidPath));
        }
    }
}
