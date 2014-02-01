using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

using System.IO.Ports;
using System.Windows.Interop;

namespace Terblonix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort term = new SerialPort();
        private StringBuilder sb = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void init(object sender, EventArgs e)
        {
            combo1_ItemHandler(null, null);                                   // Populate the combo box when the program initializes
            term.DataReceived += new SerialDataReceivedEventHandler(DataRX);  // The terminal outputs to the text buffer when it receives data

            IntPtr windowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource src = HwndSource.FromHwnd(windowHandle);
            src.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle WM_DEVICECHANGE... 
            if (msg == 0x219)
            {
                combo1.Items.Clear();
                foreach (string com in SerialPort.GetPortNames())
                {
                    combo1.Items.Add(com);
                }
                combo1.SelectedIndex = 0;
            }

            return IntPtr.Zero;
        }

        private void combo1_ItemHandler(object sender, RoutedEventArgs e)     // Populate the combo box with available serial ports
        {
            combo1.Items.Clear();
            foreach (string com in SerialPort.GetPortNames())
            {
                combo1.Items.Add(com);
            }
            combo1.SelectedIndex = 0;
        }

        private void connect_Click(object sender, RoutedEventArgs e)          // Establish a connection to the serial port selected in combo box
        {                                                                     //   If you are currently connected, pushing the button
            if (connect.Content.ToString() == "Connect")                      //   Disconnects from the currently connected channel
            {                                                                 // Combo Box is disabled while connection is active
                combo1.IsEnabled = false;
                connect.Content = "Disconnect";
                term.PortName = combo1.SelectedValue.ToString();
                term.BaudRate = (int) combo2.SelectionBoxItem;
                term.Open();
            }
            else
            {
                combo1.IsEnabled = true;
                connect.Content = "Connect";
                term.Close();
            }
        }

        private void send_Click(object sender, RoutedEventArgs e)             // Writes the text from the send line to the serial port
        {
            term.Write(inputBox.Text.Replace("\\n", "\n"));
            inputBox.Clear();
        }

        private void clear_Click(object sender, RoutedEventArgs e)            // Empties the output buffer, pretty simple
        {
            //outputBox.Clear();
            outputBox.Text = "";
        }

        private void DataRX(object sender, SerialDataReceivedEventArgs e)
        {
            outputBox.Dispatcher.Invoke(delegate() { outputBox.AppendText(term.ReadExisting());});
            outputBox.Dispatcher.Invoke(delegate() { outputBox.ScrollToEnd(); });
        }
    }
}
