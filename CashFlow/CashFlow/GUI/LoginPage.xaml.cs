using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CashFlow.GPS;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CashFlow.GUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private GPSHandler gpsHandler;
        private Geoposition position;
        private Geoposition calculatedPosition;


        public LoginPage()
        {

            this.InitializeComponent();
            gpsHandler = new GPSHandler();
            MyMapLoginScreen.ColorScheme = MapColorScheme.Dark;
            this.Loaded += page_Loaded;
        }

        private async void page_Loaded(object sender, RoutedEventArgs args)
        {

            //Init GPS
            bool succesfullConnect = await gpsHandler.RequestUserAccesAsync();
            if (succesfullConnect)
            {
                //Init gpshandler with movement threshold
                gpsHandler.InitGPSHandler(1);

                //Subscribe method for continuous location changes
                gpsHandler.SubscribeToLocation(GpsHandler_positionChangedEvent);
            }
        }

        private async void GpsHandler_positionChangedEvent(Geoposition newPosition)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                position = newPosition;
            });
        }

        private void deleteOldPos()
            {
                foreach (MapElement element in MyMapLoginScreen.MapElements)
                {
                    if (element is MapIcon)
                    {
                        MapIcon icon = (MapIcon) element;
                        if (icon.Title.Equals("Home"))
                        {
                            MyMapLoginScreen.MapElements.RemoveAt(MyMapLoginScreen.MapElements.IndexOf(element));
                            break;
                        }
                    }
                }
            }

            private void CaluculateLocationButton_OnClick(object sender, RoutedEventArgs e)
            {

            MyMapLoginScreen.Center = position.Coordinate.Point;
            MyMapLoginScreen.ZoomLevel = 17;
            deleteOldPos(); 
            var homeLocation = new MapIcon
                {
                    Title = "Home",
                    Location = position.Coordinate.Point,
                    NormalizedAnchorPoint = new Point(0.5,1),
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Res/HomeTypetrue.png"))

                };
                calculatedPosition = position;
                MyMapLoginScreen.MapElements.Add(homeLocation);
        }
    

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
           
            //saving to json plz yass bosss
            //while saving use calculatedPosition plz
            Frame.Navigate(typeof(MapsPage));
            MyMapLoginScreen.IsEnabled = false;
        }

    }
}
