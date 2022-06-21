using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using CefSharp;
using CefSharp.Wpf;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace ChromiumWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {

            InitializeComponent();

        }

        private void SetValues(object sender, RoutedEventArgs e) {
            JsCall($"(addPoint({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}))();");
        }

        private void ClearPoints(object sender, RoutedEventArgs e) {
            JsCall("(clearPoints())();");
        }

        private void JsCall(string command) {

            var frame = defaultBrowser.GetMainFrame();
            frame.EvaluateScriptAsync(command, null);

        }

    }
}
