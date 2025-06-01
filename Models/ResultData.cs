using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP2.Models
{
    public class ResultData
    {
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public double HeatProduction { get; set; }
        public double? ElectricityProduction { get; set; }
        public double? ElectricityConsumption { get; set; }
        public double Expenses {get; set; }
        public double? Profit { get; set; }
        public double GasConsumption { get; set; }
        public double OilConsumption { get; set; }
        public double CO2Emissions { get; set; }
        public List<ProductionUnit>? ProductionUnitsUsed { get; set; }
        public string ProductionUnitsUsedNames =>
        ProductionUnitsUsed != null
            ? string.Join(", ", ProductionUnitsUsed.Select(u => u.Name))
            : string.Empty;
    }
}
