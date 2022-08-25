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
using Newtonsoft.Json.Linq;
<<<<<<< HEAD
=======
using System.IO;
using CefSharp.Handler;
using System.Threading;
>>>>>>> develop

namespace ChromiumWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
<<<<<<< HEAD
    public partial class MainWindow : Window {

        enum Action { None, SetLocation, AddPoint, Addpolyline, AddPolygon, AddImage, CreateModel, LoadStructureModel, MovePoint }
=======
    public partial class MainWindow : Window
    {
        string savedCommands = "";
        enum Action { None, SetLocation, AddPoint, Addpolyline, AddPolygon, AddImage, AddCircle, CreateModel, LoadStructureModel, MovePoint, getAltitude, ExportActions, ImportActions }
>>>>>>> develop

        private Action action = Action.None;
        private readonly List<TextBox> points = new List<TextBox>();
        private readonly List<JObject> polyPoints = new List<JObject>();

<<<<<<< HEAD
        public MainWindow() {
            InitializeComponent();
        }

        private void JsCall(string command) {

            // Calls function in cesium js.
            var frame = defaultBrowser.GetMainFrame();
=======
        private static string signallingServerUrl;

        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();


           
            signallingServerUrl = (string)this.Resources["signallingServerUrl"];
            this.browser00.RequestHandler = new ExampleRequestHandler(signallingServerUrl);
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var frame = defaultBrowser.GetMainFrame();
            var task = frame.EvaluateScriptAsync("document.getElementById(\"realTimeAltData\").innerHTML");
            // Gets value retured from function.
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {

                    var response = t.Result;
                    // Gets value, (json)
                    string EvaluateJavaScriptResult = response.Result?.ToString();
                    if (EvaluateJavaScriptResult == null)
                    { return; }
                    else
                    {
                        altTextBlock.Text = "Altitude: " + EvaluateJavaScriptResult;
                    }

                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void JsCall(string command)
        {

            // Calls function in cesium js.
            var frame = defaultBrowser.GetMainFrame();
            if (!command.Contains("addPolygon"))
            {
                savedCommands = savedCommands + command;
            }
>>>>>>> develop
            frame.EvaluateScriptAsync(command, null);

        }

<<<<<<< HEAD
        private void ChooseAction(object sender, RoutedEventArgs e) {
=======
        private void ChooseAction(object sender, RoutedEventArgs e)
        {
>>>>>>> develop

            string headerAction = ((MenuItem)sender).Header.ToString();
            actionLabel.Content = headerAction;

            // Default values.
            actionStack.Visibility = Visibility.Visible;
            pointStack.Visibility = Visibility.Collapsed;
            pointsStack.Visibility = Visibility.Collapsed;
            urlStack.Visibility = Visibility.Collapsed;
<<<<<<< HEAD

=======
            circleStack.Visibility = Visibility.Collapsed;
>>>>>>> develop
            urlTextblock.Visibility = Visibility.Collapsed;
            urlTextbox.Visibility = Visibility.Collapsed;

            widthTextblock.Visibility = Visibility.Collapsed;
            widthTextbox.Visibility = Visibility.Collapsed;
            opacitySlider.Visibility = Visibility.Collapsed;

            heightTextbox.Tag = "LastInStack";
            urlTextbox.Tag = "";

<<<<<<< HEAD
            switch (headerAction) {

                case "Set location":
                    action = Action.SetLocation;
=======
            switch (headerAction)
            { // Sets action type.

                case "Set location":
                    action = Action.SetLocation;
                    pointIdText.Visibility = Visibility.Collapsed;
                    pointIdTextbox.Visibility = Visibility.Collapsed;
>>>>>>> develop
                    pointStack.Visibility = Visibility.Visible;
                    break;

                case "Add point":
                    action = Action.AddPoint;
<<<<<<< HEAD
=======

                    pointIdText.Visibility = Visibility.Visible;
                    pointIdTextbox.Visibility = Visibility.Visible;
>>>>>>> develop
                    pointStack.Visibility = Visibility.Visible;
                    break;

                case "Add polyline":
                    action = Action.Addpolyline;
                    pointsStack.Visibility = Visibility.Visible;
                    widthTextblock.Visibility = Visibility.Visible;
                    widthTextbox.Visibility = Visibility.Visible;
                    break;

                case "Add polygon":
                    action = Action.AddPolygon;
                    pointsStack.Visibility = Visibility.Visible;
                    opacitySlider.Visibility = Visibility.Visible;
                    break;

                case "Add image":
                    action = Action.AddImage;
                    pointStack.Visibility = Visibility.Visible;
                    urlTextblock.Visibility = Visibility.Visible;
                    urlTextbox.Visibility = Visibility.Visible;
                    heightTextbox.Tag = "";
                    urlTextbox.Tag = "LastInStack";
                    break;

<<<<<<< HEAD
                case "Create model":
                    action = Action.CreateModel;
                    pointStack.Visibility = Visibility.Visible;
                    urlTextblock.Visibility = Visibility.Visible;
                    urlTextbox.Visibility = Visibility.Visible;
                    heightTextbox.Tag = "";
                    urlTextbox.Tag = "LastInStack";
=======
                case "Add circle":
                    action = Action.AddCircle;
                    circleStack.Visibility = Visibility.Visible;
                    break;

                case "Create model":
                    action = Action.CreateModel;
                    DropdownStack.Visibility = Visibility.Visible;
                    ModelMenu.Visibility = Visibility.Visible;
                    urlTextbox.Visibility = Visibility.Visible;
                    modelheightTextbox.Tag = "";
                    ModelMenu.Tag = "LastInStack";
>>>>>>> develop
                    break;

                case "Load structure model":
                    action = Action.LoadStructureModel;
<<<<<<< HEAD
=======
                    ModelMenu.Visibility = Visibility.Collapsed;
>>>>>>> develop
                    urlStack.Visibility = Visibility.Visible;
                    break;

                case "Move point":
                    action = Action.MovePoint;
                    pointStack.Visibility = Visibility.Visible;
                    break;
<<<<<<< HEAD

                case "Clear points":
                    JsCall($"clearPoints();");
                    pointStack.Visibility = Visibility.Visible;
=======
                case "Get altitude of point":
                    action = Action.getAltitude;
                    pointStack.Visibility = Visibility.Visible;
                    break;


                case "Clear points":
                    JsCall($"clearPoints();");
                    break;
                case "Export actions":
                    action = Action.ExportActions;
                    urlStack.Visibility = Visibility.Visible;
                    break;
                case "Import actions":
                    action = Action.ImportActions;
                    urlStack.Visibility = Visibility.Visible;
>>>>>>> develop
                    break;

            }

        }

<<<<<<< HEAD
        private void ModeSwitch(object sender, RoutedEventArgs e) {
            JsCall($"modeSwitch();"); // Switch camera view 2D/3D.
        }

        private void Approve(object sender, RoutedEventArgs e) {

            switch (action) {
=======
        private void ModeSwitch(object sender, RoutedEventArgs e)
        {
            JsCall($"modeSwitch();"); // Switch camera view 2D/3D.
        }

        private void Approve(object sender, RoutedEventArgs e)
        {
            switch (action)
            {
>>>>>>> develop

                case Action.SetLocation: // Moves camera to position.
                    JsCall($"moveTo({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text});");
                    break;

                case Action.AddPoint:
                    JsCall($"addPoint('{pointIdTextbox.Text}', {longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text});");
                    break;

                case Action.Addpolyline:

                    JsCall($"addPolyline({BuildPointsArray(polyPoints)}, {widthTextbox.Text});");
<<<<<<< HEAD
=======
                    JsCall($"clearPoints({BuildIdsArray(polyPoints)});");
>>>>>>> develop
                    ClearPoints();
                    polyPoints.Clear();

                    break;

                case Action.AddPolygon:

<<<<<<< HEAD
                    string height = polygonHeightTextbox.Text == "" ? "100" : polygonHeightTextbox.Text;
                    JsCall($"addPolygon({BuildPointsArray(polyPoints)}, {height}, {opacitySlider.Value});");
=======
                    string height = polygonHeightTextbox.Text == "" ? "100" : polygonHeightTextbox.Text; // Default value.
                    JsCall($"addPolygon({BuildPointsArray(polyPoints)}, {height}, {opacitySlider.Value});");
                    JsCall($"clearPoints({BuildIdsArray(polyPoints)});");
>>>>>>> develop
                    ClearPoints();
                    polyPoints.Clear();

                    break;

                case Action.AddImage:
                    JsCall($"addImage({longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text}, '{urlTextbox.Text}');");
                    break;
<<<<<<< HEAD

                case Action.CreateModel:
                    JsCall($"createModel('{urlTextbox.Text}', {longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text});");
=======
                case Action.AddCircle:
                    JsCall($"addCircle({circlelongitudeTextbox.Text}, {circlelatitudeTextbox.Text}, {circleheightTextbox.Text}, {semiMinorAxisTextbox.Text}, {semiMajorAxisTextbox.Text});");
                    break;
                case Action.CreateModel:
                    string modelurl = "";
                    Console.WriteLine(ModelMenu.SelectedItem.ToString());

                    ComboBoxItem selected = (ComboBoxItem)ModelMenu.SelectedItem;
                    if (selected.Content.ToString() == "Drone")
                    {
                        modelurl = "./3dmodels/Drone.glb";
                    }
                    else if (selected.Content.ToString() == "Plane")
                    {
                        modelurl = "./3dmodels/Plane.glb";
                    }
                    else
                    {
                        modelurl = "./3dmodels/house.glb";
                    }
                    JsCall($"createModel('{modelurl}', {modellongitudeTextbox.Text}, {modellatitudeTextbox.Text}, {modelheightTextbox.Text});");
>>>>>>> develop
                    break;

                case Action.LoadStructureModel:
                    JsCall($"loadStructureModel('{urlStackTextbox.Text}');");
                    break;
<<<<<<< HEAD
                
                case Action.MovePoint:
                    JsCall($"movePoint('{pointIdTextbox.Text}', {longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text});");
                    break;

            }

            // Reseting all textboxes to empty string.
            foreach(UIElement element in actionStack.Children) {
                if (element is StackPanel stackPanel) {

                    foreach (UIElement element2 in stackPanel.Children) {
                        if (element2 is TextBox textBox) {
=======

                case Action.MovePoint:
                    JsCall($"movePoint('{pointIdTextbox.Text}', {longitudeTextbox.Text}, {latitudeTextbox.Text}, {heightTextbox.Text});");
                    break;
                case Action.getAltitude:
                    // Calls function in cesium js.
                    var frame = defaultBrowser.GetMainFrame();
                    if (pointIdTextbox.Text.Length == 0)
                    {
                        JsCall($"getAltitOfPointByLocation({ longitudeTextbox.Text}, { latitudeTextbox.Text});");
                    }
                    else
                    {
                        JsCall($"getAltitOfPointById(\"{pointIdTextbox.Text}\");");
                    }
                    Thread.Sleep(1000);
                    var task = frame.EvaluateScriptAsync("getAltData()");
                    // Gets value retured from function.
                    task.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {

                            var response = t.Result;
                            // Gets value, (json)
                            string EvaluateJavaScriptResult = response.Result?.ToString();
                            if (EvaluateJavaScriptResult == null)
                            { return; }
                            else
                            {
                                MessageBox.Show("the altitude result: " + EvaluateJavaScriptResult);
                            }

                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    break;

                case Action.ExportActions:
                    // Calls function in cesium js.
                    var exportframe = defaultBrowser.GetMainFrame();
                    var exporttask = exportframe.EvaluateScriptAsync("printToExport()", null);

                    // Gets value retured from function.
                    exporttask.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {

                            var response = t.Result;
                            // Gets value, (json)
                            string EvaluateJavaScriptResult = response.Result?.ToString();

                            if (EvaluateJavaScriptResult == null)
                            { return; }
                            else
                            {
                                savedCommands += EvaluateJavaScriptResult;
                            }

                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    File.WriteAllText(urlStackTextbox.Text, savedCommands);
                    break;
                case Action.ImportActions:
                    string importedCalls = File.ReadAllText(urlStackTextbox.Text);
                    JsCall(importedCalls);
                    break;
            }

            // Reseting all textboxes to empty string.
            foreach (UIElement element in actionStack.Children)
            {
                if (element is StackPanel stackPanel)
                {

                    foreach (UIElement element2 in stackPanel.Children)
                    {
                        if (element2 is TextBox textBox)
                        {
>>>>>>> develop
                            textBox.Text = "";
                        }
                    }

                }
            }
<<<<<<< HEAD

        }

        private void ClearPoints() {

            // Clear point inputs.
            int childCount = pointsStack.Children.Count;
            for (int i = 0; i < points.Count() / 2; i++) {
=======
            action = Action.None;
            actionLabel.Content = "";
            actionStack.Visibility = Visibility.Collapsed;
            pointStack.Visibility = Visibility.Collapsed;
            pointsStack.Visibility = Visibility.Collapsed;
            urlStack.Visibility = Visibility.Collapsed;

            urlTextblock.Visibility = Visibility.Collapsed;
            urlTextbox.Visibility = Visibility.Collapsed;

            widthTextblock.Visibility = Visibility.Collapsed;
            widthTextbox.Visibility = Visibility.Collapsed;
            opacitySlider.Visibility = Visibility.Collapsed;
        }

        private void ClearPoints()
        {

            // Resets polystack by removing all point inputs.
            int childCount = pointsStack.Children.Count;
            for (int i = 0; i < points.Count() / 2; i++)
            {
>>>>>>> develop
                pointsStack.Children.RemoveAt(childCount - 1 - 3 * i);
                pointsStack.Children.RemoveAt(childCount - 2 - 3 * i);
                pointsStack.Children.RemoveAt(childCount - 3 - 3 * i);
            }
            points.Clear();

        }

<<<<<<< HEAD
        private void AddPointInput(object sender, RoutedEventArgs e) {

            TextBlock newTextBlock = new TextBlock {
=======
        private void AddPointInput(object sender, RoutedEventArgs e)
        {

            // Adds point input for polyshape.
            TextBlock newTextBlock = new TextBlock
            {
>>>>>>> develop
                Text = $"Point {points.Count / 2 + 1}",
                FontSize = 15,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255))
            };

            TextBox newTextBoxX = new TextBox();
            TextBox newTextBoxY = new TextBox();

            newTextBoxX.KeyDown += NextField;
            newTextBoxY.KeyDown += NextField;
            newTextBoxY.Tag = "LastInStack";

<<<<<<< HEAD
            if (pointsStack.Children[pointsStack.Children.Count - 1] is TextBox textBox) {
=======
            if (pointsStack.Children[pointsStack.Children.Count - 1] is TextBox textBox)
            {
>>>>>>> develop
                textBox.Tag = "";
            }

            pointsStack.Children.Add(newTextBlock);
            pointsStack.Children.Add(newTextBoxX);
            pointsStack.Children.Add(newTextBoxY);

            points.Add(newTextBoxX);
            points.Add(newTextBoxY);

        }

<<<<<<< HEAD
        private void NextField(object sender, KeyEventArgs e) {

            if (e.Key == Key.Enter) {
=======
        private void NextField(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            { // Focuses on next textbox if pressed enter.
>>>>>>> develop

                TextBox textBox = (TextBox)sender;
                StackPanel parent = (StackPanel)textBox.Parent;

                int index = parent.Children.IndexOf(textBox);
                string tag = ((TextBox)parent.Children[index]).Tag?.ToString();

<<<<<<< HEAD
                if (tag == "LastInStack") {
=======
                if (tag == "LastInStack")
                { // If last input, approve automatically.
>>>>>>> develop
                    Approve(null, null);
                    return;
                }

<<<<<<< HEAD
                try {
                    index += parent.Children[index + 1] is TextBox ? 1 : 2;
                    parent.Children[index].Focus();
                } catch { }
                
=======
                try
                {
                    index += parent.Children[index + 1] is TextBox ? 1 : 2; // Find next textbox to focus on.
                    parent.Children[index].Focus();
                }
                catch { /* Out of range! */ }

>>>>>>> develop
            }

        }

<<<<<<< HEAD
        private void CesiumClick(object sender, MouseButtonEventArgs e) {
=======
        private void CesiumClick(object sender, MouseButtonEventArgs e)
        {
>>>>>>> develop

            if (action != Action.AddPoint
             && action != Action.Addpolyline
             && action != Action.AddPolygon) return;

            // Calls function in cesium js.
            var frame = defaultBrowser.GetMainFrame();
            var task = frame.EvaluateScriptAsync("addPointByClick()", null);

<<<<<<< HEAD
            task.ContinueWith(t => {
                if (!t.IsFaulted) {

                    var response = t.Result;
                    string EvaluateJavaScriptResult = response.Result?.ToString();

                    if (EvaluateJavaScriptResult == null) return;
                    if (action != Action.AddPoint) {
=======
            // Gets value retured from function.
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {

                    var response = t.Result;
                    // Gets value, (json)
                    string EvaluateJavaScriptResult = response.Result?.ToString();

                    if (EvaluateJavaScriptResult == null) return;
                    if (action != Action.AddPoint)
                    {
>>>>>>> develop
                        if (action == Action.AddPolygon & e.ClickCount == 2)
                        {
                            Console.WriteLine(EvaluateJavaScriptResult);
                        }
                        polyPoints.Add(JObject.Parse(EvaluateJavaScriptResult));
                    }

                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

<<<<<<< HEAD
        private string BuildPointsArray(List<JObject> points) {

            // Builds array of points, format: [x, y, x, y, x, y, ...].
            string pointArray = "[";
            polyPoints.ForEach(point => {
                pointArray += point["position"]["longitude"] + ",";
                pointArray += point["position"]["latitude"] + (point == polyPoints.Last() ? "]" : ",");
=======
        private string BuildPointsArray(List<JObject> points)
        {

            // Builds array of points, format: [x, y, x, y, x, y, ...].
            string pointArray = "[";
            points.ForEach(point =>
            {
                pointArray += point["position"]["longitude"] + ",";
                pointArray += point["position"]["latitude"] + (point == points.Last() ? "]" : ",");
>>>>>>> develop
            });

            return pointArray;

        }

<<<<<<< HEAD
=======
        private string BuildIdsArray(List<JObject> points)
        {

            // Builds array of ids, format: [id1, id2, id3, ...].
            string pointArray = "[";
            points.ForEach(point =>
            {
                pointArray += $"'{point["id"]}'{(point == points.Last() ? "]" : ",")}";
            });

            return pointArray;

        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            defaultBrowser.Reload();
            savedCommands = "";
        }
    }



    //this class overrides RequestHandler:OnCertificateError function,
    //in order to allow invalid certificates when first connecting to signalling server with https.
    public class ExampleRequestHandler : RequestHandler
    {
        private string signallingServerUrl;

        public ExampleRequestHandler(string signallingServerUrl)
        {
            this.signallingServerUrl = signallingServerUrl;
        }

        protected override bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            //Return true and call IRequestCallback.Continue() at a later time to continue or cancel the request.
            //In this instance we'll use a Task, typically you'd invoke a call to the UI Thread and display a Dialog to the user
            Task.Run(() =>
            {
                //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
                if (!callback.IsDisposed)
                {
                    if (requestUrl == this.signallingServerUrl)
                    {
                        using (callback)
                        {
                            //We'll allow the invalid certificate
                            callback.Continue(true);
                        }
                    }
                    else
                    {
                        callback.Continue(false);
                    }
                }
            });
            return true;
        }
>>>>>>> develop
    }
}
