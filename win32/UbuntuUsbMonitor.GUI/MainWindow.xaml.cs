using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UbuntuUsbMonitor.Network;

namespace UbuntuUsbMonitor
{

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetworkKernel networkKernel;

        public MainWindow()
        {
            InitializeComponent();

            networkKernel = new NetworkKernel(@"tcp://10.0.0.178:5555");
            networkKernel.OnNewPacketReceived += (sender, evArgs) => {
                System.Diagnostics.Trace.WriteLine("UrbID: " + evArgs.Packet.URB_ID.ToString());
            };
        }
    }
}
