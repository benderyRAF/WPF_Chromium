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
        private readonly List<TextBox> points = new List<TextBox>();

        public MainWindow() {
            InitializeComponent();
        }

        private void JsCall(string command) {

            // Calls function in cesium js.
            var frame = defaultBrowser.GetMainFrame();
            frame.EvaluateScriptAsync(command, null);

        }

        private void ChooseAction(object sender, RoutedEventArgs e) {

            string headerAction = ((MenuItem)sender).Header.ToString();
            actionLabel.Content = headerAction;

            // Default values.
            actionStack.Visibility = Visibility.Visible;
            pointStack.Visibility = Visibility.Collapsed;
            lineStack.Visibility = Visibility.Collapsed;
            polygonStack.Visibility = Visibility.Collapsed;
            urlTextblock.Visibility = Visibility.Collapsed;
            urlTextbox.Visibility = Visibility.Collapsed;
            heightTextbox.Tag = "LastInStack";
            urlTextbox.Tag = "";

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
                    heightTextbox.Tag = "";
                    urlTextbox.Tag = "LastInStack";
                    break;

                case "Create model":
                    action = Action.CreateModel;
                    pointStack.Visibility = Visibility.Visible;
                    urlTextblock.Visibility = Visibility.Visible;
                    urlTextbox.Visibility = Visibility.Visible;
                    heightTextbox.Tag = "";
                    urlTextbox.Tag = "LastInStack";
                    break;

            }

        }

        private void ModeSwitch(object sender, RoutedEventArgs e) {
            JsCall($"(modeSwitch())();"); // Switch camera view 2D/3D.
        }

        private void Approve(object sender, RoutedEventArgs e) {

            switch (action) {

                case Action.SetLocation: // Moves camera to position.
                    JsCall($"(moveTo({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}))();");
                    break;

                case Action.AddPoint:
                    JsCall($"(addPoint({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}))();");
                    break;

                case Action.Addpolyline:
                    JsCall($"(addLine({srcPointXTextbox.Text}, {srcPointYTextbox.Text}, {dstPointXTextbox.Text}, {dstPointYTextbox.Text}, {widthTextbox.Text}))();");
                    break;

                case Action.AddPolygon:

                    // Builds array of coordinates: [x, y, x, y, x, y, ...].
                    string pointArray = "[";
                    points.ForEach(coordinate => {
                        pointArray += coordinate.Text;
                        pointArray += coordinate == points.Last() ? "]" : ",";
                    });
                    JsCall($"(addPolygon({pointArray}, {polygonHeightTextbox.Text}))();");

                    // Clear polygon's point inputs.
                    for (int i = polygonStack.Children.Count - 1; i > 2; i--) {
                        polygonStack.Children.RemoveAt(i);
                    }
                    points.Clear();

                    break;

                case Action.AddImage:
                    JsCall($"(addImage({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}, '{urlTextbox.Text}'))();");
                    break;

                case Action.CreateModel:
                    JsCall($"(createModel('{urlTextbox.Text}', {longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}))();");
                    break;

            }

            // Reseting all textboxes to empty string.
            foreach(UIElement element in actionStack.Children) {
                if (element is StackPanel stackPanel) {

                    foreach (UIElement element2 in stackPanel.Children) {
                        if (element2 is TextBox textBox) {
                            textBox.Text = "";
                        }
                    }

                }
            }

        }

        private void AddPointInput(object sender, RoutedEventArgs e) {

            TextBlock newTextBlock = new TextBlock {
                Text = $"Point {points.Count / 2 + 1}",
                FontSize = 15,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255))
            };

            TextBox newTextBoxX = new TextBox();
            TextBox newTextBoxY = new TextBox();

            newTextBoxX.KeyDown += NextField;
            newTextBoxY.KeyDown += NextField;
            newTextBoxY.Tag = "LastInStack";

            if (polygonStack.Children[polygonStack.Children.Count - 1] is TextBox textBox) {
                textBox.Tag = "";
            }

            polygonStack.Children.Add(newTextBlock);
            polygonStack.Children.Add(newTextBoxX);
            polygonStack.Children.Add(newTextBoxY);

            points.Add(newTextBoxX);
            points.Add(newTextBoxY);

        }

        private void NextField(object sender, KeyEventArgs e) {

            if (e.Key == Key.Enter) {

                TextBox textBox = (TextBox)sender;
                StackPanel parent = (StackPanel)textBox.Parent;

                int index = parent.Children.IndexOf(textBox);
                string tag = ((TextBox)parent.Children[index]).Tag?.ToString();

                if (tag == "LastInStack") {
                    Approve(null, null);
                    return;
                }

                try {
                    index += parent.Children[index + 1] is TextBox ? 1 : 2;
                    parent.Children[index].Focus();
                } catch { }
                
            }

        }

    }
}
