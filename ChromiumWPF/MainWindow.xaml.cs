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
using CefSharp;
using CefSharp.Wpf;

namespace ChromiumWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private int tabCount = 0;
        private TabItem currentTabItem = null;
        private ChromiumWebBrowser currentBrowserShowing = null;

        public MainWindow() {
            InitializeComponent();

        }

        private void newTabMenuItem_Click(object sender, RoutedEventArgs e) {

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

        private void backButton_Click(object sender, RoutedEventArgs e) {
            if (!currentBrowserShowing.CanGoBack) return;
            currentBrowserShowing.Back();
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e) {
            if (!currentBrowserShowing.CanGoForward) return;
            currentBrowserShowing.Forward();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e) {
            currentBrowserShowing.Reload();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (tabControl.SelectedItem != null) {
                currentTabItem = tabControl.SelectedItem as TabItem;
            }
            if (currentTabItem != null) {
                currentBrowserShowing = currentTabItem.Content as ChromiumWebBrowser;
            }
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e) {

            if (e.Key == Key.Enter) {
                search();
            }

        }

        private void search() {
            if (currentBrowserShowing == null || AddressBar.Text == "") { return; }
            currentBrowserShowing.Address = $"https://www.google.com/search?q={AddressBar.Text}";
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e) {
            SettingsWindow setting = new SettingsWindow();
            setting.ShowDialog();
        }
    }
}
