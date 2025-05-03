using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace SP2.ViewModels
{
    public class ElectricityViewModel : ViewModelBase
    {
        private string _selectedSeason = "Winter";
        private ObservableCollection<ISeries> _electricityPriceSeries;
        private string _chartTitle = "Electricity prices time series - Winter";

        public ElectricityViewModel()
        {
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
            // Create sample data for the Winter Electricity Price chart
            // In a real application, this would come from your data source
            var hourlyPriceValues = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2025, 1, 1, 0, 0, 0), 42),
                new DateTimePoint(new DateTime(2025, 1, 1, 1, 0, 0), 40),
                new DateTimePoint(new DateTime(2025, 1, 1, 2, 0, 0), 38),
                new DateTimePoint(new DateTime(2025, 1, 1, 3, 0, 0), 35),
                new DateTimePoint(new DateTime(2025, 1, 1, 4, 0, 0), 36),
                new DateTimePoint(new DateTime(2025, 1, 1, 5, 0, 0), 40),
                new DateTimePoint(new DateTime(2025, 1, 1, 6, 0, 0), 45),
                new DateTimePoint(new DateTime(2025, 1, 1, 7, 0, 0), 52),
                new DateTimePoint(new DateTime(2025, 1, 1, 8, 0, 0), 60),
                new DateTimePoint(new DateTime(2025, 1, 1, 9, 0, 0), 65),
                new DateTimePoint(new DateTime(2025, 1, 1, 10, 0, 0), 68),
                new DateTimePoint(new DateTime(2025, 1, 1, 11, 0, 0), 70),
                new DateTimePoint(new DateTime(2025, 1, 1, 12, 0, 0), 72),
                new DateTimePoint(new DateTime(2025, 1, 1, 13, 0, 0), 71),
                new DateTimePoint(new DateTime(2025, 1, 1, 14, 0, 0), 69),
                new DateTimePoint(new DateTime(2025, 1, 1, 15, 0, 0), 68),
                new DateTimePoint(new DateTime(2025, 1, 1, 16, 0, 0), 70),
                new DateTimePoint(new DateTime(2025, 1, 1, 17, 0, 0), 75),
                new DateTimePoint(new DateTime(2025, 1, 1, 18, 0, 0), 80),
                new DateTimePoint(new DateTime(2025, 1, 1, 19, 0, 0), 78),
                new DateTimePoint(new DateTime(2025, 1, 1, 20, 0, 0), 72),
                new DateTimePoint(new DateTime(2025, 1, 1, 21, 0, 0), 65),
                new DateTimePoint(new DateTime(2025, 1, 1, 22, 0, 0), 58),
                new DateTimePoint(new DateTime(2025, 1, 1, 23, 0, 0), 50)
            };

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
            // Create sample data for the Summer Electricity Price chart
            // In a real application, this would come from your data source
            var hourlyPriceValues = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2025, 7, 1, 0, 0, 0), 30),
                new DateTimePoint(new DateTime(2025, 7, 1, 1, 0, 0), 28),
                new DateTimePoint(new DateTime(2025, 7, 1, 2, 0, 0), 25),
                new DateTimePoint(new DateTime(2025, 7, 1, 3, 0, 0), 22),
                new DateTimePoint(new DateTime(2025, 7, 1, 4, 0, 0), 20),
                new DateTimePoint(new DateTime(2025, 7, 1, 5, 0, 0), 22),
                new DateTimePoint(new DateTime(2025, 7, 1, 6, 0, 0), 25),
                new DateTimePoint(new DateTime(2025, 7, 1, 7, 0, 0), 30),
                new DateTimePoint(new DateTime(2025, 7, 1, 8, 0, 0), 35),
                new DateTimePoint(new DateTime(2025, 7, 1, 9, 0, 0), 40),
                new DateTimePoint(new DateTime(2025, 7, 1, 10, 0, 0), 45),
                new DateTimePoint(new DateTime(2025, 7, 1, 11, 0, 0), 50),
                new DateTimePoint(new DateTime(2025, 7, 1, 12, 0, 0), 55),
                new DateTimePoint(new DateTime(2025, 7, 1, 13, 0, 0), 58),
                new DateTimePoint(new DateTime(2025, 7, 1, 14, 0, 0), 60),
                new DateTimePoint(new DateTime(2025, 7, 1, 15, 0, 0), 62),
                new DateTimePoint(new DateTime(2025, 7, 1, 16, 0, 0), 60),
                new DateTimePoint(new DateTime(2025, 7, 1, 17, 0, 0), 58),
                new DateTimePoint(new DateTime(2025, 7, 1, 18, 0, 0), 55),
                new DateTimePoint(new DateTime(2025, 7, 1, 19, 0, 0), 50),
                new DateTimePoint(new DateTime(2025, 7, 1, 20, 0, 0), 45),
                new DateTimePoint(new DateTime(2025, 7, 1, 21, 0, 0), 40),
                new DateTimePoint(new DateTime(2025, 7, 1, 22, 0, 0), 35),
                new DateTimePoint(new DateTime(2025, 7, 1, 23, 0, 0), 32)
            };

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
