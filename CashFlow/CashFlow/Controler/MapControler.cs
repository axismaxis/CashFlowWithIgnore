using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;
using CashFlow.GameLogic;

namespace CashFlow.Controler
{
    public class MapControler
    {
        private MapControl MyMap;

        //Keeps track of element that represents the player
        private MapPolygon playerCircle;

        public MapControler(MapControl myMap)
        {
            this.MyMap = myMap;
            Mapinit();
            
            addMapElement("home", new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586266 }, "HomePin.png");
            //drawRoute(new Geopoint(new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586267 }), new Geopoint(new BasicGeoposition { Longitude = 4.0, Latitude = 51.0 }));

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
                    new Uri("ms-appx:///Res/" + building.GetBuidlingType() + building.IsBought()+".png"));
            var BuildingElement = new MapIcon
            {
                Title = building.Name,
                Location = new Geopoint(building.getPosistion()),
                NormalizedAnchorPoint = ancherPoint,
                Image = image
            };

            MyMap.MapElements.Add(BuildingElement);
        }

        public void centerMap(BasicGeoposition center)
        {
            MyMap.Center = new Geopoint(center);
            MyMap.ZoomLevel = 25;
        }

        public void centerMap(Geoposition center)
        {
            MyMap.Center = center.Coordinate.Point;
            MyMap.ZoomLevel = 25;
        }

        public void DrawPlayer(Geoposition newPos, int Radius)
        {
            if(MyMap.MapElements.Contains(playerCircle))
            {
                MyMap.MapElements.Remove(playerCircle);
            }

            
            BasicGeoposition CenterPosition = newPos.Coordinate.Point.Position;
            Color FillColor = Colors.Purple;
            Color StrokeColor = Colors.Red;
            FillColor.A = 80;
            StrokeColor.A = 80;
            playerCircle = new MapPolygon
            {
                StrokeThickness = 2,
                FillColor = FillColor,
                StrokeColor = StrokeColor,
                Path = new Geopath(CalculateCircle(CenterPosition, Radius))
            };
            MyMap.MapElements.Add(playerCircle);
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
    }
}
