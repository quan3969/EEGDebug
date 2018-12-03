using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
namespace LiveChartsTest
{
    class ConfigEEG
    {
        public void SetData()
        {
            Labels = new[] { " ", " ", " ", " ", " " };

            CHOne = new SeriesCollection
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    // 线不需要阴影
                    Fill = Brushes.Transparent,
                    // 点不需要标注
                    PointGeometry = null,
                    Values = new ChartValues<int> { 0, 0, 0, 0 }
                },
            };
            
            CHTwo = new SeriesCollection
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    // 线不需要阴影
                    Fill = Brushes.Transparent,
                    // 点不需要标注
                    PointGeometry = null,
                    Values = new ChartValues<int> { 0, 0, 0, 0 }
                },
            };

            CHThree = new SeriesCollection
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    // 线不需要阴影
                    Fill = Brushes.Transparent,
                    // 点不需要标注
                    PointGeometry = null,
                    Values = new ChartValues<int> { 0, 0, 0, 0 }
                },
            };

            CHFour = new SeriesCollection
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    // 线不需要阴影
                    Fill = Brushes.Transparent,
                    // 点不需要标注
                    PointGeometry = null,
                    Values = new ChartValues<int> { 0, 0, 0, 0 }
                },
            };

            CHFive = new SeriesCollection
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    // 线不需要阴影
                    Fill = Brushes.Transparent,
                    // 点不需要标注
                    PointGeometry = null,
                    Values = new ChartValues<int> { 0, 0, 0, 0 }
                },
            };

            CHSix = new SeriesCollection
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    // 线不需要阴影
                    Fill = Brushes.Transparent,
                    // 点不需要标注
                    PointGeometry = null,
                    Values = new ChartValues<int> { 0, 0, 0, 0 }
                },
            };

            CHSeven = new SeriesCollection
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    // 线不需要阴影
                    Fill = Brushes.Transparent,
                    // 点不需要标注
                    PointGeometry = null,
                    Values = new ChartValues<int> { 0, 0, 0, 0 }
                },
            };

            CHEight = new SeriesCollection
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    // 线不需要阴影
                    Fill = Brushes.Transparent,
                    // 点不需要标注
                    PointGeometry = null,
                    Values = new ChartValues<int> { 0, 0, 0, 0 }
                },
            };
        }
        
        public void AddDate()
        {
            Random rand = new Random();
            CHOne[0].Values.Add(rand.Next());
            CHTwo[0].Values.Add(rand.Next());
            CHThree[0].Values.Add(rand.Next());
            CHFour[0].Values.Add(rand.Next());
            CHFive[0].Values.Add(rand.Next());
            CHSix[0].Values.Add(rand.Next());
            CHSeven[0].Values.Add(rand.Next());
            CHEight[0].Values.Add(rand.Next());
        }

        public void RemoveData()
        {
            if (CHOne[0].Values.Count >= 120)
            {
                CHOne[0].Values.RemoveAt(0);
                CHTwo[0].Values.RemoveAt(0);
                CHThree[0].Values.RemoveAt(0);
                CHFour[0].Values.RemoveAt(0);
                CHFive[0].Values.RemoveAt(0);
                CHSix[0].Values.RemoveAt(0);
                CHSeven[0].Values.RemoveAt(0);
                CHEight[0].Values.RemoveAt(0);
            }
        }

        public SeriesCollection CHOne { get; set; }
        public SeriesCollection CHTwo { get; set; }
        public SeriesCollection CHThree { get; set; }
        public SeriesCollection CHFour { get; set; }
        public SeriesCollection CHFive { get; set; }
        public SeriesCollection CHSix { get; set; }
        public SeriesCollection CHSeven { get; set; }
        public SeriesCollection CHEight { get; set; }
        public string[] Labels { get; set; }
    }
        
}
