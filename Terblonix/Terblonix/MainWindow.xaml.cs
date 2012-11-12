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

namespace Terblonix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SerialPort term = new SerialPort();

        public MainWindow()
        {
            InitializeComponent();
            combo1_ItemHandler(null, null);
            term.DataReceived += new SerialDataReceivedEventHandler(DataRX);
        }

        private void combo1_ItemHandler(object sender, RoutedEventArgs e)
        {
            combo1.Items.Clear();
            foreach (string com in SerialPort.GetPortNames())
            {
                combo1.Items.Add(com);
            }
            combo1.SelectedIndex = 0;
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            if (connect.Content.ToString() == "Connect")
            {
                connect.Content = "Disconnect";
                term.PortName = combo1.SelectedValue.ToString();
                term.Open();
            }
            else
            {
                connect.Content = "Connect";
                term.Close();
            }
        }

        private void DragDock(object sender, MouseButtonEventArgs e)
        {
            window.DragMove();
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            term.Write(inputBox.Text.Replace("\\n", "\n"));
            inputBox.Clear();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            outputBox.Clear();
        }

        private void DataRX(object sender, SerialDataReceivedEventArgs e)
        {
            string tempText = term.ReadExisting();
            Action append = delegate() { outputBox.AppendText(tempText); };
            outputBox.Dispatcher.Invoke(append);
            outputBox.Dispatcher.Invoke(delegate() { outputBox.ScrollToEnd(); });
        }

    }
}
