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


namespace SP2.ViewModels
{
    public class SourceDataManager
    {
        private static double winterHeatDemand;
        private static double winterElectricityPrice;
        private static double summerHeatDemand;
        private static double summerElectricityPrice;

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
                if(DateTime.TryParse(csv.GetField(0), out var winterTimeFrom) &&
                    DateTime.TryParse(csv.GetField(1), out var winterTimeTo) &&
                    double.TryParse(csv.GetField(2), out winterHeatDemand) &&
                    double.TryParse(csv.GetField(3), out winterElectricityPrice))
                {
                    records.Add(new TimeSeriesData(winterTimeFrom, winterTimeTo, winterHeatDemand, winterElectricityPrice));
                }

                if (DateTime.TryParse(csv.GetField(5), out var summerTimeFrom) &&
                    DateTime.TryParse(csv.GetField(6), out var summerTimeTo) &&
                    double.TryParse(csv.GetField(7), out summerHeatDemand) &&
                    double.TryParse(csv.GetField(8), out summerElectricityPrice))
                {
                    records.Add(new TimeSeriesData(summerTimeFrom, summerTimeTo, winterHeatDemand, winterElectricityPrice));
                }

                return records;
            }

            
        }
    }
}
