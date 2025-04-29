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
        private static List<ResultData>? OptimizedData {get; set; }

            public static void SetOptimizedData(ResultData data)
            {
                OptimizedData.Add(data);
            }

            
            public static List<ResultData>? GetOptimizedData()
            {
                return OptimizedData;
            }

            public static ResultData? GetResultByTime(DateTime from, DateTime to)
            {
                if (OptimizedData == null)
                    return null;

                return OptimizedData
                    .FirstOrDefault(r => r.TimeFrom == from && r.TimeTo == to);
            }
        
    }
}
