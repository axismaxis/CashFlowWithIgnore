using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace CashFlow.GPS
{
    /// <summary>
    /// Wraps geolocator and only provides functions necesary for route tracking
    /// </summary>
    public class GPSHandler
    {
        ///<summary> 
        ///Private Geolocator for getting location updates on the user 
        ///</summary>
        private Geolocator myLocator;
        /// <summary>
        /// People can subscribe with this delegate type
        /// </summary>
        /// <param name="newPosition">New GPS position received from Geolocator</param>
        public delegate void positionChangedDelegate(Geoposition newPosition);
        /// <summary>
        /// Notifies subscribers when new position is received from Geolocator
        /// </summary>
        private event positionChangedDelegate positionChangedEvent;
        private int listenersSubscribed = 0;

        //geofence setup variables
        public delegate void OnGeofenceTriggered();
        public event OnGeofenceTriggered GeofenceEnteredEventTriggered;
        public event OnGeofenceTriggered GeofenceExitedEventTriggered;

        /// <summary>
        /// Buffer with latest received location update
        /// </summary>
        public Geoposition lastPositionReceived { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public GPSHandler()
        {
            Debug.WriteLine("GPS has been instantiated.");
        }

        public void SubscribeToLocation(positionChangedDelegate newMethod)
        {
            positionChangedEvent += newMethod;
            listenersSubscribed++;
        }

        public async Task<bool> RequestUserAccesAsync()
        {
            GeolocationAccessStatus LocationEnabled = await Geolocator.RequestAccessAsync();
            if (LocationEnabled == GeolocationAccessStatus.Allowed)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Always run this before using the GPSHandler
        /// </summary>
        /// <param name="movementThreshold">How many meters you move before it fires a positionchanged event</param>
        public void InitGPSHandler(int movementThreshold)
        {
            myLocator = new Geolocator();
            myLocator.PositionChanged += GpsPositionChanged;
            myLocator.DesiredAccuracy = PositionAccuracy.High;
            myLocator.MovementThreshold = movementThreshold;
        }

        /// <summary>
        /// Asks the current location to the Geolocator
        /// </summary>
        /// <returns>Returns the new currect location from the Geolocator</returns>
        public async Task<Geoposition> getCurrentLocation()
        {
            return await myLocator.GetGeopositionAsync();
        }

        /// <summary>
        /// Retrieves last location automaticly received by the geolocator
        /// </summary>
        /// <returns>Returns the last location automaticly received by the geolocator</returns>
        public Geoposition getLastLocation()
        {
            return lastPositionReceived;
        }
        
        /// <summary>
        /// Gets called when Geolocator sends a new position
        /// </summary>
        private void GpsPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            lastPositionReceived = args.Position;
            positionChangedEvent?.Invoke(args.Position);
        }
    }
}
