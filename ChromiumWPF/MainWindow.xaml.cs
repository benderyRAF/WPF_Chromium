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

        enum Action { None, SetLocation, AddPoint, Addpolyline, AddPolygon, AddImage, CreateModel }

        private Action action = Action.None;

        List<TextBox> points = new List<TextBox>();

        public MainWindow() {

            InitializeComponent();

        }

        private void ClearPoints(object sender, RoutedEventArgs e) {
            JsCall("(clearPoints())();");
        }

        private void JsCall(string command) {

            var frame = defaultBrowser.GetMainFrame();
            frame.EvaluateScriptAsync(command, null);

        }

        private void ChooseAction(object sender, RoutedEventArgs e) {

            string headerAction = ((MenuItem)sender).Header.ToString();

            actionLabel.Content = headerAction;

            actionStack.Visibility = Visibility.Visible;
            pointStack.Visibility = Visibility.Collapsed;
            lineStack.Visibility = Visibility.Collapsed;
            polygonStack.Visibility = Visibility.Collapsed;
            urlTextblock.Visibility = Visibility.Collapsed;
            urlTextbox.Visibility = Visibility.Collapsed;

            switch (headerAction) {

                case "Set location":
                    action = Action.SetLocation;
                    pointStack.Visibility = Visibility.Visible;
                    break;

                case "Add point":
                    action = Action.AddPoint;
                    pointStack.Visibility = Visibility.Visible;
                    break;

                case "Add polyline":
                    action = Action.Addpolyline;
                    lineStack.Visibility = Visibility.Visible;
                    break;

                case "Add polygon":
                    action = Action.AddPolygon;
                    polygonStack.Visibility = Visibility.Visible;
                    break;

                case "Add image":
                    action = Action.AddImage;
                    pointStack.Visibility = Visibility.Visible;
                    urlTextblock.Visibility = Visibility.Visible;
                    urlTextbox.Visibility = Visibility.Visible; 
                    break;

                case "Create model":
                    action = Action.CreateModel;
                    pointStack.Visibility = Visibility.Visible;
                    urlTextblock.Visibility = Visibility.Visible;
                    urlTextbox.Visibility = Visibility.Visible;
                    break;

            }

        }

        private void ModeSwitch(object sender, RoutedEventArgs e) {
            JsCall($"(modeSwitch())();");
        }

        private void Approve(object sender, RoutedEventArgs e) {

            switch (action) {

                case Action.SetLocation:
                    JsCall($"(moveTo({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}))();");
                    break;

                case Action.AddPoint:
                    JsCall($"(addPoint({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}))();");
                    break;

                case Action.Addpolyline:
                    JsCall($"(addLine({srcPointXTextbox.Text}, {srcPointYTextbox.Text}, {dstPointXTextbox.Text}, {dstPointYTextbox.Text}, {widthTextbox.Text}))();");
                    break;

                case Action.AddPolygon:
                    string pointArray = "[";
                    points.ForEach(coordinate => {
                        pointArray += coordinate.Text;
                        pointArray += coordinate == points.Last() ? "]" : ",";
                    });
                    JsCall($"(addPolygon({pointArray}, {polygonHeightTextbox.Text}))();");
                    break;

                case Action.AddImage:
                    JsCall($"(addImage({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}, {urlTextbox.Text}))();");
                    break;

                case Action.CreateModel:
                    JsCall($"(createModel({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}, {urlTextbox.Text}))();");
                    break;

            }

        }

        private void AddPointInput(object sender, RoutedEventArgs e) {

            TextBlock newTextBlock = new TextBlock();
            newTextBlock.Text = $"Point {points.Count / 2 + 1}";
            newTextBlock.FontSize = 15;
            newTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TextBox newTextBoxX = new TextBox();
            TextBox newTextBoxY = new TextBox();

            polygonStack.Children.Add(newTextBlock);
            polygonStack.Children.Add(newTextBoxX);
            polygonStack.Children.Add(newTextBoxY);

            points.Add(newTextBoxX);
            points.Add(newTextBoxY);

        }
    }
}
