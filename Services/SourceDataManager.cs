using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SP2.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;


namespace SP2.Services
{
    public static class SourceDataManager
    {
        public static List<TimeSeriesData> LoadData(string csvPath)
        {
            var records = new List<TimeSeriesData>();
            
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                MissingFieldFound = null,
                IgnoreBlankLines = true
            });

            csv.Read(); csv.Read();

            while (csv.Read())
            {
                // Read winter period data
                if (DateTime.TryParseExact(csv.GetField(0), "M/d/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var winterTimeFrom) &&
                    DateTime.TryParseExact(csv.GetField(1), "M/d/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var winterTimeTo) &&
                    double.TryParse(csv.GetField(2), out var winterHeatDemand) &&
                    double.TryParse(csv.GetField(3), out var winterElectricityPrice))
                {
                    records.Add(new TimeSeriesData(winterTimeFrom, winterTimeTo, winterHeatDemand, winterElectricityPrice));
                }

                // Read summer period data
                if (DateTime.TryParseExact(csv.GetField(5), "M/d/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var summerTimeFrom) &&
                    DateTime.TryParseExact(csv.GetField(6), "M/d/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var summerTimeTo) &&
                    double.TryParse(csv.GetField(7), out var summerHeatDemand) &&
                    double.TryParse(csv.GetField(8), out var summerElectricityPrice))
                {
                    records.Add(new TimeSeriesData(summerTimeFrom, summerTimeTo, summerHeatDemand, summerElectricityPrice));
                }
            }

            return records;
        }
    }
}
