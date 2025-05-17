using Xunit;
using SP2.Services;
using SP2.Models;

namespace SP2Tests
{
    public class RDMUnitTests
    {
        [Fact]
        public void SetWinterOptimizedData_ShouldAddDataToWinterList()
        {
            // Arrange
            var resultData = new ResultData
            {
                TimeFrom = DateTime.Now,
                TimeTo = DateTime.Now.AddHours(1),
                HeatProduction = 100,
                Expenses = 200
            };

            // Act
            ResultDataManager.SetWinterOptimizedData(resultData, "Scenario1");
            var winterData = ResultDataManager.GetWinterOptimizedData("Scenario1");

            // Assert
            Assert.NotNull(winterData);
            Assert.Contains(resultData, winterData);

            Dispose();
        }

        [Fact]
        public void GetSummerResultByTime_ShouldReturnCorrectResult()
        {
            // Arrange
            var timeFrom = DateTime.Now;
            var timeTo = timeFrom.AddHours(1);
            var resultData = new ResultData
            {
                TimeFrom = timeFrom,
                TimeTo = timeTo,
                HeatProduction = 250,
                Expenses = 500
            };
            ResultDataManager.SetSummerOptimizedData(resultData, "Scenario1");

            // Act
            var retrievedData = ResultDataManager.GetSummerResultByTime(timeFrom, timeTo, "Scenario1");

            // Assert
            Assert.NotNull(retrievedData);
            Assert.Equal(resultData, retrievedData);

            Dispose();
        }

        [Fact]
        public void GetWinterResultByTime_ShouldReturnNull_WhenNoMatch()
        {
            // Act
            var retrievedData = ResultDataManager.GetWinterResultByTime(DateTime.Now, DateTime.Now.AddHours(1), "Scenario1");

            // Assert
            Assert.Null(retrievedData);

            Dispose();
        }

        [Fact]
        public void Dispose()
        {
            ResultDataManager.ClearData();
        }
    }
}
