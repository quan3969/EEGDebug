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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SerialPort serialPort1 = new SerialPort();
        public static MainWindow mainWindow;
        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
        }

        /// <summary>
        /// “连接”按键按下，先获取当前的串口号和波特率，然后打开或关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButton_connect_clicked(object sender, RoutedEventArgs e)
        {
            if(Convert.ToBoolean(toggleButton_connect.IsChecked))
            {
                try
                {
                    serialPort1.PortName = comboBox_port.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox_baudRate.Text);
                    serialPort1.DataBits = Convert.ToInt32(comBox_dataBits.Text);
                    switch(comBo_parity.Text)
                    {
                        case "None":
                            serialPort1.Parity = Parity.None;
                            break;
                        case "Odd":
                            serialPort1.Parity = Parity.Odd;
                            break;
                        case "Even":
                            serialPort1.Parity = Parity.Even;
                            break;
                        case "Mark":
                            serialPort1.Parity = Parity.Mark;
                            break;
                        case "Space":
                            serialPort1.Parity = Parity.Space;
                            break;
                        default:
                            serialPort1.Parity = Parity.None;
                            break;
                    }
                    switch(comBox_stopBits.Text)
                    {
                        case "1":
                            serialPort1.StopBits = StopBits.One;
                            break;
                        case "2":
                            serialPort1.StopBits = StopBits.Two;
                            break;
                        default:
                            serialPort1.StopBits = StopBits.One;
                            break;
                    }
                    serialPort1.ReadTimeout = 2000;
                    serialPort1.Open();
                    toggleButton_enableBle.IsChecked = true;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "串口打开失败！");
                    toggleButton_connect.IsChecked = false;
                    toggleButton_enableBle.IsChecked = false;
                }
            }
            else
            {
                serialPort1.Close();
            }
            
        }
        /// <summary>
        /// “串口号” comboBox 展开事件，展开时获取电脑当前可用的串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_port_dropDownOpened(object sender, System.EventArgs e)
        {
            comboBox_port.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            for(int i=0; i<ports.Length; i++)
            {
                comboBox_port.Items.Add(ports[i]);
            }
        }
        /// <summary>
        /// “获取”按键按下，先检查串口是否连接，串口连接则从串口获取当前寄存器值，并将值同步到
        /// 所有设置当中。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_getAdsCfg_clicked(object sender, RoutedEventArgs e)
        {
            byte[] regValue = new byte[24];
            if (serialPort1.IsOpen)
            {
                // 发送字符‘B’，获得当前所有寄存器的值
                serialPort1.ReadExisting();
                serialPort1.Write("B");
                try
                {
                    serialPort1.Read(regValue, 0, 24);
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message, "获取失败!");
                }

                #region 根据当前按寄存器值设置显示内容
                // 设备ID
                textBox_adsID.Text = "0x" + string.Format("{0:X}", (regValue[0]));
                // 测试信号
                // 输入源
                switch ((regValue[2] & 0x10) >> 4)
                {
                    case 0:
                        comboBox_testSignal_source.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_testSignal_source.SelectedIndex = 1;
                        break;
                }
                // 振幅
                switch ((regValue[2] & 0x04) >> 2)
                {
                    case 0:
                        comboBox_testSignal_amplitude.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_testSignal_amplitude.SelectedIndex = 1;
                        break;
                }
                // 频率
                switch (regValue[2] & 0x03)
                {
                    case 0:
                        comboBox_testSingal_frequency.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_testSingal_frequency.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_testSingal_frequency.SelectedIndex = 2;
                        break;
                }
                // 通道设置
                // 开关
                switch ((regValue[5] & 0x80) >> 7)
                {
                    case 0:
                        checkBox_ch1_isOpen.IsChecked = true;
                        break;
                    case 1:
                        checkBox_ch1_isOpen.IsChecked = false;
                        break;
                }
                switch ((regValue[6] & 0x80) >> 7)
                {
                    case 0:
                        checkBox_ch2_isOpen.IsChecked = true;
                        break;
                    case 1:
                        checkBox_ch2_isOpen.IsChecked = false;
                        break;
                }
                switch ((regValue[7] & 0x80) >> 7)
                {
                    case 0:
                        checkBox_ch3_isOpen.IsChecked = true;
                        break;
                    case 1:
                        checkBox_ch3_isOpen.IsChecked = false;
                        break;
                }
                switch ((regValue[8] & 0x80) >> 7)
                {
                    case 0:
                        checkBox_ch4_isOpen.IsChecked = true;
                        break;
                    case 1:
                        checkBox_ch4_isOpen.IsChecked = false;
                        break;
                }
                switch ((regValue[9] & 0x80) >> 7)
                {
                    case 0:
                        checkBox_ch5_isOpen.IsChecked = true;
                        break;
                    case 1:
                        checkBox_ch5_isOpen.IsChecked = false;
                        break;
                }
                switch ((regValue[10] & 0x80) >> 7)
                {
                    case 0:
                        checkBox_ch6_isOpen.IsChecked = true;
                        break;
                    case 1:
                        checkBox_ch6_isOpen.IsChecked = false;
                        break;
                }
                switch ((regValue[11] & 0x80) >> 7)
                {
                    case 0:
                        checkBox_ch7_isOpen.IsChecked = true;
                        break;
                    case 1:
                        checkBox_ch7_isOpen.IsChecked = false;
                        break;
                }
                switch ((regValue[12] & 0x80) >> 7)
                {
                    case 0:
                        checkBox_ch8_isOpen.IsChecked = true;
                        break;
                    case 1:
                        checkBox_ch8_isOpen.IsChecked = false;
                        break;
                }
                // Gain
                switch ((regValue[5] & 0x70) >> 4)
                {
                    case 0:
                        comboBox_ch1_gain.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch1_gain.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch1_gain.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch1_gain.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch1_gain.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch1_gain.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch1_gain.SelectedIndex = 6;
                        break;
                }
                switch ((regValue[6] & 0x70) >> 4)
                {
                    case 0:
                        comboBox_ch2_gain.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch2_gain.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch2_gain.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch2_gain.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch2_gain.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch2_gain.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch2_gain.SelectedIndex = 6;
                        break;
                }
                switch ((regValue[7] & 0x70) >> 4)
                {
                    case 0:
                        comboBox_ch3_gain.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch3_gain.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch3_gain.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch3_gain.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch3_gain.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch3_gain.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch3_gain.SelectedIndex = 6;
                        break;
                }
                switch ((regValue[8] & 0x70) >> 4)
                {
                    case 0:
                        comboBox_ch4_gain.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch4_gain.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch4_gain.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch4_gain.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch4_gain.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch4_gain.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch4_gain.SelectedIndex = 6;
                        break;
                }
                switch ((regValue[9] & 0x70) >> 4)
                {
                    case 0:
                        comboBox_ch5_gain.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch5_gain.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch5_gain.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch5_gain.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch5_gain.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch5_gain.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch5_gain.SelectedIndex = 6;
                        break;
                }
                switch ((regValue[10] & 0x70) >> 4)
                {
                    case 0:
                        comboBox_ch6_gain.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch6_gain.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch6_gain.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch6_gain.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch6_gain.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch6_gain.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch6_gain.SelectedIndex = 6;
                        break;
                }
                switch ((regValue[11] & 0x70) >> 4)
                {
                    case 0:
                        comboBox_ch7_gain.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch7_gain.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch7_gain.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch7_gain.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch7_gain.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch7_gain.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch7_gain.SelectedIndex = 6;
                        break;
                }
                switch ((regValue[12] & 0x70) >> 4)
                {
                    case 0:
                        comboBox_ch8_gain.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch8_gain.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch8_gain.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch8_gain.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch8_gain.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch8_gain.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch8_gain.SelectedIndex = 6;
                        break;
                }
                // SRB2
                switch ((regValue[5] & 0x08) >> 3)
                {
                    case 0:
                        comboBox_ch1_srb2.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch1_srb2.SelectedIndex = 1;
                        break;
                }
                switch ((regValue[6] & 0x08) >> 3)
                {
                    case 0:
                        comboBox_ch2_srb2.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch2_srb2.SelectedIndex = 1;
                        break;
                }
                switch ((regValue[7] & 0x08) >> 3)
                {
                    case 0:
                        comboBox_ch3_srb2.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch3_srb2.SelectedIndex = 1;
                        break;
                }
                switch ((regValue[8] & 0x08) >> 3)
                {
                    case 0:
                        comboBox_ch4_srb2.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch4_srb2.SelectedIndex = 1;
                        break;
                }
                switch ((regValue[9] & 0x08) >> 3)
                {
                    case 0:
                        comboBox_ch5_srb2.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch5_srb2.SelectedIndex = 1;
                        break;
                }
                switch ((regValue[10] & 0x08) >> 3)
                {
                    case 0:
                        comboBox_ch6_srb2.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch6_srb2.SelectedIndex = 1;
                        break;
                }
                switch ((regValue[11] & 0x08) >> 3)
                {
                    case 0:
                        comboBox_ch7_srb2.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch7_srb2.SelectedIndex = 1;
                        break;
                }
                switch ((regValue[12] & 0x08) >> 3)
                {
                    case 0:
                        comboBox_ch8_srb2.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch8_srb2.SelectedIndex = 1;
                        break;
                }
                // Input
                switch (regValue[5] & 0x07)
                {
                    case 0:
                        comboBox_ch1_input.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch1_input.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch1_input.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch1_input.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch1_input.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch1_input.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch1_input.SelectedIndex = 6;
                        break;
                }
                switch (regValue[6] & 0x07)
                {
                    case 0:
                        comboBox_ch2_input.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch2_input.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch2_input.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch2_input.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch2_input.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch2_input.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch2_input.SelectedIndex = 6;
                        break;
                }
                switch (regValue[7] & 0x07)
                {
                    case 0:
                        comboBox_ch3_input.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch3_input.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch3_input.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch3_input.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch3_input.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch3_input.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch3_input.SelectedIndex = 6;
                        break;
                }
                switch (regValue[8] & 0x07)
                {
                    case 0:
                        comboBox_ch4_input.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch4_input.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch4_input.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch4_input.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch4_input.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch4_input.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch4_input.SelectedIndex = 6;
                        break;
                }
                switch (regValue[9] & 0x07)
                {
                    case 0:
                        comboBox_ch5_input.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch5_input.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch5_input.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch5_input.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch5_input.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch5_input.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch5_input.SelectedIndex = 6;
                        break;
                }
                switch (regValue[10] & 0x07)
                {
                    case 0:
                        comboBox_ch6_input.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch6_input.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch6_input.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch6_input.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch6_input.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch6_input.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch6_input.SelectedIndex = 6;
                        break;
                }
                switch (regValue[11] & 0x07)
                {
                    case 0:
                        comboBox_ch7_input.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch7_input.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch7_input.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch7_input.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch7_input.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch7_input.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch7_input.SelectedIndex = 6;
                        break;
                }
                switch (regValue[12] & 0x07)
                {
                    case 0:
                        comboBox_ch8_input.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_ch8_input.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_ch8_input.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_ch8_input.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_ch8_input.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_ch8_input.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_ch8_input.SelectedIndex = 6;
                        break;
                }
                // 输出速率
                switch (regValue[1] & 0x07)
                {
                    case 0:
                        comboBox_dataRate.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_dataRate.SelectedIndex = 1;
                        break;
                    case 2:
                        comboBox_dataRate.SelectedIndex = 2;
                        break;
                    case 3:
                        comboBox_dataRate.SelectedIndex = 3;
                        break;
                    case 4:
                        comboBox_dataRate.SelectedIndex = 4;
                        break;
                    case 5:
                        comboBox_dataRate.SelectedIndex = 5;
                        break;
                    case 6:
                        comboBox_dataRate.SelectedIndex = 6;
                        break;
                }
                // SRB1
                switch ((regValue[21] & 0x10) >> 4)
                {
                    case 0:
                        comboBox_srb1.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_srb1.SelectedIndex = 1;
                        break;
                }
                // Conversion Mode
                switch ((regValue[23] & 0x08) >> 3)
                {
                    case 0:
                        comboBox_conversionMode.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_conversionMode.SelectedIndex = 1;
                        break;
                }
                // Reference Voltage
                switch ((regValue[3] & 0x80) >> 7)
                {
                    case 0:
                        comboBox_reference.SelectedIndex = 0;
                        break;
                    case 1:
                        comboBox_reference.SelectedIndex = 1;
                        break;
                }
                #endregion 根据当前按寄存器值设置显示内容
            }
            else
            {
                MessageBox.Show("请先连接串口!");
            }
        }
        /// <summary>
        /// “应用”按键按下，读取当前用户设置，并发送到下位机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_applyAdsCfg_clicked(object sender, RoutedEventArgs e)
        {
            char[] charToSend = { 'C' };
            byte[] regValue = {
                /* ID     	  */        0x00,	// Read only hopefully 0x3E.
	            /* CONFIG1 	  */		0x96,	// Daisy-chain mode, CLKout disable, 250SPS.
	            /* CONFIG2 	  */		0xC0,	// Test signal off.
	            /* CONFIG3 	  */		0xEC,	// Internal reference on, Bias measurement off, Bias reference signal internal, Bias on, Bias detection off, Read status of bias.
	            /* LOFF 	  */		0x00,   // Lead-off detection control default.
	            /* CH1SET 	  */		0x60,	// Power on, Gain 24, SRB2 disconnect, Input normal electorde.
	            /* CH2SET 	  */		0x60,
	            /* CH3SET 	  */		0x60,
	            /* CH4SET 	  */		0x60,
	            /* CH5SET 	  */		0x60,
	            /* CH6SET 	  */		0x60,
	            /* CH7SET     */		0x60,
	            /* CH8SET     */		0x60,
	            /* BIAS_SENSP */		0xFF,   // Bias positive 1~8 connect.
	            /* BIAS_SENSN */		0xFF,	// Bias negative 1~8 connect.
	            /* LOFF_SENSP */		0x00,   // Lead-off positive 1~8 disconnect.
	            /* LOFF_SENSN */		0x00,   // Lead-off negative 1~8 disconnect.
	            /* LOFF_FLIP  */		0x00,	// Lead-off flip default.
	            /* LOFF_STATP */		0x00,	// Read only, status of the lead-off positive status.
	            /* LOFF_STATN */		0x00,	// Read only, status of the lead-off negative status.
	            /* GPIO 	  */		0xFF,   // GPIO input.
	            /* MISC1 	  */		0x20,	// SRB1 connect.
	            /* MISC2 	  */		0x00,	// Reserved always 0x00.
	            /* CONFIG4 	  */		0x00,	// Single shot off, Lead-off off.
            };
            #region 读取当前用户设置
            // 测试信号
            // 输入源
            switch (comboBox_testSignal_source.SelectedIndex)
            {
                case 0:
                    regValue[2] &= 0xEF;
                    break;
                case 1:
                    regValue[2] |= 0x10;
                    break;
            }
            // 振幅
            switch (comboBox_testSignal_amplitude.SelectedIndex)
            {
                case 0:
                    regValue[2] &= 0xF7;
                    break;
                case 1:
                    regValue[2] |= 0x08;
                    break;
            }
            // 频率
            switch (comboBox_testSingal_frequency.SelectedIndex)
            {
                case 0:
                    regValue[2] &= 0xFC;
                    break;
                case 1:
                    regValue[2] &= 0xFD;
                    regValue[2] |= 0x01;
                    break;
                case 2:
                    regValue[2] |= 0x02;
                    regValue[2] &= 0xFE;
                    break;
            }
            // 通道设置
            // 开关
            #region 通道开关
            if (Convert.ToBoolean(checkBox_ch1_isOpen.IsChecked))
            {
                regValue[5] &= 0x7F;
            }
            else
            {
                regValue[5] |= 0x80;
            }

            if (Convert.ToBoolean(checkBox_ch2_isOpen.IsChecked))
            {
                regValue[6] &= 0x7F;
            }
            else
            {
                regValue[6] |= 0x80;
            }

            if (Convert.ToBoolean(checkBox_ch3_isOpen.IsChecked))
            {
                regValue[7] &= 0x7F;
            }
            else
            {
                regValue[7] |= 0x80;
            }

            if (Convert.ToBoolean(checkBox_ch4_isOpen.IsChecked))
            {
                regValue[8] &= 0x7F;
            }
            else
            {
                regValue[8] |= 0x80;
            }

            if (Convert.ToBoolean(checkBox_ch5_isOpen.IsChecked))
            {
                regValue[9] &= 0x7F;
            }
            else
            {
                regValue[9] |= 0x80;
            }

            if (Convert.ToBoolean(checkBox_ch6_isOpen.IsChecked))
            {
                regValue[10] &= 0x7F;
            }
            else
            {
                regValue[10] |= 0x80;
            }

            if (Convert.ToBoolean(checkBox_ch7_isOpen.IsChecked))
            {
                regValue[11] &= 0x7F;
            }
            else
            {
                regValue[11] |= 0x80;
            }

            if (Convert.ToBoolean(checkBox_ch8_isOpen.IsChecked))
            {
                regValue[12] &= 0x7F;
            }
            else
            {
                regValue[12] |= 0x80;
            }
            #endregion 通道开关
            // Gain
            switch (comboBox_ch1_gain.SelectedIndex)
            {
                case 0: // 000
                    regValue[5] &= 0x8F;
                    break;
                case 1: // 001
                    regValue[5] &= 0x9F;
                    regValue[5] |= 0x10;
                    break;
                case 2: // 010
                    regValue[5] &= 0xAF;
                    regValue[5] |= 0x20;
                    break;
                case 3: // 011
                    regValue[5] &= 0xBF;
                    regValue[5] |= 0x30;
                    break;
                case 4: // 100
                    regValue[5] &= 0xCF;
                    regValue[5] |= 0x40;
                    break;
                case 5: // 101
                    regValue[5] &= 0xDF;
                    regValue[5] |= 0x50;
                    break;
                case 6:
                    regValue[5] &= 0xEF;
                    regValue[5] |= 0x60;
                    break;
            }
            switch (comboBox_ch2_gain.SelectedIndex)
            {
                case 0: // 000
                    regValue[6] &= 0x8F;
                    break;
                case 1: // 001
                    regValue[6] &= 0x9F;
                    regValue[6] |= 0x10;
                    break;
                case 2: // 010
                    regValue[6] &= 0xAF;
                    regValue[6] |= 0x20;
                    break;
                case 3: // 011
                    regValue[6] &= 0xBF;
                    regValue[6] |= 0x30;
                    break;
                case 4: // 100
                    regValue[6] &= 0xCF;
                    regValue[6] |= 0x40;
                    break;
                case 5: // 101
                    regValue[6] &= 0xDF;
                    regValue[6] |= 0x50;
                    break;
                case 6:
                    regValue[6] &= 0xEF;
                    regValue[6] |= 0x60;
                    break;
            }
            switch (comboBox_ch3_gain.SelectedIndex)
            {
                case 0: // 000
                    regValue[7] &= 0x8F;
                    break;
                case 1: // 001
                    regValue[7] &= 0x9F;
                    regValue[7] |= 0x10;

                    break;
                case 2: // 010
                    regValue[7] &= 0xAF;
                    regValue[7] |= 0x20;
                    break;
                case 3: // 011
                    regValue[7] &= 0xBF;
                    regValue[7] |= 0x30;
                    break;
                case 4: // 100
                    regValue[7] &= 0xCF;
                    regValue[7] |= 0x40;
                    break;
                case 5: // 101
                    regValue[7] &= 0xDF;
                    regValue[7] |= 0x50;
                    break;
                case 6:
                    regValue[7] &= 0xEF;
                    regValue[7] |= 0x60;
                    break;
            }
            switch (comboBox_ch4_gain.SelectedIndex)
            {
                case 0: // 000
                    regValue[8] &= 0x8F;
                    break;
                case 1: // 001
                    regValue[8] &= 0x9F;
                    regValue[8] |= 0x10;
                    break;
                case 2: // 010
                    regValue[8] &= 0xAF;
                    regValue[8] |= 0x20;
                    break;
                case 3: // 011
                    regValue[8] &= 0xBF;
                    regValue[8] |= 0x30;
                    break;
                case 4: // 100
                    regValue[8] &= 0xCF;
                    regValue[8] |= 0x40;
                    break;
                case 5: // 101
                    regValue[8] &= 0xDF;
                    regValue[8] |= 0x50;
                    break;
                case 6:
                    regValue[8] &= 0xEF;
                    regValue[8] |= 0x60;
                    break;
            }
            switch (comboBox_ch5_gain.SelectedIndex)
            {
                case 0: // 000
                    regValue[9] &= 0x8F;
                    break;
                case 1: // 001
                    regValue[9] &= 0x9F;
                    regValue[9] |= 0x10;
                    break;
                case 2: // 010
                    regValue[9] &= 0xAF;
                    regValue[9] |= 0x20;
                    break;
                case 3: // 011
                    regValue[9] &= 0xBF;
                    regValue[9] |= 0x30;
                    break;
                case 4: // 100
                    regValue[9] &= 0xCF;
                    regValue[9] |= 0x40;
                    break;
                case 5: // 101
                    regValue[9] &= 0xDF;
                    regValue[9] |= 0x50;
                    break;
                case 6:
                    regValue[9] &= 0xEF;
                    regValue[9] |= 0x60;
                    break;
            }
            switch (comboBox_ch6_gain.SelectedIndex)
            {
                case 0: // 000
                    regValue[10] &= 0x8F;
                    break;
                case 1: // 001
                    regValue[10] &= 0x9F;
                    regValue[10] |= 0x10;
                    break;
                case 2: // 010
                    regValue[10] &= 0xAF;
                    regValue[10] |= 0x20;
                    break;
                case 3: // 011
                    regValue[10] &= 0xBF;
                    regValue[10] |= 0x30;
                    break;
                case 4: // 100
                    regValue[10] &= 0xCF;
                    regValue[10] |= 0x40;
                    break;
                case 5: // 101
                    regValue[10] &= 0xDF;
                    regValue[10] |= 0x50;
                    break;
                case 6:
                    regValue[10] &= 0xEF;
                    regValue[10] |= 0x60;
                    break;
            }
            switch (comboBox_ch7_gain.SelectedIndex)
            {
                case 0: // 000
                    regValue[11] &= 0x8F;
                    break;
                case 1: // 001
                    regValue[11] &= 0x9F;
                    regValue[11] |= 0x10;
                    break;
                case 2: // 010
                    regValue[11] &= 0xAF;
                    regValue[11] |= 0x20;
                    break;
                case 3: // 011
                    regValue[11] &= 0xBF;
                    regValue[11] |= 0x30;
                    break;
                case 4: // 100
                    regValue[11] &= 0xCF;
                    regValue[11] |= 0x40;
                    break;
                case 5: // 101
                    regValue[11] &= 0xDF;
                    regValue[11] |= 0x50;
                    break;
                case 6:
                    regValue[11] &= 0xEF;
                    regValue[11] |= 0x60;
                    break;
            }
            switch (comboBox_ch8_gain.SelectedIndex)
            {
                case 0: // 000
                    regValue[12] &= 0x8F;
                    break;
                case 1: // 001
                    regValue[12] &= 0x9F;
                    regValue[12] |= 0x10;
                    break;
                case 2: // 010
                    regValue[12] &= 0xAF;
                    regValue[12] |= 0x20;
                    break;
                case 3: // 011
                    regValue[12] &= 0xBF;
                    regValue[12] |= 0x30;
                    break;
                case 4: // 100
                    regValue[12] &= 0xCF;
                    regValue[12] |= 0x40;
                    break;
                case 5: // 101
                    regValue[12] &= 0xDF;
                    regValue[12] |= 0x50;
                    break;
                case 6:
                    regValue[12] &= 0xEF;
                    regValue[12] |= 0x60;
                    break;
            }
            // SRB2
            switch (comboBox_ch1_srb2.SelectedIndex)
            {
                case 0:
                    regValue[5] &= 0xF7;
                    break;
                case 1:
                    regValue[5] |= 0x08;
                    break;
            }
            switch (comboBox_ch2_srb2.SelectedIndex)
            {
                case 0:
                    regValue[6] &= 0xF7;
                    break;
                case 1:
                    regValue[6] |= 0x08;
                    break;
            }
            switch (comboBox_ch3_srb2.SelectedIndex)
            {
                case 0:
                    regValue[7] &= 0xF7;
                    break;
                case 1:
                    regValue[7] |= 0x08;
                    break;
            }
            switch (comboBox_ch4_srb2.SelectedIndex)
            {
                case 0:
                    regValue[8] &= 0xF7;
                    break;
                case 1:
                    regValue[8] |= 0x08;
                    break;
            }
            switch (comboBox_ch5_srb2.SelectedIndex)
            {
                case 0:
                    regValue[9] &= 0xF7;
                    break;
                case 1:
                    regValue[9] |= 0x08;
                    break;
            }
            switch (comboBox_ch6_srb2.SelectedIndex)
            {
                case 0:
                    regValue[10] &= 0xF7;
                    break;
                case 1:
                    regValue[10] |= 0x08;
                    break;
            }
            switch (comboBox_ch7_srb2.SelectedIndex)
            {
                case 0:
                    regValue[11] &= 0xF7;
                    break;
                case 1:
                    regValue[11] |= 0x08;
                    break;
            }
            switch (comboBox_ch8_srb2.SelectedIndex)
            {
                case 0:
                    regValue[12] &= 0xF7;
                    break;
                case 1:
                    regValue[12] |= 0x08;
                    break;
            }
            // Input
            switch (comboBox_ch1_input.SelectedIndex)
            {
                case 0: // 000
                    regValue[5] &= 0xF8;
                    break;
                case 1: // 001
                    regValue[5] |= 0x01;
                    regValue[5] &= 0xF9;
                    break;
                case 2: // 010
                    regValue[5] |= 0x02;
                    regValue[5] &= 0xFA;
                    break;
                case 3: // 011
                    regValue[5] |= 0x03;
                    regValue[5] &= 0xFB;
                    break;
                case 4: // 100
                    regValue[5] |= 0x04;
                    regValue[5] &= 0xFC;
                    break;
                case 5: // 101
                    regValue[5] |= 0x05;
                    regValue[5] &= 0xFD;
                    break;
                case 6: // 110
                    regValue[5] |= 0x06;
                    regValue[5] &= 0xFE;
                    break;
                case 7:  // 111
                    regValue[5] |= 0x07;
                    break;
            }
            switch (comboBox_ch2_input.SelectedIndex)
            {
                case 0: // 000
                    regValue[6] &= 0xF8;
                    break;
                case 1: // 001
                    regValue[6] |= 0x01;
                    regValue[6] &= 0xF9;
                    break;
                case 2: // 010
                    regValue[6] |= 0x02;
                    regValue[6] &= 0xFA;
                    break;
                case 3: // 011
                    regValue[6] |= 0x03;
                    regValue[6] &= 0xFB;
                    break;
                case 4: // 100
                    regValue[6] |= 0x04;
                    regValue[6] &= 0xFC;
                    break;
                case 5: // 101
                    regValue[6] |= 0x05;
                    regValue[6] &= 0xFD;
                    break;
                case 6: // 110
                    regValue[6] |= 0x06;
                    regValue[6] &= 0xFE;
                    break;
                case 7:  // 111
                    regValue[6] |= 0x07;
                    break;
            }
            switch (comboBox_ch3_input.SelectedIndex)
            {
                case 0: // 000
                    regValue[7] &= 0xF8;
                    break;
                case 1: // 001
                    regValue[7] |= 0x01;
                    regValue[7] &= 0xF9;
                    break;
                case 2: // 010
                    regValue[7] |= 0x02;
                    regValue[7] &= 0xFA;
                    break;
                case 3: // 011
                    regValue[7] |= 0x03;
                    regValue[7] &= 0xFB;
                    break;
                case 4: // 100
                    regValue[7] |= 0x04;
                    regValue[7] &= 0xFC;
                    break;
                case 5: // 101
                    regValue[7] |= 0x05;
                    regValue[7] &= 0xFD;
                    break;
                case 6: // 110
                    regValue[7] |= 0x06;
                    regValue[7] &= 0xFE;
                    break;
                case 7:  // 111
                    regValue[7] |= 0x07;
                    break;
            }
            switch (comboBox_ch4_input.SelectedIndex)
            {
                case 0: // 000
                    regValue[8] &= 0xF8;
                    break;
                case 1: // 001
                    regValue[8] |= 0x01;
                    regValue[8] &= 0xF9;
                    break;
                case 2: // 010
                    regValue[8] |= 0x02;
                    regValue[8] &= 0xFA;
                    break;
                case 3: // 011
                    regValue[8] |= 0x03;
                    regValue[8] &= 0xFB;
                    break;
                case 4: // 100
                    regValue[8] |= 0x04;
                    regValue[8] &= 0xFC;
                    break;
                case 5: // 101
                    regValue[8] |= 0x05;
                    regValue[8] &= 0xFD;
                    break;
                case 6: // 110
                    regValue[8] |= 0x06;
                    regValue[8] &= 0xFE;
                    break;
                case 7:  // 111
                    regValue[8] |= 0x07;
                    break;
            }
            switch (comboBox_ch5_input.SelectedIndex)
            {
                case 0: // 000
                    regValue[9] &= 0xF8;
                    break;
                case 1: // 001
                    regValue[9] |= 0x01;
                    regValue[9] &= 0xF9;
                    break;
                case 2: // 010
                    regValue[9] |= 0x02;
                    regValue[9] &= 0xFA;
                    break;
                case 3: // 011
                    regValue[9] |= 0x03;
                    regValue[9] &= 0xFB;
                    break;
                case 4: // 100
                    regValue[9] |= 0x04;
                    regValue[9] &= 0xFC;
                    break;
                case 5: // 101
                    regValue[9] |= 0x05;
                    regValue[9] &= 0xFD;
                    break;
                case 6: // 110
                    regValue[9] |= 0x06;
                    regValue[9] &= 0xFE;
                    break;
                case 7:  // 111
                    regValue[9] |= 0x07;
                    break;
            }
            switch (comboBox_ch6_input.SelectedIndex)
            {
                case 0: // 000
                    regValue[10] &= 0xF8;
                    break;
                case 1: // 001
                    regValue[10] |= 0x01;
                    regValue[10] &= 0xF9;
                    break;
                case 2: // 010
                    regValue[10] |= 0x02;
                    regValue[10] &= 0xFA;
                    break;
                case 3: // 011
                    regValue[10] |= 0x03;
                    regValue[10] &= 0xFB;
                    break;
                case 4: // 100
                    regValue[10] |= 0x04;
                    regValue[10] &= 0xFC;
                    break;
                case 5: // 101
                    regValue[10] |= 0x05;
                    regValue[10] &= 0xFD;
                    break;
                case 6: // 110
                    regValue[10] |= 0x06;
                    regValue[10] &= 0xFE;
                    break;
                case 7:  // 111
                    regValue[10] |= 0x07;
                    break;
            }
            switch (comboBox_ch7_input.SelectedIndex)
            {
                case 0: // 000
                    regValue[11] &= 0xF8;
                    break;
                case 1: // 001
                    regValue[11] |= 0x01;
                    regValue[11] &= 0xF9;
                    break;
                case 2: // 010
                    regValue[11] |= 0x02;
                    regValue[11] &= 0xFA;
                    break;
                case 3: // 011
                    regValue[11] |= 0x03;
                    regValue[11] &= 0xFB;
                    break;
                case 4: // 100
                    regValue[11] |= 0x04;
                    regValue[11] &= 0xFC;
                    break;
                case 5: // 101
                    regValue[11] |= 0x05;
                    regValue[11] &= 0xFD;
                    break;
                case 6: // 110
                    regValue[11] |= 0x06;
                    regValue[11] &= 0xFE;
                    break;
                case 7:  // 111
                    regValue[11] |= 0x07;
                    break;
            }
            switch (comboBox_ch8_input.SelectedIndex)
            {
                case 0: // 000
                    regValue[12] &= 0xF8;
                    break;
                case 1: // 001
                    regValue[12] |= 0x01;
                    regValue[12] &= 0xF9;
                    break;
                case 2: // 010
                    regValue[12] |= 0x02;
                    regValue[12] &= 0xFA;
                    break;
                case 3: // 011
                    regValue[12] |= 0x03;
                    regValue[12] &= 0xFB;
                    break;
                case 4: // 100
                    regValue[12] |= 0x04;
                    regValue[12] &= 0xFC;
                    break;
                case 5: // 101
                    regValue[12] |= 0x05;
                    regValue[12] &= 0xFD;
                    break;
                case 6: // 110
                    regValue[12] |= 0x06;
                    regValue[12] &= 0xFE;
                    break;
                case 7:  // 111
                    regValue[12] |= 0x07;
                    break;
            }
            // 输出速率
            switch (comboBox_dataRate.SelectedIndex)
            {
                case 0: // 000
                    regValue[1] &= 0xF8;
                    break;
                case 1: // 001
                    regValue[1] |= 0x01;
                    regValue[1] &= 0xF9;
                    break;
                case 2: // 010
                    regValue[1] |= 0x02;
                    regValue[1] &= 0xFA;
                    break;
                case 3:
                    regValue[1] |= 0x03;
                    regValue[1] &= 0xFB;
                    break;
                case 4:
                    regValue[1] |= 0x04;
                    regValue[1] &= 0xFC;
                    break;
                case 5:
                    regValue[1] |= 0x05;
                    regValue[1] &= 0xFD;
                    break;
                case 6:
                    regValue[1] |= 0x06;
                    regValue[1] &= 0xFE;
                    break;
            }
            // SRB1
            switch (comboBox_srb1.SelectedIndex)
            {
                case 0:
                    regValue[21] &= 0xDF;
                    break;
                case 1:
                    regValue[21] |= 0x20;
                    break;
            }
            // Conversion Mode
            switch (comboBox_conversionMode.SelectedIndex)
            {
                case 0:
                    regValue[23] &= 0xF7;
                    break;
                case 1:
                    regValue[23] |= 0x80;
                    break;
            }
            // Reference Voltage
            switch (comboBox_reference.SelectedIndex)
            {
                case 0:
                    regValue[3] &= 0x7F;
                    break;
                case 1:
                    regValue[3] |= 0x80;
                    break;
            }
            #endregion 读取当前用户设置
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(charToSend, 0, 1);
                Thread.Sleep(20);
                serialPort1.Write(regValue, 0, 24);
            }
            else
            {
                MessageBox.Show("请先连接串口!");
            }
        }
        /// <summary>
        /// “读取配置”comboBox关闭，根据选择的配置设置显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_selectCfg_DropDownClosed(object sender, EventArgs e)
        {
            switch (comboBox_selectCfg.SelectedIndex)
            {
                case 0:
                    #region 读取配置0
                    textBox_adsID.Clear();
                    comboBox_testSignal_source.SelectedIndex = 0;
                    comboBox_testSignal_amplitude.SelectedIndex = 0;
                    comboBox_testSingal_frequency.SelectedIndex = 2;
                    checkBox_ch1_isOpen.IsChecked = true;
                    checkBox_ch2_isOpen.IsChecked = true;
                    checkBox_ch3_isOpen.IsChecked = true;
                    checkBox_ch4_isOpen.IsChecked = true;
                    checkBox_ch5_isOpen.IsChecked = true;
                    checkBox_ch6_isOpen.IsChecked = true;
                    checkBox_ch7_isOpen.IsChecked = true;
                    checkBox_ch8_isOpen.IsChecked = true;
                    comboBox_ch1_gain.SelectedIndex = 6;
                    comboBox_ch2_gain.SelectedIndex = 6;
                    comboBox_ch3_gain.SelectedIndex = 6;
                    comboBox_ch4_gain.SelectedIndex = 6;
                    comboBox_ch5_gain.SelectedIndex = 6;
                    comboBox_ch6_gain.SelectedIndex = 6;
                    comboBox_ch7_gain.SelectedIndex = 6;
                    comboBox_ch8_gain.SelectedIndex = 6;
                    comboBox_ch1_srb2.SelectedIndex = 0;
                    comboBox_ch2_srb2.SelectedIndex = 0;
                    comboBox_ch3_srb2.SelectedIndex = 0;
                    comboBox_ch4_srb2.SelectedIndex = 0;
                    comboBox_ch5_srb2.SelectedIndex = 0;
                    comboBox_ch6_srb2.SelectedIndex = 0;
                    comboBox_ch7_srb2.SelectedIndex = 0;
                    comboBox_ch8_srb2.SelectedIndex = 0;
                    comboBox_ch1_input.SelectedIndex = 1;
                    comboBox_ch2_input.SelectedIndex = 1;
                    comboBox_ch3_input.SelectedIndex = 1;
                    comboBox_ch4_input.SelectedIndex = 1;
                    comboBox_ch5_input.SelectedIndex = 1;
                    comboBox_ch6_input.SelectedIndex = 1;
                    comboBox_ch7_input.SelectedIndex = 1;
                    comboBox_ch8_input.SelectedIndex = 1;
                    comboBox_dataRate.SelectedIndex = 6;
                    comboBox_srb1.SelectedIndex = 0;
                    comboBox_conversionMode.SelectedIndex = 0;
                    comboBox_reference.SelectedIndex = 0;
                    #endregion 读取配置0
                    break;
                case 1:
                    #region 读取配置1
                    textBox_adsID.Clear();
                    comboBox_testSignal_source.SelectedIndex = 0;
                    comboBox_testSignal_amplitude.SelectedIndex = 0;
                    comboBox_testSingal_frequency.SelectedIndex = 2;
                    checkBox_ch1_isOpen.IsChecked = true;
                    checkBox_ch2_isOpen.IsChecked = true;
                    checkBox_ch3_isOpen.IsChecked = true;
                    checkBox_ch4_isOpen.IsChecked = true;
                    checkBox_ch5_isOpen.IsChecked = true;
                    checkBox_ch6_isOpen.IsChecked = true;
                    checkBox_ch7_isOpen.IsChecked = true;
                    checkBox_ch8_isOpen.IsChecked = true;
                    comboBox_ch1_gain.SelectedIndex = 6;
                    comboBox_ch2_gain.SelectedIndex = 6;
                    comboBox_ch3_gain.SelectedIndex = 6;
                    comboBox_ch4_gain.SelectedIndex = 6;
                    comboBox_ch5_gain.SelectedIndex = 6;
                    comboBox_ch6_gain.SelectedIndex = 6;
                    comboBox_ch7_gain.SelectedIndex = 6;
                    comboBox_ch8_gain.SelectedIndex = 6;
                    comboBox_ch1_srb2.SelectedIndex = 0;
                    comboBox_ch2_srb2.SelectedIndex = 0;
                    comboBox_ch3_srb2.SelectedIndex = 0;
                    comboBox_ch4_srb2.SelectedIndex = 0;
                    comboBox_ch5_srb2.SelectedIndex = 0;
                    comboBox_ch6_srb2.SelectedIndex = 0;
                    comboBox_ch7_srb2.SelectedIndex = 0;
                    comboBox_ch8_srb2.SelectedIndex = 0;
                    comboBox_ch1_input.SelectedIndex = 0;
                    comboBox_ch2_input.SelectedIndex = 0;
                    comboBox_ch3_input.SelectedIndex = 0;
                    comboBox_ch4_input.SelectedIndex = 0;
                    comboBox_ch5_input.SelectedIndex = 0;
                    comboBox_ch6_input.SelectedIndex = 0;
                    comboBox_ch7_input.SelectedIndex = 0;
                    comboBox_ch8_input.SelectedIndex = 0;
                    comboBox_dataRate.SelectedIndex = 6;
                    comboBox_srb1.SelectedIndex = 1;
                    comboBox_conversionMode.SelectedIndex = 0;
                    comboBox_reference.SelectedIndex = 1;
                    #endregion 读取配置1
                    break;
                case 2:
                    #region 读取配置2
                    textBox_adsID.Clear();
                    comboBox_testSignal_source.SelectedIndex = 1;
                    comboBox_testSignal_amplitude.SelectedIndex = 0;
                    comboBox_testSingal_frequency.SelectedIndex = 0;
                    checkBox_ch1_isOpen.IsChecked = true;
                    checkBox_ch2_isOpen.IsChecked = true;
                    checkBox_ch3_isOpen.IsChecked = true;
                    checkBox_ch4_isOpen.IsChecked = true;
                    checkBox_ch5_isOpen.IsChecked = true;
                    checkBox_ch6_isOpen.IsChecked = true;
                    checkBox_ch7_isOpen.IsChecked = true;
                    checkBox_ch8_isOpen.IsChecked = true;
                    comboBox_ch1_gain.SelectedIndex = 0;
                    comboBox_ch2_gain.SelectedIndex = 0;
                    comboBox_ch3_gain.SelectedIndex = 0;
                    comboBox_ch4_gain.SelectedIndex = 0;
                    comboBox_ch5_gain.SelectedIndex = 0;
                    comboBox_ch6_gain.SelectedIndex = 0;
                    comboBox_ch7_gain.SelectedIndex = 0;
                    comboBox_ch8_gain.SelectedIndex = 0;
                    comboBox_ch1_srb2.SelectedIndex = 0;
                    comboBox_ch2_srb2.SelectedIndex = 0;
                    comboBox_ch3_srb2.SelectedIndex = 0;
                    comboBox_ch4_srb2.SelectedIndex = 0;
                    comboBox_ch5_srb2.SelectedIndex = 0;
                    comboBox_ch6_srb2.SelectedIndex = 0;
                    comboBox_ch7_srb2.SelectedIndex = 0;
                    comboBox_ch8_srb2.SelectedIndex = 0;
                    comboBox_ch1_input.SelectedIndex = 5;
                    comboBox_ch2_input.SelectedIndex = 5;
                    comboBox_ch3_input.SelectedIndex = 5;
                    comboBox_ch4_input.SelectedIndex = 5;
                    comboBox_ch5_input.SelectedIndex = 5;
                    comboBox_ch6_input.SelectedIndex = 5;
                    comboBox_ch7_input.SelectedIndex = 5;
                    comboBox_ch8_input.SelectedIndex = 5;
                    comboBox_dataRate.SelectedIndex = 6;
                    comboBox_srb1.SelectedIndex = 0;
                    comboBox_conversionMode.SelectedIndex = 0;
                    comboBox_reference.SelectedIndex = 0;
                    #endregion 读取配置2
                    break;
            }
        }
        /// <summary>
        /// “查看”按键按下，打开EEG窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_checkAds_clicked(object sender, RoutedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                Window eegWindow = new EEG();
                eegWindow.Show();
                mainWindow.Hide();
            }
            else
            {
                MessageBox.Show("请先连接串口!");
            }
        }
        /// <summary>
        /// “启用模块”按键按下，开启/关闭蓝牙模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButton_enableBle_clicked(object sender, RoutedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (Convert.ToBoolean(toggleButton_enableBle.IsChecked))
                {
                    serialPort1.Write("E");
                }
                else
                {
                    serialPort1.Write("F");
                    toggleButton_enableBle.IsChecked = false;
                }
            }
            else
            {
                MessageBox.Show("请先打开串口！");
                toggleButton_enableBle.IsChecked = false;
            }
        }
        /// <summary>
        /// “获取”按键按下，获取当前蓝牙名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_getBleName_clicked(object sender, RoutedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (Convert.ToBoolean(toggleButton_enableBle.IsChecked))
                {
                    string bleName = "unknown";
                    serialPort1.Write("G");
                    Thread.Sleep(20);
                    try
                    {
                        bleName = serialPort1.ReadExisting();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message, "获取失败!");
                    }
                    textBox_bleName.Text = bleName;
                }
                else
                {
                    MessageBox.Show("请先启用蓝牙模块！");
                }
            }
            else
            {
                MessageBox.Show("请先打开串口！");
            }
        }
        /// <summary>
        /// “修改”按键按下，修改当前蓝牙名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_setBleName_clicked(object sender, RoutedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (Convert.ToBoolean(toggleButton_enableBle.IsChecked))
                {
                    if (textBox_bleName.Text.Length != 0)
                    {
                        serialPort1.Write("H" + textBox_bleName.Text);
                    }
                    else
                    {
                        MessageBox.Show("新蓝牙名称不能为空！");
                    }
                }
                else
                {
                    MessageBox.Show("请先启用蓝牙模块！");
                }
            }
            else
            {
                MessageBox.Show("请先打开串口！");
            }
        }
        /// <summary>
        /// “修改”按键按下，根据当前comboBOx上的值修改蓝牙模块的发射功率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_setBleTpl_clicked(object sender, RoutedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (Convert.ToBoolean(toggleButton_enableBle.IsChecked))
                {
                    string bleTpl = "(0)";
                    switch(comboBox_BleTpl.SelectedIndex)
                    {
                        case 0:
                            bleTpl = "(2)";
                            break;
                        case 1:
                            bleTpl = "(1)";
                            break;
                        case 2:
                            bleTpl = "(0)";
                            break;
                        case 3:
                            bleTpl = "(-3)";
                            break;
                        case 4:
                            bleTpl = "(-6)";
                            break;
                        case 5:
                            bleTpl = "(-9)";
                            break;
                        case 6:
                            bleTpl = "(-12)";
                            break;
                        case 7:
                            bleTpl = "(-15)";
                            break;
                        case 8:
                            bleTpl = "(-18)";
                            break;
                        case 9:
                            bleTpl = "(-21)";
                            break;
                    }
                    serialPort1.Write("J" + bleTpl);
                }
                else
                {
                    MessageBox.Show("请先启用蓝牙模块！");
                }
            }
            else
            {
                MessageBox.Show("请先打开串口！");
            }
        }
    }
}
