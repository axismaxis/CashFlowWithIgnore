﻿using System;
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
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Maps;
using CashFlow.GPS;
using CashFlow.GameLogic;
using Windows.Devices.Geolocation.Geofencing;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CashFlow.GUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapsPage : Page
    {
        //Controlling the map
        private MapController mapController;

        //Used for position tracking
        GPSHandler gpsHandler;
        


        //List that keeps track of the building that the player can visit
        public List<Building> buildingsVistiting = new List<Building>();

        public MapsPage()
        {
            this.InitializeComponent();
            mapController = new MapController(MyMap);
            gpsHandler = new GPSHandler();
            this.Loaded += page_Loaded;
            this.Unloaded += MapsPage_Unloaded;
        }

        private void MapsPage_Unloaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void page_Loaded(object sender, RoutedEventArgs args)
        {
            //Init map
            mapController.InitMap();

            //Init GPS
            bool succesfullConnect = await gpsHandler.RequestUserAccesAsync();
            if(succesfullConnect)
            {
                //Init gpshandler with movement threshold
                gpsHandler.InitGPSHandler(1);

                //Subscribe method for continuous location changes
                gpsHandler.SubscribeToLocation(GpsHandler_positionChangedEvent);

                foreach(Building b in mapController.buildingList)
                {
                    mapController.addGeofence(b.Posistion, 100, b.Name);
                    mapController.DrawCircle(b.Posistion, 100);
                }
                mapController.GeofenceEnteredEventTriggered += MapController_GeofenceEnteredEventTriggered;
                mapController.GeofenceExitedEventTriggered += MapController_GeofenceExitedEventTriggered;
            }
        }

        

        private void MapController_GeofenceEnteredEventTriggered(Geofence geofence)
        {
            //Do shit when geofence leaves
            foreach (Building b in mapController.buildingList)
            {
                if (geofence.Id.Equals(b.Name))
                {
                    mapController.centerMap(b.Posistion);
                    buildingsVistiting.Add(b);
                }
            }
        }

        private void MapController_GeofenceExitedEventTriggered(Geofence geofence)
        {
            foreach (Building b in mapController.buildingList)
            {
                if (geofence.Id.Equals(b.Name))
                {
                    mapController.centerMap(b.Posistion);
                    buildingsVistiting.Remove(b);
                }
            }
        }


        private async void GpsHandler_positionChangedEvent(Geoposition newPosition)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                mapController.centerMap(newPosition);
                mapController.ZoomMap(17);
                mapController.DrawPlayer(newPosition);
            });
        }

        private void HamburgerButton_OnClick(object sender, RoutedEventArgs e)
        {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
           // SplitView.IsEnabled = !SplitView.IsEnabled;
        }

        private void MyMap_OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            mapController.OnMapElementCLick(sender, args, buildingsVistiting);
        }

        private void Houses_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AcountListBox.IsSelected)
            {
                Frame.Navigate(typeof(AcountPage));
            }




            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
        }

       
    }


}
