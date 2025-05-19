using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SP2.Models;
using SP2.Services;
using System.Linq;

namespace SP2.ViewModels
{
    public class EconEnvironmentViewModel : ViewModelBase
    {
        private string _selectedScenario = "Scenario 1";
        private ObservableCollection<ISeries> _productionCostsSeries;
        private ObservableCollection<ISeries> _co2EmissionsSeries;
        private string _productionCostsTitle = "Net Production Costs - Scenario 1";
        private string _co2EmissionsTitle = "CO2 Emissions - Scenario 1";

        public EconEnvironmentViewModel()
        {
            // Run optimizations to ensure data is available
            Optimiser.OptimizeScenario1();
            Optimiser.OptimizeScenario2();
            
            // Initialize the chart data
            UpdateChartData();
        }

        #region Properties

        // The series collection for the Production Costs bar chart
        public ObservableCollection<ISeries> ProductionCostsSeries
        {
            get => _productionCostsSeries;
            set
            {
                if (_productionCostsSeries != value)
                {
                    _productionCostsSeries = value;
                    OnPropertyChanged(nameof(ProductionCostsSeries));
                }
            }
        }

        // The series collection for the CO2 Emissions bar chart
        public ObservableCollection<ISeries> CO2EmissionsSeries
        {
            get => _co2EmissionsSeries;
            set
            {
                if (_co2EmissionsSeries != value)
                {
                    _co2EmissionsSeries = value;
                    OnPropertyChanged(nameof(CO2EmissionsSeries));
                }
            }
        }

        private List<Axis> _productionCostsXAxes;
        private List<Axis> _productionCostsYAxes;
        private List<Axis> _co2EmissionsXAxes;
        private List<Axis> _co2EmissionsYAxes;

        // X-axis configuration for the Production Costs chart
        public List<Axis> ProductionCostsXAxes
        {
            get => _productionCostsXAxes;
            set
            {
                if (_productionCostsXAxes != value)
                {
                    _productionCostsXAxes = value;
                    OnPropertyChanged(nameof(ProductionCostsXAxes));
                }
            }
        }

        // Y-axis configuration for the Production Costs chart
        public List<Axis> ProductionCostsYAxes
        {
            get => _productionCostsYAxes;
            set
            {
                if (_productionCostsYAxes != value)
                {
                    _productionCostsYAxes = value;
                    OnPropertyChanged(nameof(ProductionCostsYAxes));
                }
            }
        }

        // X-axis configuration for the CO2 Emissions chart
        public List<Axis> CO2EmissionsXAxes
        {
            get => _co2EmissionsXAxes;
            set
            {
                if (_co2EmissionsXAxes != value)
                {
                    _co2EmissionsXAxes = value;
                    OnPropertyChanged(nameof(CO2EmissionsXAxes));
                }
            }
        }

        // Y-axis configuration for the CO2 Emissions chart
        public List<Axis> CO2EmissionsYAxes
        {
            get => _co2EmissionsYAxes;
            set
            {
                if (_co2EmissionsYAxes != value)
                {
                    _co2EmissionsYAxes = value;
                    OnPropertyChanged(nameof(CO2EmissionsYAxes));
                }
            }
        }

        // Available scenarios for the dropdown
        public List<string> AvailableScenarios { get; } = new List<string> { "Scenario 1", "Scenario 2" };

        // Selected scenario from the dropdown
        public string SelectedScenario
        {
            get => _selectedScenario;
            set
            {
                if (_selectedScenario != value)
                {
                    _selectedScenario = value;
                    OnPropertyChanged(nameof(SelectedScenario));
                    
                    // Update chart titles
                    ProductionCostsTitle = $"Net Production Costs - {_selectedScenario}";
                    CO2EmissionsTitle = $"CO2 Emissions - {_selectedScenario}";
                    
                    // Update chart data based on selected scenario
                    UpdateChartData();
                }
            }
        }

        // Production Costs chart title property
        public string ProductionCostsTitle
        {
            get => _productionCostsTitle;
            set
            {
                if (_productionCostsTitle != value)
                {
                    _productionCostsTitle = value;
                    OnPropertyChanged(nameof(ProductionCostsTitle));
                }
            }
        }

