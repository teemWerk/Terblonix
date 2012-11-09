import clr
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

from System.IO import File
from System.IO.Ports import SerialPort
from System.Windows.Markup import XamlReader

from System.Windows import (
    Application, Window
)
from System.Windows.Controls import (
    Label, ComboBox, ComboBoxItem
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

        self.dockbar = self.Root.FindName('dockbar')
        self.dockbar.MouseLeftButtonDown += self.dragDock

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
        else:
            self.connect.Content = 'Connect'

    def dragDock(self, sender, event):
        self.window.DragMove()

stage = MainWin()

app = Application()
app.Run(stage.Root)