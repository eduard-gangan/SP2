using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using SP2.Models;

namespace SP2.Services
{
    public static class AssetManager
    {
        private static readonly HeatGrid HeatGrid = new HeatGrid
        {
            Name = "Heatington",
            Image = "Assets/HeatGrid.png"
        };

        private static readonly List<ProductionUnit> _productionUnits = new List<ProductionUnit>
            {
            new ProductionUnit("Gas Boiler 1", "Assets/GasBoiler.png", 4.0, 0.0, 520.0, 175.0, 0.9, "Gas", UnitType.HeatOnly, 0, 0),
            new ProductionUnit("Gas Boiler 2", "Assets/GasBoiler.png", 3.0, 0.0, 560.0, 130.0, 0.7, "Gas", UnitType.HeatOnly, 0, 0),
            new ProductionUnit("Oil Boiler 1", "Assets/OilBoiler.png", 4.0, 0.0, 670.0, 330.0, 1.5, "Oil", UnitType.HeatOnly, 0, 0),
            new ProductionUnit("Gas Motor 1", "Assets/GasMotor.png", 3.5, 2.6, 990.0, 650.0, 1.8, "Gas", UnitType.ElectricityProducing, 2.6, 0),
            new ProductionUnit("Heat Pump 1", "Assets/HeatPump.png", 6.0, -6.0, 60.0, 0, 0, "Electricity", UnitType.ElectricityConsuming, 0, 6)
            };

        public static List<ProductionUnit> GetProdUnits()
        {
            return _productionUnits;
        }
        public static ProductionUnit? GetProdUnit(string name)
        {
            return _productionUnits.FirstOrDefault(x => x.Name == name);
        }
        public static HeatGrid GetHeatGrid()
        {
            return HeatGrid;
        }
    }
}