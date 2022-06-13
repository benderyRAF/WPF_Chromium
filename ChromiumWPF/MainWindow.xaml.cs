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

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        private readonly Brush Lime = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        private readonly Brush Red = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private readonly Brush Gray = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        private readonly Brush LightBlue = new SolidColorBrush(Color.FromRgb(173, 216, 230));
        private readonly Brush Lightgreen = new SolidColorBrush(Color.FromRgb(144, 238, 144));
        private readonly Brush IndianRed = new SolidColorBrush(Color.FromRgb(205, 92, 92));

        private int tabCount = 0;
        private TabItem currentTabItem = null;
        private ChromiumWebBrowser currentBrowserShowing = null;

        public string Header {
            get => Connected ? "on page" : "off page";
        }
        public string Address {
            get => Connected ? "file:///C:/Users/SHONDIM/Desktop/LocalhostOn.html" : "file:///C:/Users/SHONDIM/Desktop/LocalhostOff.html";
        }
        public string ConnectionState {
            get => Connected ? "On line" : "Off line";
        }
        public Brush ConnectionStateColor {
            get => Connected ? Lightgreen : IndianRed;
        }

        private bool connected = true;
        private bool Connected {
            get => connected;
            set {

                connected = value;
                currentTabItem.Header = Header;
                currentBrowserShowing.Address = Address;
                OnOffLineText.Text = ConnectionState;
                OnOffLineText.Foreground = ConnectionStateColor;

            }
        }

        private bool autoCheck = true;
        private bool AutoCheck {
            get => autoCheck;
            set {

                autoCheck = value;

                AutoCheckButton.Background = autoCheck ? Lime : Red;
                OnOffLineButton.Background = autoCheck ? Gray : LightBlue;

                if (autoCheck) checkConnectionTimer.Start();
                else checkConnectionTimer.Stop();

            }
        }

        DispatcherTimer checkConnectionTimer;

        public MainWindow() {

            InitializeComponent();
            connected = InternetGetConnectedState(out _, 0);

            TabItem tabItem = new TabItem {
                Name = "defaultPage",
                Header = Header
            };
            tabControl.Items.Add(tabItem);

            ChromiumWebBrowser browser = new ChromiumWebBrowser(Address) {
                Name = "defaultBrowser",
                BrowserSettings = {
                    DefaultEncoding = "UTF-8",
                    WebGl = CefState.Disabled
                }
            };

            tabItem.Content = browser;

            currentTabItem = tabItem;
            currentBrowserShowing = browser;

            // Checks internet connection every second.
            checkConnectionTimer = new DispatcherTimer();
            checkConnectionTimer.Tick += new EventHandler(CheckConnection);
            checkConnectionTimer.Interval = new TimeSpan(0, 0, 1);
            checkConnectionTimer.Start();

        }

        private void CheckConnection(object sender, EventArgs e) {

            bool check = InternetGetConnectedState(out _, 0);
            if (check == connected) return;

            // Refreshing & changing connection state.
            Connected = check;

        }

        private void NewTabMenuItem_Click(object sender, RoutedEventArgs e) {

            TabItem tabItem = new TabItem();
            tabControl.Items.Add(tabItem);

            tabItem.Name = "tab_" + tabCount;
            tabItem.Header = $"New blank page ({tabCount})";

            ChromiumWebBrowser browser = new ChromiumWebBrowser("https://www.google.co.il") {
                Name = "Browser_" + tabCount++,
                BrowserSettings = {
                    DefaultEncoding = "UTF-8",
                    WebGl = CefState.Disabled
                }
            };
            
            tabItem.Content = browser;

            tabControl.SelectedItem = tabItem;

            currentTabItem = tabItem;
            currentBrowserShowing = browser;
            browser.Loaded += FinishLoadingWebPage;
            
        }

        private void FinishLoadingWebPage(object sender, RoutedEventArgs e) {

            var sndr = sender as ChromiumWebBrowser;
            currentTabItem.Header = sndr.Address;

        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            if (!currentBrowserShowing.CanGoBack) return;
            currentBrowserShowing.Back();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e) {
            if (!currentBrowserShowing.CanGoForward) return;
            currentBrowserShowing.Forward();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            currentBrowserShowing.Reload();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (tabControl.SelectedItem != null) {
                currentTabItem = tabControl.SelectedItem as TabItem;
            }
            if (currentTabItem != null) {
                currentBrowserShowing = currentTabItem.Content as ChromiumWebBrowser;
            }
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e) {
            SettingsWindow setting = new SettingsWindow();
            setting.ShowDialog();
        }

        private void AutoCheckButtonClick(object sender, RoutedEventArgs e) {
            AutoCheck = !AutoCheck;
        }

        private void ChangeConnection(object sender, RoutedEventArgs e) {

            if (OnOffLineButton.Background != LightBlue) return;
            Connected = !Connected;

        }
    }
}
