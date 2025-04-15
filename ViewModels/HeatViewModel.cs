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
    public class HeatViewModel : ViewModelBase
    {
        public HeatViewModel()
        {
            // Initialize the chart data
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
            // Create sample data for the Heat Demand line chart
            // In a real application, this would come from your data source
            var monthlyDemandValues = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2025, 1, 1), 120),
                new DateTimePoint(new DateTime(2025, 2, 1), 132),
                new DateTimePoint(new DateTime(2025, 3, 1), 101),
                new DateTimePoint(new DateTime(2025, 4, 1), 89),
                new DateTimePoint(new DateTime(2025, 5, 1), 62),
                new DateTimePoint(new DateTime(2025, 6, 1), 45),
                new DateTimePoint(new DateTime(2025, 7, 1), 40),
                new DateTimePoint(new DateTime(2025, 8, 1), 38),
                new DateTimePoint(new DateTime(2025, 9, 1), 57),
                new DateTimePoint(new DateTime(2025, 10, 1), 78),
                new DateTimePoint(new DateTime(2025, 11, 1), 98),
                new DateTimePoint(new DateTime(2025, 12, 1), 110)
            };

            // Create the line series with styling
            HeatDemandSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<DateTimePoint>
                {
                    // Name = "Heat Demand (MWh)",
                    Values = monthlyDemandValues,
                    Fill = null,
                    GeometrySize = 8,
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue),
                    GeometryStroke = new SolidColorPaint(SKColors.DodgerBlue),
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    LineSmoothness = 0.5, // Makes the line slightly curved for better visualization
                    TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:MMM yyyy}: {((DateTimePoint)point.Model).Value} MWh"
                }
            };

            // Configure the X-axis (time axis)
            HeatDemandXAxes = new List<Axis>
            {
                new Axis
                {
                    // Name = "Month",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    Labeler = value => new DateTime((long)value).ToString("MMM"),
                    UnitWidth = TimeSpan.FromDays(30).Ticks,
                    MinStep = TimeSpan.FromDays(30).Ticks
                }
            };

            // Configure the Y-axis
            HeatDemandYAxes = new List<Axis>
            {
                new Axis
                {
                    // Name = "Heat Demand (MWh)",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    MinLimit = 0, // Start Y-axis at 0
                    Labeler = value => $"{value} MWh"
                }
            };
        }

        private void InitializeHeatProductionChart()
        {
            // Heat Pump data
            var heatPumpValues = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2025, 1, 1), 70),
                new DateTimePoint(new DateTime(2025, 2, 1), 75),
                new DateTimePoint(new DateTime(2025, 3, 1), 60),
                new DateTimePoint(new DateTime(2025, 4, 1), 55),
                new DateTimePoint(new DateTime(2025, 5, 1), 40),
                new DateTimePoint(new DateTime(2025, 6, 1), 30),
                new DateTimePoint(new DateTime(2025, 7, 1), 25),
                new DateTimePoint(new DateTime(2025, 8, 1), 23),
                new DateTimePoint(new DateTime(2025, 9, 1), 35),
                new DateTimePoint(new DateTime(2025, 10, 1), 45),
                new DateTimePoint(new DateTime(2025, 11, 1), 60),
                new DateTimePoint(new DateTime(2025, 12, 1), 65)
            };
            
            // Gas Boiler data
            var gasBoilerValues = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2025, 1, 1), 50),
                new DateTimePoint(new DateTime(2025, 2, 1), 57),
                new DateTimePoint(new DateTime(2025, 3, 1), 41),
                new DateTimePoint(new DateTime(2025, 4, 1), 34),
                new DateTimePoint(new DateTime(2025, 5, 1), 22),
                new DateTimePoint(new DateTime(2025, 6, 1), 15),
                new DateTimePoint(new DateTime(2025, 7, 1), 15),
                new DateTimePoint(new DateTime(2025, 8, 1), 15),
                new DateTimePoint(new DateTime(2025, 9, 1), 22),
                new DateTimePoint(new DateTime(2025, 10, 1), 33),
                new DateTimePoint(new DateTime(2025, 11, 1), 38),
                new DateTimePoint(new DateTime(2025, 12, 1), 45)
            };
            
            // Electric Boiler data
            var electricBoilerValues = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2025, 1, 1), 30),
                new DateTimePoint(new DateTime(2025, 2, 1), 28),
                new DateTimePoint(new DateTime(2025, 3, 1), 25),
                new DateTimePoint(new DateTime(2025, 4, 1), 20),
                new DateTimePoint(new DateTime(2025, 5, 1), 15),
                new DateTimePoint(new DateTime(2025, 6, 1), 10),
                new DateTimePoint(new DateTime(2025, 7, 1), 8),
                new DateTimePoint(new DateTime(2025, 8, 1), 7),
                new DateTimePoint(new DateTime(2025, 9, 1), 12),
                new DateTimePoint(new DateTime(2025, 10, 1), 18),
                new DateTimePoint(new DateTime(2025, 11, 1), 25),
                new DateTimePoint(new DateTime(2025, 12, 1), 32)
            };
            
            // Solar Thermal data
            var solarThermalValues = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2025, 1, 1), 5),
                new DateTimePoint(new DateTime(2025, 2, 1), 8),
                new DateTimePoint(new DateTime(2025, 3, 1), 15),
                new DateTimePoint(new DateTime(2025, 4, 1), 22),
                new DateTimePoint(new DateTime(2025, 5, 1), 30),
                new DateTimePoint(new DateTime(2025, 6, 1), 35),
                new DateTimePoint(new DateTime(2025, 7, 1), 38),
                new DateTimePoint(new DateTime(2025, 8, 1), 36),
                new DateTimePoint(new DateTime(2025, 9, 1), 28),
                new DateTimePoint(new DateTime(2025, 10, 1), 18),
                new DateTimePoint(new DateTime(2025, 11, 1), 10),
                new DateTimePoint(new DateTime(2025, 12, 1), 4)
            };
            
            // District Heating data
            var districtHeatingValues = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2025, 1, 1), 25),
                new DateTimePoint(new DateTime(2025, 2, 1), 22),
                new DateTimePoint(new DateTime(2025, 3, 1), 18),
                new DateTimePoint(new DateTime(2025, 4, 1), 15),
                new DateTimePoint(new DateTime(2025, 5, 1), 10),
                new DateTimePoint(new DateTime(2025, 6, 1), 5),
                new DateTimePoint(new DateTime(2025, 7, 1), 3),
                new DateTimePoint(new DateTime(2025, 8, 1), 4),
                new DateTimePoint(new DateTime(2025, 9, 1), 8),
                new DateTimePoint(new DateTime(2025, 10, 1), 12),
                new DateTimePoint(new DateTime(2025, 11, 1), 18),
                new DateTimePoint(new DateTime(2025, 12, 1), 24)
            };

            // Create the area series with styling
            HeatProductionSeries = new ObservableCollection<ISeries>
            {
                new StackedAreaSeries<DateTimePoint>
                {
                    Name = "Gas Boiler 1",
                    Values = heatPumpValues,
                    Stroke = new SolidColorPaint(SKColors.MediumSeaGreen),
                    Fill = new SolidColorPaint(SKColors.MediumSeaGreen.WithAlpha(150)),
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:MMM yyyy}: {((DateTimePoint)point.Model).Value} MWh"
                },
                new StackedAreaSeries<DateTimePoint>
                {
                    Name = "Gas Boiler 2",
                    Values = gasBoilerValues,
                    Stroke = new SolidColorPaint(SKColors.OrangeRed),
                    Fill = new SolidColorPaint(SKColors.OrangeRed.WithAlpha(150)),
                    GeometrySize = 0, // No points on the line
                    LineSmoothness = 0.5,
                    TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:MMM yyyy}: {((DateTimePoint)point.Model).Value} MWh"
                },
                new StackedAreaSeries<DateTimePoint>
                {
                    Name = "Oil Boiler",
                    Values = electricBoilerValues,
                    Stroke = new SolidColorPaint(SKColors.RoyalBlue),
                    Fill = new SolidColorPaint(SKColors.RoyalBlue.WithAlpha(150)),
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:MMM yyyy}: {((DateTimePoint)point.Model).Value} MWh"
                },
                new StackedAreaSeries<DateTimePoint>
                {
                    Name = "Gas Motor",
                    Values = solarThermalValues,
                    Stroke = new SolidColorPaint(SKColors.Gold),
                    Fill = new SolidColorPaint(SKColors.Gold.WithAlpha(150)),
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:MMM yyyy}: {((DateTimePoint)point.Model).Value} MWh"
                },
                new StackedAreaSeries<DateTimePoint>
                {
                    Name = "Heat Pump",
                    Values = districtHeatingValues,
                    Stroke = new SolidColorPaint(SKColors.Purple),
                    Fill = new SolidColorPaint(SKColors.Purple.WithAlpha(150)),
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    TooltipLabelFormatter = point => $"{((DateTimePoint)point.Model).DateTime:MMM yyyy}: {((DateTimePoint)point.Model).Value} MWh"
                }
            };

            // Configure the X-axis (time axis)
            HeatProductionXAxes = new List<Axis>
            {
                new Axis
                {
                    // Name = "Month",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    Labeler = value => new DateTime((long)value).ToString("MMM"),
                    UnitWidth = TimeSpan.FromDays(30).Ticks,
                    MinStep = TimeSpan.FromDays(30).Ticks
                }
            };

            // Configure the Y-axis
            HeatProductionYAxes = new List<Axis>
            {
                new Axis
                {
                    // Name = "Heat Production (MWh)",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    MinLimit = 0, // Start Y-axis at 0
                    Labeler = value => $"{value} MWh"
                }
            };
        }

        #endregion
    }
}
