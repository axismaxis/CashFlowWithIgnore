using System;
using System.Diagnostics;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;

namespace CashFlow.Controler
{
    public class MapControler
    {
        private MapControl MyMap;

        public MapControler(MapControl myMap)
        {
            this.MyMap = myMap;
            Mapinit();
            centerMap(new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586267 });
            addMapElement("home", new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586266 }, "HomePin.png");
            drawRoute(new Geopoint(new BasicGeoposition { Longitude = 4.780172, Latitude = 51.586267 }), new Geopoint(new BasicGeoposition { Longitude = 4.0, Latitude = 51.0 }));

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
    }
}
