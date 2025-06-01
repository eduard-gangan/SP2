using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SP2.Models;
using SP2.Services;

namespace SP2.ViewModels
{
    public class HeatViewModel : ViewModelBase
    {
// Property to hold the selected season
private string _selectedSeason;
public string SelectedSeason
{
    get => _selectedSeason;
    set
    {
        if (_selectedSeason != value)
        {
            _selectedSeason = value;
            OnPropertyChanged(nameof(SelectedSeason));
            
            // Update chart title
            UpdateChartTitle();
            
            // Update charts when season changes
            InitializeHeatDemandChart();
            InitializeHeatProductionChart();
        }
    }
}

// Property to hold the available seasons
public List<string> AvailableSeasons { get; } = new List<string> { "Winter", "Summer" };

// Property to hold the selected scenario
private string _selectedScenario;
public string SelectedScenario
{
    get => _selectedScenario;
    set
    {
        if (_selectedScenario != value)
        {
            _selectedScenario = value;
            OnPropertyChanged(nameof(SelectedScenario));
            
            // Update chart title
            UpdateChartTitle();
            
            // Update charts when scenario changes
            InitializeHeatProductionChart();
        }
    }
}

// Property to hold the available scenarios
public List<string> AvailableScenarios { get; } = new List<string> { "Scenario1", "Scenario2" };

// Property to hold the CO2 optimization toggle
private bool _isCO2OptimizationEnabled;
public bool IsCO2OptimizationEnabled
{
    get => _isCO2OptimizationEnabled;
    set
    {
        if (_isCO2OptimizationEnabled != value)
        {
            _isCO2OptimizationEnabled = value;
            OnPropertyChanged(nameof(IsCO2OptimizationEnabled));
            
            // Update chart title
            UpdateChartTitle();
            
            // Update charts when CO2 optimization mode changes
            RefreshOptimizationData();
        }
    }
}

// Property for the Heat Production chart title
private string _heatProductionTitle;
public string HeatProductionTitle
{
    get => _heatProductionTitle;
    set
    {
        if (_heatProductionTitle != value)
        {
            _heatProductionTitle = value;
            OnPropertyChanged(nameof(HeatProductionTitle));
        }
    }
}

// Helper method to update the chart title
private void UpdateChartTitle()
{
    string optimizationMode = IsCO2OptimizationEnabled ? "CO2 Optimized" : "Cost Optimized";
    HeatProductionTitle = $"Heat Production by Unit ({SelectedSeason} - {SelectedScenario} - {optimizationMode})";
}

public HeatViewModel()
{
    // Subscribe to production unit changes
    AssetManager.ProductionUnitsChanged += OnProductionUnitsChanged;
    
    // Run the optimization to generate data
    Services.Optimiser.OptimizeScenario1(false);
    Services.Optimiser.OptimizeScenario2(false);
    
    // Set default values
    _selectedSeason = "Winter";
    _selectedScenario = "Scenario1";
    
    // Set initial chart title
    UpdateChartTitle();
    
    // Initialize the chart data
    InitializeHeatDemandChart();
    InitializeHeatProductionChart();
}

// Event handler for when production units change
private void OnProductionUnitsChanged()
{
    // Refresh optimization data and charts when units are toggled
    RefreshOptimizationData();
}

// Method to refresh optimization data and charts
public void RefreshOptimizationData()
{
    // Clear existing optimization data
    ResultDataManager.ClearData();
    
    // Re-run optimization with current unit availability settings and CO2 mode
    Services.Optimiser.OptimizeScenario1(IsCO2OptimizationEnabled);
    Services.Optimiser.OptimizeScenario2(IsCO2OptimizationEnabled);
    
    // Refresh the charts
    InitializeHeatDemandChart();
    InitializeHeatProductionChart();
}

        #region Heat Demand Chart Properties

        // The series collection for the Heat Demand line chart
        public ObservableCollection<ISeries> HeatDemandSeries { get; set; }

        // X-axis configuration for the Heat Demand chart
        public List<Axis> HeatDemandXAxes { get; set; }

        // Y-axis configuration for the Heat Demand chart
        public List<Axis> HeatDemandYAxes { get; set; }

        #endregion

        #region Heat Production Chart Properties

        // The series collection for the Heat Production area chart
        public ObservableCollection<ISeries> HeatProductionSeries { get; set; }

        // X-axis configuration for the Heat Production chart
        public List<Axis> HeatProductionXAxes { get; set; }

        // Y-axis configuration for the Heat Production chart
        public List<Axis> HeatProductionYAxes { get; set; }

        #endregion

        #region Initialization Methods