        // CO2 Emissions chart title property
        public string CO2EmissionsTitle
        {
            get => _co2EmissionsTitle;
            set
            {
                if (_co2EmissionsTitle != value)
                {
                    _co2EmissionsTitle = value;
                    OnPropertyChanged(nameof(CO2EmissionsTitle));
                }
            }
        }

        #endregion

        #region Data Methods

        private void UpdateChartData()
        {
            // Choose the appropriate data set based on selected scenario
            if (_selectedScenario == "Scenario 1")
            {
                InitializeScenarioCharts("Scenario1");
            }
            else
            {
                InitializeScenarioCharts("Scenario2");
            }
        }

        private void InitializeScenarioCharts(string scenarioKey)
        {
            // Get real data from ResultDataManager
            var winterData = ResultDataManager.GetWinterOptimizedData(scenarioKey);
            var summerData = ResultDataManager.GetSummerOptimizedData(scenarioKey);
            
            if (winterData == null || summerData == null || winterData.Count == 0 || summerData.Count == 0)
            {
                // Fallback to empty data if optimization results aren't available
                ProductionCostsSeries = new ObservableCollection<ISeries>();
                CO2EmissionsSeries = new ObservableCollection<ISeries>();
                return;
            }

            // Get production units used in each time period
            var allUnits = new List<ProductionUnit>();
            foreach (var result in winterData.Concat(summerData))
            {
                foreach (var unit in result.ProductionUnitsUsed)
                {
                    if (!allUnits.Any(u => u.Name == unit.Name))
                    {
                        allUnits.Add(unit);
                    }
                }
            }

            // Production Costs data
            var productionCostsValues = allUnits.Select(unit => unit.ProductionCosts).ToList();
            var productionCostsLabels = allUnits.Select(unit => unit.Name).ToList();
            
            // CO2 Emissions data
            var co2EmissionsValues = allUnits.Select(unit => unit.CO2Emissions).ToList();
            
            // Initialize the charts with the data
            InitializeProductionCostsChart(productionCostsValues, productionCostsLabels);
            InitializeCO2EmissionsChart(co2EmissionsValues, productionCostsLabels);
        }

        private void InitializeProductionCostsChart(List<double> values, List<string> labels)
        {
            // Create the bar series for Production Costs
            ProductionCostsSeries = new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Name = "Production Costs",
                    Values = values,
                    Fill = new SolidColorPaint(SKColors.DodgerBlue),
                    Stroke = null,
                    MaxBarWidth = 40,
                    TooltipLabelFormatter = point => $"{labels[(int)point.Context.Index]}: {point.PrimaryValue} DKK/MWh"
                }
            };

            // Configure the X-axis
            ProductionCostsXAxes = new List<Axis>
            {
                new Axis
                {
                    Labels = labels,
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12
                }
            };

            // Configure the Y-axis
            ProductionCostsYAxes = new List<Axis>
            {
                new Axis
                {
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    MinLimit = 0, // Start Y-axis at 0
                    Labeler = value => $"{value} DKK/MWh"
                }
            };
        }

        private void InitializeCO2EmissionsChart(List<double> values, List<string> labels)
        {
            // Create the bar series for CO2 Emissions
            CO2EmissionsSeries = new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Name = "CO2 Emissions",
                    Values = values,
                    Fill = new SolidColorPaint(SKColors.OrangeRed),
                    Stroke = null,
                    MaxBarWidth = 40,
                    TooltipLabelFormatter = point => $"{labels[(int)point.Context.Index]}: {point.PrimaryValue} tons CO2/MWh"
                }
            };

            // Configure the X-axis
            CO2EmissionsXAxes = new List<Axis>
            {
                new Axis
                {
                    Labels = labels,
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12
                }
            };

            // Configure the Y-axis
            CO2EmissionsYAxes = new List<Axis>
            {
                new Axis
                {
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    MinLimit = 0, // Start Y-axis at 0
                    Labeler = value => $"{value} tons CO2/MWh"
                }
            };
        }

        #endregion
    }
}
