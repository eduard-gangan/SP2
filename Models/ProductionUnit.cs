namespace SP2.Models
{
    public class ProductionUnit
    {
        public string Name { get; }
        public string ImagePath { get; }
        public double MaxHeat { get; }
        public double? MaxElectricity { get; }
        public double ProductionCosts { get; }
        public double? CO2Emissions { get; }
        public double? FuelConsumption { get; }
        public string? FuelType { get; }

        public ProductionUnit(string name, string imagePath, double maxHeat, double? maxElectricity, double productionCosts, double? co2Emissions, double fuelConsumption, string fuelType)
        {
            Name = name;
            ImagePath = imagePath;
            MaxHeat = maxHeat;
            MaxElectricity = maxElectricity;
            ProductionCosts = productionCosts;
            CO2Emissions = co2Emissions;
            FuelConsumption = fuelConsumption;
            FuelType = fuelType;
        }
    }
}
