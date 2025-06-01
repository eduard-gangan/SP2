using CsvHelper.Configuration;

namespace SP2.Models
{

    public sealed class ResultDataMap1 : ClassMap<ResultData>
    {
        public ResultDataMap1()
        {
            Map(m => m.TimeFrom).Name("Time From (DK)");
            Map(m => m.TimeTo).Name("Time To (DK)");
            Map(m => m.HeatProduction).Name("Heat Production (MW)");
            Map(m => m.Expenses).Name("Expenses (DKK)");
            Map(m => m.GasConsumption).Name("Gas Consumption (MWh(gas))");
            Map(m => m.OilConsumption).Name("Oil Consumption (MWh(oil))");
            Map(m => m.CO2Emissions).Name("CO2 Emissions (Kg)");
            Map(m => m.ProductionUnitsUsedNames).Name("Units Used");
        }
    }

    public sealed class ResultDataMap2 : ClassMap<ResultData>
    {
        public ResultDataMap2()
        {
            Map(m => m.TimeFrom).Name("Time From (DK)");
            Map(m => m.TimeTo).Name("Time To (DK)");
            Map(m => m.HeatProduction).Name("Heat Production (MW)");
            Map(m => m.ElectricityProduction).Name("Electricity Production (MW)");
            Map(m => m.ElectricityConsumption).Name("Electricity Consumption (MW)");
            Map(m => m.Expenses).Name("Expenses (DKK)");
            Map(m => m.Profit).Name("Profit (DKK)");
            Map(m => m.GasConsumption).Name("Gas Consumption (MWh(gas))");
            Map(m => m.OilConsumption).Name("Oil Consumption (MWh(oil))");
            Map(m => m.CO2Emissions).Name("CO2 Emissions (Kg)");
            Map(m => m.ProductionUnitsUsedNames).Name("Units Used");
        }
    }
}
