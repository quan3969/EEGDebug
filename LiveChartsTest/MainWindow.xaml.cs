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
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ConfigEEG eeg = new ConfigEEG();
        public MainWindow()  
        {
            InitializeComponent();

            Thread thread1 = new Thread(TreadOne)
            {
                IsBackground = true
            };

            Thread thread2 = new Thread(TreadTwo)
            {
                IsBackground = true
            };

            eeg.SetData();

            DataContext = this;

            thread1.Start();
            thread2.Start();
        } 

        public void TreadOne()
        {
            while (true)
            {

                eeg.AddDate();
                Thread.Sleep(100);
                eeg.RemoveData();
            }
        }

        public void TreadTwo()
        {
        }


    }

}
