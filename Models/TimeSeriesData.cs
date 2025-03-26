using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP2.Models
{
    public class TimeSeriesData
    {
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public double HeatDemand { get; set; }
        public double ElectricityPrice { get; set; }

        public TimeSeriesData(DateTime timeFrom, DateTime timeTo, double heatDemand, double electricityPrice)
        {
            TimeFrom = timeFrom;
            TimeTo = timeTo;    
            HeatDemand = heatDemand;
            ElectricityPrice = electricityPrice;
        }
    }
}