        private void InitializeHeatDemandChart()
        {
            // Load real data from the CSV file using SourceDataManager
            var csvPath = "Assets/2025 Heat Production Optimization - Danfoss Deliveries - Source Data Manager.csv";
            var timeSeriesData = SourceDataManager.LoadData(csvPath);
            
            // Filter data based on selected season
            var filteredData = FilterDataBySeason(timeSeriesData);
            
            // Group the data by day and calculate the average heat demand for each day
            var dailyDemandValues = new ObservableCollection<DateTimePoint>();
            
            // Group by date (ignoring time component)
            var groupedByDay = filteredData
                .GroupBy(d => d.TimeFrom.Date)
                .OrderBy(g => g.Key);
                
            foreach (var dayGroup in groupedByDay)
            {
                // Calculate average heat demand for the day
                double avgHeatDemand = dayGroup.Average(d => d.HeatDemand);
                dailyDemandValues.Add(new DateTimePoint(dayGroup.Key, avgHeatDemand));
            }

            // Create the line series with enhanced styling
            HeatDemandSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "Heat Demand",
                    Values = dailyDemandValues,
                    Fill = new SolidColorPaint(SKColors.DodgerBlue.WithAlpha(40)),
                    GeometrySize = 6,
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue, 2),
                    GeometryStroke = new SolidColorPaint(SKColors.DodgerBlue),
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    LineSmoothness = 0.7,
                    TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:d MMM yyyy}: {((DateTimePoint)point.Model).Value:F2} MWh"
                }
            };

            // Configure the X-axis (time axis) with improved formatting
            HeatDemandXAxes = new List<Axis>
            {
                new Axis
                {
                    Name = "",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    Labeler = value => new DateTime((long)value).ToString("d MMM"),
                    UnitWidth = TimeSpan.FromDays(1).Ticks,
                    MinStep = TimeSpan.FromDays(7).Ticks, // Show weekly labels for better readability
                    LabelsRotation = 45 // Rotate labels for better fit
                }
            };

            // Configure the Y-axis with improved formatting
            HeatDemandYAxes = new List<Axis>
            {
                new Axis
                {
                    Name = "",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    MinLimit = 0, // Start Y-axis at 0
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(100)) { StrokeThickness = 1 },
                    Labeler = value => $"{value:F1} MWh"
                }
            };
            
            // Notify that the chart data has changed
            OnPropertyChanged(nameof(HeatDemandSeries));
            OnPropertyChanged(nameof(HeatDemandXAxes));
            OnPropertyChanged(nameof(HeatDemandYAxes));
        }

// Helper method to filter time series data by season
private List<TimeSeriesData> FilterDataBySeason(List<TimeSeriesData> data)
{
    if (data == null || !data.Any())
        return new List<TimeSeriesData>();
        
    // Filter based on selected season
    if (SelectedSeason == "Winter")
    {
        // For winter, include data from October to March
        return data.Where(d => 
            d.TimeFrom.Month >= 10 || d.TimeFrom.Month <= 3
        ).ToList();
    }
    else
    {
        // For summer, include data from April to September
        return data.Where(d => 
            d.TimeFrom.Month >= 4 && d.TimeFrom.Month <= 9
        ).ToList();
    }
}

