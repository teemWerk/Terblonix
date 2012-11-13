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
        public SerialPort term = new SerialPort();                            // Toplevel Serial Port object. The IO stream of COM port

        public MainWindow()
        {
            InitializeComponent();
            combo1_ItemHandler(null, null);                                   // Populate the combo box when the program initializes
            term.DataReceived += new SerialDataReceivedEventHandler(DataRX);  // The terminal outputs to the text buffer when it receives data
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
                term.Open();
            }
            else
            {
                combo1.IsEnabled = true;
                connect.Content = "Connect";
                term.Close();
            }
        }

        private void DragDock(object sender, MouseButtonEventArgs e)          // Holding the left mouse button down on the top text bar
        {                                                                     //   allows the window to be dragged
            window.DragMove();
            if (window.ResizeMode == ResizeMode.NoResize)          // Dragging the maximzed window restores to default size right now
            {
                window.ResizeMode = ResizeMode.CanResizeWithGrip;
                window.Width = 800;
                window.Height = 600;
            }
        }

        private void send_Click(object sender, RoutedEventArgs e)             // Writes the text from the send line to the serial port
        {
            term.Write(inputBox.Text.Replace("\\n", "\n"));
            inputBox.Clear();
        }

        private void clear_Click(object sender, RoutedEventArgs e)            // Empties the output buffer, pretty simple
        {
            outputBox.Clear();
        }

        private void DataRX(object sender, SerialDataReceivedEventArgs e)     // Takes all the data from the serial line and displays it
        {                                                                     //   on the output text box
            string tempText = term.ReadExisting();
            Action append = delegate() { outputBox.AppendText(tempText); };
            outputBox.Dispatcher.Invoke(append);
            outputBox.Dispatcher.Invoke(delegate() { outputBox.ScrollToEnd(); });
        }

        private void close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)  // Close the Window
        {
            window.Close();
        }

        private void max_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)   // Maximize the window, should also restore
        {
            window.Width = SystemParameters.WorkArea.Width;
            window.Height = SystemParameters.WorkArea.Height;
            window.Left = 0;
            window.Top = 0;
            window.ResizeMode = ResizeMode.NoResize;
        }

        private void min_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)  // Minimize the Window
        {
            window.WindowState = WindowState.Minimized;
        }
    }
}
