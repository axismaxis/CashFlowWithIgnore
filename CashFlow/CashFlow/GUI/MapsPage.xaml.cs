using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CashFlow.Controler;
using Windows.Devices.Geolocation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CashFlow.GUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapsPage : Page
    {
        //Controlling the map
        private MapControler mapController;

        //Used for position tracking
        private bool positionSet = false;
        Geolocator geolocator;

        public MapsPage()
        {
            this.InitializeComponent();
            mapController = new MapControler(MyMap);
            this.Loaded += page_Loaded;
        }

        private async void page_Loaded(object sender, RoutedEventArgs args)
        {
            GeolocationAccessStatus accessStatus = await Geolocator.RequestAccessAsync();
            switch(accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    geolocator = new Geolocator { DesiredAccuracy = PositionAccuracy.Default, MovementThreshold = 10};
                    geolocator.PositionChanged += Geolocator_PositionChanged;

                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        mapController.centerMap(pos);
                    });
                    break;
                case GeolocationAccessStatus.Denied:
                    break;

                case GeolocationAccessStatus.Unspecified:
                    break;
            }
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
             {
                 mapController.centerMap(args.Position);
                 mapController.DrawCircle(args.Position, 50);
             });

            positionSet = true;
        }

        private void HamburgerButton_OnClick(object sender, RoutedEventArgs e)
        {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
        }
    }
}
