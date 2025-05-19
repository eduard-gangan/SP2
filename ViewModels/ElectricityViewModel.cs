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
    public class ElectricityViewModel : ViewModelBase
    {
        private string _selectedSeason = "Winter";
        private ObservableCollection<ISeries> _electricityPriceSeries;
        private string _chartTitle = "Electricity prices time series - Winter";
        private List<TimeSeriesData> _timeSeriesData;

        public ElectricityViewModel()
        {
            // Load the data from CSV
            _timeSeriesData = SourceDataManager.LoadData("Assets/2025 Heat Production Optimization - Danfoss Deliveries - Source Data Manager.csv");
            
            // Initialize the chart data
            UpdateChartData();
        }

        #region Properties

        // The series collection for the Electricity Price line chart
        public ObservableCollection<ISeries> ElectricityPriceSeries
        {
            get => _electricityPriceSeries;
            set
            {
                if (_electricityPriceSeries != value)
                {
                    _electricityPriceSeries = value;
                    OnPropertyChanged(nameof(ElectricityPriceSeries));
                }
            }
        }

        // X-axis configuration for the Electricity Price chart
        public List<Axis> ElectricityPriceXAxes { get; set; }

        // Y-axis configuration for the Electricity Price chart
        public List<Axis> ElectricityPriceYAxes { get; set; }

        // Available seasons for the dropdown
        public List<string> AvailableSeasons { get; } = new List<string> { "Winter", "Summer" };

        // Selected season from the dropdown
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
                    ChartTitle = $"Electricity prices time series - {_selectedSeason}";
                    
                    // Update chart data based on selected season
                    UpdateChartData();
                }
            }
        }

        // Chart title property
        public string ChartTitle
        {
            get => _chartTitle;
            set
            {
                if (_chartTitle != value)
                {
                    _chartTitle = value;
                    OnPropertyChanged(nameof(ChartTitle));
                }
            }
        }

        #endregion

        #region Data Methods

        private void UpdateChartData()
        {
            // Choose the appropriate data set based on selected season
            if (_selectedSeason == "Winter")
            {
                InitializeWinterPriceChart();
            }
            else
            {
                InitializeSummerPriceChart();
            }
        }

        private void InitializeWinterPriceChart()
        {
            // Filter winter data (March data from the CSV)
            var winterData = _timeSeriesData
                .Where(d => d.TimeFrom.Month == 3) // March data represents winter
                .OrderBy(d => d.TimeFrom)
                .ToList();

            // Create data points for the Winter Electricity Price chart
            var hourlyPriceValues = new ObservableCollection<DateTimePoint>();
            
            foreach (var data in winterData)
            {
                hourlyPriceValues.Add(new DateTimePoint(data.TimeFrom, data.ElectricityPrice));
            }

            // Create the line series with styling
            ElectricityPriceSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "Winter Electricity Price",
                    Values = hourlyPriceValues,
                    Fill = null,
                    GeometrySize = 8,
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue),
                    GeometryStroke = new SolidColorPaint(SKColors.DodgerBlue),
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    LineSmoothness = 0.5, // Makes the line slightly curved for better visualization
                    TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:HH:mm}: {((DateTimePoint)point.Model).Value} €/MWh"
                }
            };

            // Configure the X-axis (time axis)
            ElectricityPriceXAxes = new List<Axis>
            {
                new Axis
                {
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    Labeler = value => new DateTime((long)value).ToString("HH:mm"),
                    UnitWidth = TimeSpan.FromHours(1).Ticks,
                    MinStep = TimeSpan.FromHours(2).Ticks // Show every 2 hours for readability
                }
            };

            // Configure the Y-axis
            ElectricityPriceYAxes = new List<Axis>
            {
                new Axis
                {
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    MinLimit = 0, // Start Y-axis at 0
                    Labeler = value => $"{value} €/MWh"
                }
            };
        }

        private void InitializeSummerPriceChart()
        {
            // Filter summer data (August data from the CSV)
            var summerData = _timeSeriesData
                .Where(d => d.TimeFrom.Month == 8) // August data represents summer
                .OrderBy(d => d.TimeFrom)
                .ToList();

            // Create data points for the Summer Electricity Price chart
            var hourlyPriceValues = new ObservableCollection<DateTimePoint>();
            
            foreach (var data in summerData)
            {
                hourlyPriceValues.Add(new DateTimePoint(data.TimeFrom, data.ElectricityPrice));
            }

            // Create the line series with styling
            ElectricityPriceSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "Summer Electricity Price",
                    Values = hourlyPriceValues,
                    Fill = null,
                    GeometrySize = 8,
                    Stroke = new SolidColorPaint(SKColors.OrangeRed),
                    GeometryStroke = new SolidColorPaint(SKColors.OrangeRed),
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    LineSmoothness = 0.5, // Makes the line slightly curved for better visualization
                    TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:HH:mm}: {((DateTimePoint)point.Model).Value} €/MWh"
                }
            };

            // Configure the X-axis (time axis)
            ElectricityPriceXAxes = new List<Axis>
            {
                new Axis
                {
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    Labeler = value => new DateTime((long)value).ToString("HH:mm"),
                    UnitWidth = TimeSpan.FromHours(1).Ticks,
                    MinStep = TimeSpan.FromHours(2).Ticks // Show every 2 hours for readability
                }
            };

            // Configure the Y-axis
            ElectricityPriceYAxes = new List<Axis>
            {
                new Axis
                {
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    MinLimit = 0, // Start Y-axis at 0
                    Labeler = value => $"{value} €/MWh"
                }
            };
        }

        #endregion
    }
}