private void InitializeHeatProductionChart()
{
    // Get the optimized data based on selected scenario
    List<ResultData> data;
    
    if (SelectedSeason == "Winter")
    {
        data = ResultDataManager.GetWinterOptimizedData(SelectedScenario);
    }
    else
    {
        data = ResultDataManager.GetSummerOptimizedData(SelectedScenario);
    }
    
    if (data == null || !data.Any())
    {
        // If no data is available, return empty chart
        HeatProductionSeries = new ObservableCollection<ISeries>();
        HeatProductionXAxes = new List<Axis>();
        HeatProductionYAxes = new List<Axis>();
        return;
    }
    
    // Sort by time
    var allData = data.OrderBy(d => d.TimeFrom).ToList();
    
    // Get all unique production units used across all time periods
    var allUnits = allData
        .SelectMany(d => d.ProductionUnitsUsed)
        .Select(u => u.Name)
        .Distinct()
        .ToList();
    
    // Create a dictionary to store data series for each production unit
    var unitDataSeries = new Dictionary<string, ObservableCollection<DateTimePoint>>();
    
    // Initialize empty collections for each unit
    foreach (var unitName in allUnits)
    {
        unitDataSeries[unitName] = new ObservableCollection<DateTimePoint>();
    }
    
    // Process each time period
    foreach (var period in allData)
    {
        // Use the middle of the time period as the data point time
        var pointTime = period.TimeFrom.AddHours((period.TimeTo - period.TimeFrom).TotalHours / 2);
        
        // Calculate total heat production for this period
        double totalHeat = period.HeatProduction;
        
        // Dictionary to track heat production by unit for this period
        var unitHeatProduction = new Dictionary<string, double>();
        
        // Initialize all units to 0 for this period
        foreach (var unitName in allUnits)
        {
            unitHeatProduction[unitName] = 0;
        }
        
        // Calculate heat production for each unit in this period
        foreach (var unit in period.ProductionUnitsUsed)
        {
            // Find how much heat this unit produced in this period
            // We need to calculate this based on the optimization logic
            var unitHeat = CalculateUnitHeatProduction(unit, period);
            unitHeatProduction[unit.Name] = unitHeat;
        }
        
        // Add data points for each unit
        foreach (var unitName in allUnits)
        {
            unitDataSeries[unitName].Add(new DateTimePoint(pointTime, unitHeatProduction[unitName]));
        }
    }
    
    // Define colors for each unit type
    var unitColors = new Dictionary<string, SKColor>
    {
        { "Gas Boiler 1", SKColors.MediumSeaGreen },
        { "Gas Boiler 2", SKColors.OrangeRed },
        { "Oil Boiler 1", SKColors.RoyalBlue },
        { "Gas Motor 1", SKColors.Gold },
        { "Heat Pump 1", SKColors.Purple }
    };
    
    // Create the series collection
    HeatProductionSeries = new ObservableCollection<ISeries>();
    
    // Add a series for each production unit
    foreach (var unitName in allUnits)
    {
        // Get color for this unit (or default if not found)
        var unitColor = unitColors.ContainsKey(unitName) 
            ? unitColors[unitName] 
            : SKColors.Gray;
            
        HeatProductionSeries.Add(
            new StackedAreaSeries<DateTimePoint>
            {
                Name = unitName,
                Values = unitDataSeries[unitName],
                Stroke = new SolidColorPaint(unitColor),
                Fill = new SolidColorPaint(unitColor.WithAlpha(150)),
                GeometrySize = 0,
                LineSmoothness = 0.5,
                TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:g}: {((DateTimePoint)point.Model).Value:F2} MW"
            }
        );
    }
    
    // Configure the X-axis (time axis) with improved formatting
    HeatProductionXAxes = new List<Axis>
    {
        new Axis
        {
            Name = "",
            NamePaint = new SolidColorPaint(SKColors.Black),
            LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
            TextSize = 12,
            Labeler = value => new DateTime((long)value).ToString("M/d HH:mm"),
            UnitWidth = TimeSpan.FromHours(12).Ticks,
            MinStep = TimeSpan.FromHours(12).Ticks,
            LabelsRotation = 45,
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(100)) { StrokeThickness = 1 }
        }
    };
    
    // Configure the Y-axis with improved formatting
    HeatProductionYAxes = new List<Axis>
    {
        new Axis
        {
            Name = "",
            NamePaint = new SolidColorPaint(SKColors.Black),
            LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
            TextSize = 12,
            MinLimit = 0, // Start Y-axis at 0
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(100)) { StrokeThickness = 1 },
            Labeler = value => $"{value:F1} MW"
        }
    };
    
    // Notify that the chart data has changed
    OnPropertyChanged(nameof(HeatProductionSeries));
    OnPropertyChanged(nameof(HeatProductionXAxes));
    OnPropertyChanged(nameof(HeatProductionYAxes));
}

// Helper method to calculate how much heat a unit produced in a given period
private double CalculateUnitHeatProduction(ProductionUnit unit, ResultData period)
{
    // For Scenario2, we need to calculate based on the optimization logic in Optimizer.cs
    // This is a simplified version of the calculation
    
    double targetHeatDemand = period.HeatProduction;
    double unitHeat = 0;
    
    // If this unit is in the list of units used, calculate its contribution
    if (period.ProductionUnitsUsed.Any(u => u.Name == unit.Name))
    {
        // Get all units used in this period, ordered by their position in the list
        // This approximates the order they were used in the optimization
        var usedUnits = period.ProductionUnitsUsed;
        
        // Find the index of this unit in the list
        int unitIndex = usedUnits.FindIndex(u => u.Name == unit.Name);
        
        // Calculate how much heat was produced by units before this one
        double heatProducedBefore = 0;
        for (int i = 0; i < unitIndex; i++)
        {
            heatProducedBefore += Math.Min(usedUnits[i].MaxHeat, targetHeatDemand - heatProducedBefore);
        }
        
        // Calculate how much heat this unit produced
        unitHeat = Math.Min(unit.MaxHeat, targetHeatDemand - heatProducedBefore);
    }
    
    return unitHeat;
}

        #endregion
    }
}
