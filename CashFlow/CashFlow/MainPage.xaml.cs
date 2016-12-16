using CashFlow.Acount;
using CashFlow.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CashFlow.Controler;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CashFlow
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Mapinit();
            centerMap(new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586267 });
            addMapElement("home", new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586267 }, "HomePin.png");
            drawRoute(new Geopoint(new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586267 }), new Geopoint(new BasicGeoposition { Longitude = 4.0, Latitude = 51.0 }));


            this.Loaded += SignUpPage_Loaded;
        }

        private void Mapinit()
        {
            MyMap.ColorScheme = MapColorScheme.Dark;
            MyMap.LandmarksVisible = true;
            MyMap.DesiredPitch = 45;
        }


        public void addMapElement(string title, BasicGeoposition posistion, string imgName)
        {
            var ancherPoint = new Point(0.5, 1);
            var image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Res/" + imgName));

            var Shape = new MapIcon
            {
                Title = title,
                Location = new Geopoint(posistion),
                NormalizedAnchorPoint = ancherPoint,
                Image = image
            };

            MyMap.MapElements.Add(Shape);
        }

        public void centerMap(BasicGeoposition center)
        {
            MyMap.Center = new Geopoint(center);
            MyMap.ZoomLevel = 25;
        }

        public async void drawRoute(Geopoint pointA, Geopoint pointB)
        {
            MapRouteFinderResult routeFinderResult = await MapRouteFinder.GetWalkingRouteAsync(pointA, pointB);

            if (routeFinderResult.Status == MapRouteFinderStatus.Success)
            {
                MapRouteView viewOfRoute = new MapRouteView(routeFinderResult.Route);
                viewOfRoute.RouteColor = Colors.DodgerBlue;
                viewOfRoute.OutlineColor = Colors.LightBlue;
                MyMap.Routes.Add(viewOfRoute);
            }
            else
            {
                Debug.WriteLine("route no succes");
            }
        }

        private async void SignUpPage_Loaded(object sender, RoutedEventArgs e)
        {
            //AccountInfo loadedAccountInfo = await JsonSave.LoadPersonalDataFromJson();
            //outputBox.Text = loadedAccountInfo.ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            AccountInfo info = new AccountInfo("Kerk", 100.0, 4.7812310, 51.5851460);
            JsonSave.SavePersonalDataToJson(info);
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            AccountInfo loadedAccountInfo = await JsonSave.LoadPersonalDataFromJson();
            outputBox.Text = loadedAccountInfo.ToString();
        }
    }
}
