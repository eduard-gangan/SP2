using System;
using IronXL;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HeatOptimization.Models;

namespace HeatOptimisation.Services
{
    public class AssetManagerService
    {
        private readonly List<ProductionUnit> _productionUnits;

        public AssetManagerService()
        {
            _productionUnits = new List<ProductionUnit>
            {
                new ProductionUnit("Gas Boiler 1", 4.0, null, 520.0, 175.0, 0.9, "Gas"),
                new ProductionUnit("Gas Boiler 2", 3.0, null, 560.0, 130.0, 0.7, "Gas"),
                new ProductionUnit("Oil Boiler 1", 4.0, null, 670.0, 330.0, 1.5, "Oil"),
                new ProductionUnit("Gas Motor 1", 3.5, 2.6, 990.0, 650.0, 1.8, "Gas"),
                new ProductionUnit("HP1", 6.0, -6.0, 60.0, 0, 0, "Electricity")
            };
        }

        public List<ProductionUnit> GetProdUnits()
        {
            return _productionUnits;
        }
    }
}
