using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP2.Models;

namespace SP2.Services
{
    public static class AssetManager
    {
        
        private static readonly List<ProductionUnit> _productionUnits = new List<ProductionUnit>
            {
            new ProductionUnit("Gas Boiler 1", "Assets/GasBoiler.png", 4.0, null, 520.0, 175.0, 0.9, "Gas"),
            new ProductionUnit("Gas Boiler 2", "Assets/GasBoiler.png", 3.0, null, 560.0, 130.0, 0.7, "Gas"),
            new ProductionUnit("Oil Boiler 1", "Assets/OilBoiler.png", 4.0, null, 670.0, 330.0, 1.5, "Oil"),
            new ProductionUnit("Gas Motor 1", "Assets/GasMotor.png", 3.5, 2.6, 990.0, 650.0, 1.8, "Gas"),
            new ProductionUnit("Heat Pump 1", "Assets/HeatPump.png", 6.0, -6.0, 60.0, 0, 0, "Electricity")
            };
        
        public static List<ProductionUnit> GetProdUnits()
        {
            return _productionUnits;
        }
        public static ProductionUnit? GetProdUnit(string name)
        {
            return _productionUnits.FirstOrDefault(x => x.Name == name);
        }

    }
}
