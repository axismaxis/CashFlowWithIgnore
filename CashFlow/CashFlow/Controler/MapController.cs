using CashFlow.GameLogic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using CashFlow.Storage;
using Windows.Devices.Geolocation.Geofencing;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using CashFlow.Acount;

namespace CashFlow.Controler
{
    public class MapController
    {
        private readonly MapControl _myMap;

        //Keeps track of element that represents the player
        private MapPolygon _playerCircle;
        public List<Building> buildingList = new List<Building>();
        ContentDialog dialog = new ContentDialog();

        private IList<Geofence> geofences = GeofenceMonitor.Current.Geofences;
        public delegate void OnGeofenceTriggered(Geofence geofence);
        public event OnGeofenceTriggered GeofenceEnteredEventTriggered;
        public event OnGeofenceTriggered GeofenceExitedEventTriggered;
        private AccountInfo account;
        private Building ClickedBuilding;

        public MapController(MapControl myMap)
        {
            this._myMap = myMap;

           Debug.WriteLine("now in mappage" + _myMap);
            dialog.Hide();
            dialog.FullSizeDesired = true;
            dialog.PrimaryButtonClick += Dialog_CloseButton;
            //drawRoute(new Geopoint(new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586267 }), new Geopoint(new BasicGeoposition { Longitude = 4.0, Latitude = 51.0 }));
            GeofenceMonitor.Current.GeofenceStateChanged += OnGeofenceStateChanged;
        }

        private async void OnGeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();

