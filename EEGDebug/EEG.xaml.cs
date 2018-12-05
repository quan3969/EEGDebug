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
        public static double[] rxDouble = new double[8];
        Thread thread1;
        Thread thread2;
        public EEG()
        {
            InitializeComponent();
            #region SetData

            
            CHOne = new SeriesCollection
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    // 线不需要阴影
                    Fill = Brushes.Transparent,
                    // 点不需要标注
                    PointGeometry = null,
                    Values = new ChartValues<double> ()
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
                    Values = new ChartValues<double> ()
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
                    Values = new ChartValues<double> ()
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
                    Values = new ChartValues<double> ()
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
                    Values = new ChartValues<double> ()
                }
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
                    Values = new ChartValues<double> ()
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
                    Values = new ChartValues<double> ()
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
                    Values = new ChartValues<double> ()
                },
            };

            #endregion SetData 

            MainWindow.serialPort1.Write("A");
            thread1 = new Thread(TreadOne);
            thread2 = new Thread(TreadTwo);
            DataContext = this;

            thread1.Start();
            //thread2.Start();
        }

        public SeriesCollection CHOne { get; set; }
        public SeriesCollection CHTwo { get; set; }
        public SeriesCollection CHThree { get; set; }
        public SeriesCollection CHFour { get; set; }
        public SeriesCollection CHFive { get; set; }
        public SeriesCollection CHSix { get; set; }
        public SeriesCollection CHSeven { get; set; }
        public SeriesCollection CHEight { get; set; }

        public void TreadOne()
        {
            int intTemp = 0;

            while (true)
            {
                #region 从串口中读取数据并格式化成 double 型

                MainWindow.serialPort1.ReadLine();
                MainWindow.serialPort1.Read(rxByte, 0, 32);

                for (int i = 0; i < 8; i++)
                {
                    rxData[i] = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        intTemp = 0;
                        intTemp |= rxByte[k + i * 4];
                        intTemp <<= 8 * (3 - k);
                        rxData[i] |= intTemp;
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    rxDouble[i] = 0;
                    rxDouble[i] = rxData[i] / 100.663296;
                }

                #endregion 从串口中读取数据并格式化成 double 型

                CHOne[0].Values.Add(rxDouble[0]);
                CHTwo[0].Values.Add(rxDouble[1]);
                CHThree[0].Values.Add(rxDouble[2]);
                CHFour[0].Values.Add(rxDouble[3]);
                CHFive[0].Values.Add(rxDouble[4]);
                CHSix[0].Values.Add(rxDouble[5]);
                CHSeven[0].Values.Add(rxDouble[6]);
                CHEight[0].Values.Add(rxDouble[7]);
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
            }
        }

        public void TreadTwo()
        {
            while (true)
            {

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
