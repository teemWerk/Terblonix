import clr
clr.AddReference("IronPython")
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")
clr.AddReference("WindowsBase")

from IronPython.Compiler import CallTarget0
from System.IO import File
from System.IO.Ports import SerialPort
from System.Windows.Markup import XamlReader
from System.Windows.Threading import Dispatcher, DispatcherPriority

from System.Windows import (
    Application, Window
)
from System.Windows.Controls import (
    Label, ComboBox, ComboBoxItem, TextBox
)

class MainWin(object):
    def __init__(self):
        stream = File.OpenRead("terblonix.xaml")
        self.Root = XamlReader.Load(stream)

        self.window = self.Root.FindName('window')

        self.combo = self.Root.FindName('combo1')
        self.ports = SerialPort.GetPortNames()
        for com in self.ports:
            item = ComboBoxItem()
            item.Content = com
            item.FontSize = 16
            self.combo.Items.Add(item)
        if(self.ports):
            self.combo.SelectedIndex = 0

        self.connect = self.Root.FindName('connect')
        self.connect.Click += self.connectClick

        self.rescan = self.Root.FindName('rescan')
        self.rescan.Click += self.scanClick

        self.portStack = self.Root.FindName('portStack')

        self.dockbar = self.Root.FindName('dockbar')
        self.dockbar.MouseLeftButtonDown += self.dragDock

        self.inputBuff = self.Root.FindName('inputBox')

        self.outputBuff = self.Root.FindName('outputBox')

        self.send = self.Root.FindName('send')
        self.send.Click += self.sendClick

        self.clear = self.Root.FindName('clear')
        self.clear.Click += self.clearClick

    def scanClick(self, sender, event):
        self.combo.Items.Clear()
        self.ports = SerialPort.GetPortNames()
        for com in self.ports:
            item = ComboBoxItem()
            item.Content = com
            item.FontSize = 16
            self.combo.Items.Add(item)
        if(self.ports):
            self.combo.SelectedIndex = 0
    
    def connectClick(self, sender, event):
        if self.connect.Content == 'Connect':
            self.connect.Content = 'Disconnect'
            self.term = SerialPort(self.combo.SelectedValue.Content, 9600)  # Set to 9600 for calibration purposes because that is what my board uses
            self.term.Open()
            self.term.DataReceived += self.dataRX
        else:
            self.connect.Content = 'Connect'
            self.term.Close()

    def dragDock(self, sender, event):
        self.window.DragMove()
    
    def sendClick(self, sender, event):
        self.term.Write(self.inputBuff.Text)
        self.inputBuff.Text = ''

    def clearClick(self, sender, event):
        self.outputBuff.Clear()

    def dataRX(self, sender, event):
        tempText = self.term.ReadExisting().replace('\\n','\r\n')
        textDelegate = self.outDel(tempText)
        self.outputBuff.Dispatcher.Invoke(DispatcherPriority.Normal, textDelegate)
        self.outputBuff.Dispatcher.Invoke(DispatcherPriority.Normal, self.scrollToBottom())

    def outDel(self, text):
        return CallTarget0(lambda:TextBox.AppendText(self.outputBuff, text))

    def scrollToBottom(self):
        return CallTarget0(lambda:TextBox.ScrollToEnd(self.outputBuff))


stage = MainWin()

app = Application()
app.Run(stage.Root)