            await CoreApplication.MainView.Dispatcher.RunAsync
            (CoreDispatcherPriority.Normal, () =>
            {
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;
                    Geofence geofence = report.Geofence;

                    if (state == GeofenceState.Entered)
                    {
                        GeofenceEnteredEventTriggered?.Invoke(geofence);
                    }
                    else if(state == GeofenceState.Exited)
                    {
                        GeofenceExitedEventTriggered?.Invoke(geofence);
                    }
                }
            });
        }

        public void InitMap()
        {
            _myMap.ColorScheme = MapColorScheme.Dark;
            _myMap.LandmarksVisible = true;
            _myMap.DesiredPitch = 55;
            _myMap.ZoomLevel = 17;
        }

        public void initBuildings()
        {
            GetJsonBuildings();
        }

        public void addGeofence(BasicGeoposition position, double radius, string geofenceName)
        {
            Geofence newGeofence = GenerateGeofence(position, radius, geofenceName);
            bool existingGeofence = false;
            foreach(Geofence g in geofences)
            {
                if (g.Id.Equals(newGeofence.Id))
                {
                    existingGeofence = true;
                }
            }
            if(!existingGeofence)
            {
                geofences.Add(newGeofence);
            }
        }

        private Geofence GenerateGeofence(BasicGeoposition position, double radius, string geofenceName)
        {
            string geofenceId = geofenceName;
            // the geofence is a circular region:
            Geocircle geocircle = new Geocircle(position, radius);

            bool singleUse = false;

            MonitoredGeofenceStates mask = MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited;

            TimeSpan dwellTime = new TimeSpan(0,0,1);

            return new Geofence(geofenceId, geocircle, mask, singleUse, dwellTime);
        }


        public void AddMapElement(string title, BasicGeoposition posistion, string imgName)
        {
            var ancherPoint = new Point(0.5, 1);
            var image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Res/" + imgName));

            var shape = new MapIcon
            {
                Title = title,
                Location = new Geopoint(posistion),
                NormalizedAnchorPoint = ancherPoint,
                Image = image
            };

            _myMap.MapElements.Add(shape);
        }

        public async Task GetJsonBuildings()
         {
            try
            {
                //Get buildings from json
                buildingList = await JsonSave.getBuildingList();

                //Add home location from json
                account = await JsonSave.LoadPersonalDataFromJson();
                BasicGeoposition position = new BasicGeoposition();
                position.Latitude = account.getLatitude();
                position.Longitude = account.getLongitude();
                buildingList.Add(new Home("Home", 0, 123, position, true));
                account.setEarnings(9000000);
            }
            catch (Exception)
            {
                //Test();
                //GetJsonBuildings();            
            }

            DrawBuildingList(buildingList);
        }

        public void ClearMapMarkers()
        {
            _myMap.MapElements.Clear();
        }

        public void DrawBuildingList(List<Building> list)
        {
            foreach (Building building in list)
            {
                AddBuilding(building);
            }
        }

        private void AddBuilding(Building building)
        {
            var ancherPoint = new Point(0.5, 1);
            var image =
                RandomAccessStreamReference.CreateFromUri(
                    new Uri("ms-appx:///Res/" + building.GetBuidlingType() + building.IsBought() + ".png"));
            var buildingElement = new MapIcon
            {
                Title = building.Name,
                Location = new Geopoint(building.getPosistion()),
                NormalizedAnchorPoint = ancherPoint,
                Image = image,
                ZIndex = 20
            };
            buildingElement.AddData(building);
            _myMap.MapElements.Add(buildingElement);
        }

        public void centerMap(BasicGeoposition center)
        {
            _myMap.Center = new Geopoint(center);
        }

        public void centerMap(Geoposition center)
        {
            _myMap.Center = center.Coordinate.Point;
        }

        /// <summary>
        /// Zooms the map
        /// </summary>
        /// <param name="zoomLvl">Int between 1-20 for zoomlevels</param>
        public void ZoomMap(int zoomLvl)
        {
            _myMap.ZoomLevel = zoomLvl;
        }

        public void DrawPlayer(Geoposition drawLocation)
        {
            DeletePlayerFromMapList(_myMap);

            Color fillColor = Colors.Blue;
            Color strokeColor = Colors.Black;
            fillColor.A = 255;
            strokeColor.A = 255;
            _playerCircle = new MapPolygon
            {
                StrokeThickness = 2,
                FillColor = fillColor,
                StrokeColor = strokeColor,
                Path = new Geopath(CalculateCircle(drawLocation.Coordinate.Point.Position, 10)),
                ZIndex = 5
            };
            _myMap.MapElements.Add(_playerCircle);
            centerMap(drawLocation);

        }

        private void DeletePlayerFromMapList(MapControl myMap)
        {
            while (myMap.MapElements.Contains(_playerCircle))
            {
                myMap.MapElements.Remove(_playerCircle);
            }
        }

        public void DrawCircle(BasicGeoposition CenterPosition, int Radius)
        {
            Color fillColor = Colors.Purple;
            Color strokeColor = Colors.Red;
            fillColor.A = 80;
            strokeColor.A = 80;
            var circle = new MapPolygon
            {
                StrokeThickness = 2,
                FillColor = fillColor,
                StrokeColor = strokeColor,
                Path = new Geopath(CalculateCircle(CenterPosition, Radius))
            };
            _myMap.MapElements.Add(circle);
        }

        public void DrawCircle(Geoposition newPos, int Radius)
        {
            BasicGeoposition centerPosition = newPos.Coordinate.Point.Position;
            DrawCircle(centerPosition, Radius);
        }

        const double EarthRadius = 6371000D;
        const double Circumference = 2D * Math.PI * EarthRadius;

        private static List<BasicGeoposition> CalculateCircle(BasicGeoposition Position, double Radius)
        {
            List<BasicGeoposition> geoPositions = new List<BasicGeoposition>();
            for (int i = 0; i <= 360; i++)
            {
                double Bearing = ToRad(i);
                double CircumferenceLatitudeCorrected = 2D * Math.PI * Math.Cos(ToRad(Position.Latitude)) * EarthRadius;
                double lat1 = Circumference / 360D * Position.Latitude;
                double lon1 = CircumferenceLatitudeCorrected / 360D * Position.Longitude;
                double lat2 = lat1 + Math.Sin(Bearing) * Radius;
                double lon2 = lon1 + Math.Cos(Bearing) * Radius;
                BasicGeoposition NewBasicPosition = new BasicGeoposition();
                NewBasicPosition.Latitude = lat2 / (Circumference / 360D);
                NewBasicPosition.Longitude = lon2 / (CircumferenceLatitudeCorrected / 360D);
                geoPositions.Add(NewBasicPosition);
            }
            return geoPositions;
        }

        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180D);
        }

        public async void drawRoute(Geopoint pointA, Geopoint pointB)
        {
            MapRouteFinderResult routeFinderResult = await MapRouteFinder.GetWalkingRouteAsync(pointA, pointB);

            if (routeFinderResult.Status == MapRouteFinderStatus.Success)
            {
                MapRouteView viewOfRoute = new MapRouteView(routeFinderResult.Route);
                viewOfRoute.RouteColor = Colors.DodgerBlue;
                viewOfRoute.OutlineColor = Colors.LightBlue;
                _myMap.Routes.Add(viewOfRoute);
            }
            else
            {
                Debug.WriteLine("route no succes");
            }
        }


        public async void OnMapElementCLick(MapControl sender, MapElementClickEventArgs args, List<Building> buildingsVisited)
        {
            MapIcon myClickedIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            Building clickedBuilding = myClickedIcon.ReadData();
            Grid myGrid = new Grid();
            ListView LV = new ListView();

            LV.DataContext = clickedBuilding.type + "\r" + clickedBuilding.price + "\r" + clickedBuilding.EarningsP_S +
                             "\r" + "gekocht: " + clickedBuilding.Bought;
           
            myGrid.Children.Add(LV);
            dialog.Title = clickedBuilding.Name;

            dialog.Content = "building type: " + clickedBuilding.type + "\r" + "price of this building: "+ clickedBuilding.price + "\r" + "earnings per seccond: "+ clickedBuilding.EarningsP_S +
                             "\r" + "is Bought =  " + clickedBuilding.Bought; 
            dialog.PrimaryButtonText = "Close";
            this.ClickedBuilding = clickedBuilding;
            if (clickedBuilding.Bought)
            {
                dialog.SecondaryButtonText = "Not in range";

                dialog.IsSecondaryButtonEnabled = false;
                foreach (Building b in buildingsVisited)
                {
                    if (b.Name.Equals(clickedBuilding.Name))
                    {
                        dialog.IsSecondaryButtonEnabled = true;
                        dialog.SecondaryButtonText = "Collect";
                    }
                }

                dialog.SecondaryButtonClick += collectButton_Click;
            }
            else
            {
                dialog.SecondaryButtonText = "buy for: " + clickedBuilding.price;
                dialog.SecondaryButtonClick += buyButton_click;
                dialog.IsSecondaryButtonEnabled = false;
                foreach (Building b in buildingsVisited)
                {
                    if (b.Name.Equals(clickedBuilding.Name))
                    {
                        dialog.IsSecondaryButtonEnabled = true;
                        dialog.SecondaryButtonText = "buy for: " + clickedBuilding.price;
                    }
                }
                dialog.SecondaryButtonClick += buyButton_click;
            }
             await dialog.ShowAsync();
        }

        private async void buyButton_click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            sender.IsEnabled = true;
            if (!ClickedBuilding.IsBought())
            {
                if (ClickedBuilding.price < account.GetEarnings())
                {
                    account.setEarnings(account.GetEarnings() - ClickedBuilding.price);
                    List<AccountInfo> acc = new List<AccountInfo>();
                    acc.Add(account);
                    // await JsonSave.SavePersonalDataToJson(acc);
                    await buyBuilding(ClickedBuilding);
                    RedrawBuilding(ClickedBuilding);
                }
                else
                {
                    args.Cancel = true;
                    sender.Title = "not enough money";
                    sender.Content =
                        "You do not have enough money to buy the following building: " + ClickedBuilding.Name + ". \r \r you need to earn " +
                        (ClickedBuilding.price - account.GetEarnings()) + " more";
                    sender.PrimaryButtonText = "close";
                    sender.SecondaryButtonText = "";
                    sender.UpdateLayout();
                    sender.FullSizeDesired = false;
                }

            }
        }

        private void RedrawBuilding(Building clickedBuilding)
        {
            foreach (MapElement mapElement in _myMap.MapElements)
            {
                if (mapElement.ReadData() == clickedBuilding)
                {
                    _myMap.MapElements.RemoveAt(_myMap.MapElements.IndexOf(mapElement));
                    AddBuilding(clickedBuilding);
                    break;
                }            
            }
        }
        private async Task buyBuilding(Building building)
        {
            int index = buildingList.IndexOf(building);
            buildingList[index].Bought = true;
            ClickedBuilding = buildingList[index];
            JsonSave.saveBuildingdata(buildingList);
        }
        private void collectButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //throw new NotImplementedException();
        }

        private void Dialog_CloseButton(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            dialog.Hide();
        }

        private string CalcOwnedBuildings()
        {
            int BoughtIndex = 0;
            foreach (Building building in buildingList)
            {
                if (building.IsBought())
                {
                    BoughtIndex = BoughtIndex + 1;
                }
            }
            return BoughtIndex.ToString();
        }
        public async void showContent(String name)
        {
            if (name.Equals("acount"))
            {

                String content = "Account name: " + account.GetName() + "\r" +
                                 "Total earnings: " + account.GetEarnings() + "\r" +
                                 "Total owned buildings: " + CalcOwnedBuildings();

                ContentDialog Dialog = new ContentDialog
                {
                    Title = "Acount info",
                    Content = content
                };
                Dialog.PrimaryButtonText = "Close";
                Dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
                await Dialog.ShowAsync();
            }
        }

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            sender.Hide();
        }

        public static async Task<bool> Test()
        {
            Debug.WriteLine("in test");

            List<Building> list = FillTest();
            JsonSave.saveBuildingdata(list);


            return true;

        }

        public static List<Building> FillTest()
        {
            Debug.WriteLine("filling building list");
            List<Building> list = new List<Building>();

            list.Add(new Wonder(
                "Kerk van Breda",
                12000000,
                123,
                new BasicGeoposition { Longitude = 4.7752340, Latitude = 51.5890150 },
                true

            ));

            list.Add(new Monument(
                "Chasse theater",
                500000,
                123,
                new BasicGeoposition { Longitude = 4.7818280, Latitude = 51.5873440 },
                false
                ));

            list.Add(new House(
                "keizerstraat 45",
                190000,
                123,
                new BasicGeoposition { Latitude = 51.5849670, Longitude = 4.7788590 },
                true
                ));
            return list;
        }
    }
}
