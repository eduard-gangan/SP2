namespace HeatOptimization.Models
{
    public class ProductionUnit
    {
        public string Name { get; }
        public double MaxHeat { get; }
        public double? MaxElectricity { get; }
        public double ProductionCosts { get; }
        public double? CO2Emissions { get; }
        public double FuelConsumption { get; }
        public string FuelType { get; }

        public ProductionUnit(string name, double maxHeat, double? maxElectricity, double productionCosts, double? co2Emissions, double fuelConsumption, string fuelType)
        {
            Name = name;
            MaxHeat = maxHeat;
            MaxElectricity = maxElectricity;
            ProductionCosts = productionCosts;
            CO2Emissions = co2Emissions;
            FuelConsumption = fuelConsumption;
            FuelType = fuelType;
        }
    }
}
