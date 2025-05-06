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
        private static List<ResultData> WinterOptimizedData { get; set; } = new List<ResultData>();
        private static List<ResultData> SummerOptimizedData { get; set; } = new List<ResultData>();

        public static void SetWinterOptimizedData(ResultData data)
        {
            WinterOptimizedData.Add(data);
        }

        public static void SetSummerOptimizedData(ResultData data)
        {
            SummerOptimizedData.Add(data);
        }

        public static List<ResultData>? GetWinterOptimizedData()
        {
            return WinterOptimizedData;
        }

        public static List<ResultData>? GetSummerOptimizedData()
        {
            return SummerOptimizedData;
        }

        public static ResultData? GetWinterResultByTime(DateTime from, DateTime to)
        {
            if (WinterOptimizedData == null)
                return null;

            return WinterOptimizedData
                .FirstOrDefault(r => r.TimeFrom == from && r.TimeTo == to);
        }

        public static ResultData? GetSummerResultByTime(DateTime from, DateTime to)
        {
            if (SummerOptimizedData == null)
                return null;
            return SummerOptimizedData
                .FirstOrDefault(r => r.TimeFrom == from && r.TimeTo == to);
        }
    }
}
