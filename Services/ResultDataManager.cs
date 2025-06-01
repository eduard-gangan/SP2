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
    public static class ResultDataManager
    {
        private static List<ResultData> WinterOptimizedData1 { get; set; } = new List<ResultData>();
        private static List<ResultData> SummerOptimizedData1 { get; set; } = new List<ResultData>();
        private static List<ResultData> WinterOptimizedData2 { get; set; } = new List<ResultData>();
        private static List<ResultData> SummerOptimizedData2 { get; set; } = new List<ResultData>();

        public static void SetWinterOptimizedData(ResultData data, string scenario)
        {
            if (scenario == "Scenario1")
                WinterOptimizedData1.Add(data);
            else
                WinterOptimizedData2.Add(data);
        }

        public static void SetSummerOptimizedData(ResultData data, string scenario)
        {
            if (scenario == "Scenario1")
                SummerOptimizedData1.Add(data);
            else
                SummerOptimizedData2.Add(data);
        }

        public static List<ResultData>? GetWinterOptimizedData(string scenario)
        {
            if (scenario == "Scenario1")
            {
                return WinterOptimizedData1;
            }
            else
            {
                return WinterOptimizedData2;
            }
        }

        public static List<ResultData>? GetSummerOptimizedData(string scenario)
        {
            if (scenario == "Scenario1")
                return SummerOptimizedData1;
            else
                return SummerOptimizedData2;
        }

        public static ResultData? GetWinterResultByTime(DateTime from, DateTime to, string scenario)
        {
            if (scenario == "Scenario1")
            {
                if (WinterOptimizedData1 == null)
                    return null;

                return WinterOptimizedData1
                    .FirstOrDefault(r => r.TimeFrom == from && r.TimeTo == to);
            }
            else
            {
                if (WinterOptimizedData2 == null)
                    return null;
                return WinterOptimizedData2
                    .FirstOrDefault(r => r.TimeFrom == from && r.TimeTo == to);
            }
        }

        public static ResultData? GetSummerResultByTime(DateTime from, DateTime to, string scenario)
        {
            if (scenario == "Scenario1")
            {
                if (SummerOptimizedData1 == null)
                    return null;
                return SummerOptimizedData1
                    .FirstOrDefault(r => r.TimeFrom == from && r.TimeTo == to);
            }
            else
            {
                if (SummerOptimizedData2 == null)
                    return null;
                return SummerOptimizedData2
                    .FirstOrDefault(r => r.TimeFrom == from && r.TimeTo == to);
            }
        }

        public static double GetCO2Emissions(string scenario)
        {
            if (scenario == "Scenario1")
            {
                return WinterOptimizedData1.Sum(x => x.CO2Emissions);
            }
            else
            {
                return WinterOptimizedData2.Sum(x => x.CO2Emissions);
            }
        }
        
        public static double GetExpenses(string scenario)
        {
            if (scenario == "Scenario1")
            {
                return WinterOptimizedData1.Sum(x => x.Expenses);
            }
            else
            {
                return WinterOptimizedData2.Sum(x => x.Expenses);
            }
        }

        public static void SaveDataToCSV(int scenario)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string directory = Path.Combine(baseDir, "../../../", "SavedData");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            if (scenario == 1)
            {
                using (var writer = new StreamWriter(Path.Combine(directory, "WinterOptimizedData1.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<ResultDataMap1>();
                    csv.WriteRecords(WinterOptimizedData1);
                }
                using (var writer = new StreamWriter(Path.Combine(directory, "SummerOptimizedData1.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<ResultDataMap1>();
                    csv.WriteRecords(SummerOptimizedData1);
                }
            }
            else if (scenario == 2)
            {
                using (var writer = new StreamWriter(Path.Combine(directory, "WinterOptimizedData2.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<ResultDataMap2>();
                    csv.WriteRecords(WinterOptimizedData2);
                }
                using (var writer = new StreamWriter(Path.Combine(directory, "SummerOptimizedData2.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<ResultDataMap2>();
                    csv.WriteRecords(SummerOptimizedData2);
                }
            }
        }

        public static void ClearData()
        {
            WinterOptimizedData1.Clear();
            SummerOptimizedData1.Clear();
            WinterOptimizedData2.Clear();
            SummerOptimizedData2.Clear();
        }

    }
}
