using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP2.Models
{
    public class ResultData
    {
        public static double HeatProduction { get; set; }
        public static double ElectricityProduction { get; set; }
        public static double ElectricityConsumption { get; set; }
        public static double Expenses {get; set; }
        public static double Profit { get; set; }
        public static double GasConsumption { get; set; }
        public static double OilConsumption { get; set; }
        public static double CO2Emissions { get; set; }
    }
}
