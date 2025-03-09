using System.Collections.Generic;
using IronXL;

namespace HeatOptimisation.Models
{
    public class DataConverter
    {
        static readonly WorkBook workbook = WorkBook.LoadCSV("../Assets/2025 Heat Production Optimization - Danfoss Deliveries - Source Data Manager.csv");
        readonly WorkSheet workSheet = workbook.GetWorkSheet("Heat Demands");


        private void LoadData()
        {
            foreach (var cell in workSheet)
            {

            }
        }
    }
}
