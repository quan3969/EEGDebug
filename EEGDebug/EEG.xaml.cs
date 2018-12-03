using System;
using System.Windows;
using System.IO.Ports;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Threading; 

namespace EEGDebug
{
    /// <summary>
    /// EEG.xaml 的交互逻辑
    /// </summary>
    public partial class EEG : Window
    {
        public static byte[] rxByte = new byte[32];
        public static int[] rxData = new int[8];
        public static bool dataIsReady = false;
        Thread thread1;
        Thread thread2;
        public EEG()
        {
            InitializeComponent();
            #region SetData

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

            #endregion SetData 

            MainWindow.serialPort1.Write("A");
            thread1 = new Thread(TreadOne);
            thread2 = new Thread(TreadTwo);
            DataContext = this;

            thread1.Start();
            thread2.Start();
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


        public void TreadOne()
        {
            while (true)
            {
                CHOne[0].Values.Add(rxData[0]);
                CHTwo[0].Values.Add(rxData[1]);
                CHThree[0].Values.Add(rxData[2]);
                CHFour[0].Values.Add(rxData[3]);
                CHFive[0].Values.Add(rxData[4]);
                CHSix[0].Values.Add(rxData[5]);
                CHSeven[0].Values.Add(rxData[6]);
                CHEight[0].Values.Add(rxData[7]);
                if (CHOne[0].Values.Count >= 500)
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
                Thread.Sleep(1);
                dataIsReady = true;
            }
        }

        public void TreadTwo()
        {
            int temp = 0;
            while (true)
            {
                if (dataIsReady)
                {
                    MainWindow.serialPort1.ReadLine();
                    MainWindow.serialPort1.Read(rxByte, 0, 32);
                    for (int i = 0; i < 8; i++)
                    {
                        rxData[i] = 0;
                        for (int j = 0; j < 4; j++)
                        {
                            temp = 0;
                            temp |= rxByte[j + i * 4];
                            temp <<= 8 * (3 - j);
                            rxData[i] |= temp;
                        }
                    }
                    dataIsReady = false;
                }
            }
        }
        /// <summary>
        /// 窗口关闭时发送“D”
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.serialPort1.Write("D");
            thread1.Abort();
            thread2.Abort();
            Thread.Sleep(100);
            MainWindow.serialPort1.ReadExisting();
            MainWindow.mainWindow.Show();
        }
    }
}
