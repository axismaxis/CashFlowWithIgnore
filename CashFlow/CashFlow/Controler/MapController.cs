using CashFlow.GameLogic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using CashFlow.Storage;

namespace CashFlow.Controler
{
    public class MapController
    {
        private MapControl MyMap;

        //Keeps track of element that represents the player
        private MapPolygon playerCircle;
        List<Building> buildingList = new List<Building>();

        public MapController(MapControl myMap)
        {
            this.MyMap = myMap;

            addMapElement("home", new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586266 }, "HomeTypetrue.png");
            //getJSONBuildings();
            test();
            drawBuildingList(buildingList);
            //drawRoute(new Geopoint(new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586267 }), new Geopoint(new BasicGeoposition { Longitude = 4.0, Latitude = 51.0 }));

        }

        public void InitMap()
        {
            MyMap.ColorScheme = MapColorScheme.Dark;
            MyMap.LandmarksVisible = true;
            MyMap.DesiredPitch = 65;
            MyMap.ZoomLevel = 17;
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

        public async void getJSONBuildings()
        {
            buildingList = await JsonSave.getBuildingList();
        }

        public void clearMapMarkers()
        {
            MyMap.MapElements.Clear();
        }

        public void drawBuildingList(List<Building> list)
        {
            foreach (Building building in list)
            {
                addBuilding(building);
            }
        }

        private void addBuilding(Building building)
        {
            var ancherPoint = new Point(0.5, 1);
            var image =
                RandomAccessStreamReference.CreateFromUri(
                    new Uri("ms-appx:///Res/" + building.GetBuidlingType() + building.IsBought() + ".png"));
            var BuildingElement = new MapIcon
            {
                Title = building.Name,
                Location = new Geopoint(building.getPosistion()),
                NormalizedAnchorPoint = ancherPoint,
                Image = image
            };
            BuildingElement.AddData(building);
            MyMap.MapElements.Add(BuildingElement);
        }

        public void centerMap(BasicGeoposition center)
        {
            MyMap.Center = new Geopoint(center);
        }

        public void centerMap(Geoposition center)
        {
            MyMap.Center = center.Coordinate.Point;
        }

        /// <summary>
        /// Zooms the map
        /// </summary>
        /// <param name="zoomLvl">Int between 1-20 for zoomlevels</param>
        public void ZoomMap(int zoomLvl)
        {
            MyMap.ZoomLevel = zoomLvl;
        }

        public void drawPlayer(Geoposition drawLocation)
        {
            deletePlayerFromMapList(MyMap);

            Color FillColor = Colors.Blue;
            Color StrokeColor = Colors.Black;
            FillColor.A = 255;
            StrokeColor.A = 255;
            playerCircle = new MapPolygon
            {
                StrokeThickness = 2,
                FillColor = FillColor,
                StrokeColor = StrokeColor,
                Path = new Geopath(CalculateCircle(drawLocation.Coordinate.Point.Position, 10)),
                ZIndex = 100
            };
            MyMap.MapElements.Add(playerCircle);
            centerMap(drawLocation);

        }

        private void deletePlayerFromMapList(MapControl myMap)
        {
            while (myMap.MapElements.Contains(playerCircle))
            {
                myMap.MapElements.Remove(playerCircle);
            }
        }

        public void DrawCircle(BasicGeoposition CenterPosition, int Radius)
        {
            Color FillColor = Colors.Purple;
            Color StrokeColor = Colors.Red;
            FillColor.A = 80;
            StrokeColor.A = 80;
            var Circle = new MapPolygon
            {
                StrokeThickness = 2,
                FillColor = FillColor,
                StrokeColor = StrokeColor,
                Path = new Geopath(CalculateCircle(CenterPosition, Radius))
            };
            MyMap.MapElements.Add(Circle);
        }

        public void DrawCircle(Geoposition newPos, int Radius)
        {
            BasicGeoposition CenterPosition = newPos.Coordinate.Point.Position;
            DrawCircle(CenterPosition, Radius);
        }

        const double earthRadius = 6371000D;
        const double Circumference = 2D * Math.PI * earthRadius;

        private static List<BasicGeoposition> CalculateCircle(BasicGeoposition Position, double Radius)
        {
            List<BasicGeoposition> GeoPositions = new List<BasicGeoposition>();
            for (int i = 0; i <= 360; i++)
            {
                double Bearing = ToRad(i);
                double CircumferenceLatitudeCorrected = 2D * Math.PI * Math.Cos(ToRad(Position.Latitude)) * earthRadius;
                double lat1 = Circumference / 360D * Position.Latitude;
                double lon1 = CircumferenceLatitudeCorrected / 360D * Position.Longitude;
                double lat2 = lat1 + Math.Sin(Bearing) * Radius;
                double lon2 = lon1 + Math.Cos(Bearing) * Radius;
                BasicGeoposition NewBasicPosition = new BasicGeoposition();
                NewBasicPosition.Latitude = lat2 / (Circumference / 360D);
                NewBasicPosition.Longitude = lon2 / (CircumferenceLatitudeCorrected / 360D);
                GeoPositions.Add(NewBasicPosition);
            }
            return GeoPositions;
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
                MyMap.Routes.Add(viewOfRoute);
            }
            else
            {
                Debug.WriteLine("route no succes");
            }
        }


        public async void onMapElementCLick(MapControl sender, MapElementClickEventArgs args)
        {
            MapIcon myClickedIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;

            Building ClickedBuilding = myClickedIcon.ReadData();

            var dialog = new MessageDialog(
                ClickedBuilding.type + "\r" + ClickedBuilding.price + "\r" + ClickedBuilding.EarningsP_S + "\r" + "gekocht: " + ClickedBuilding.Bought, ClickedBuilding.Name );
            await dialog.ShowAsync();
        }

        public async void test()
        {
            List<Building> list = fillTest();
            JsonSave.saveBuildingdata(list);

            list = await JsonSave.getBuildingList();

            drawBuildingList(list);

        }

        public List<Building> fillTest()
        {
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
