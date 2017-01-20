using System.Runtime.Serialization;
using Windows.Devices.Geolocation;

namespace CashFlow.Acount
{
    [DataContract]
    public class AccountInfo
    {
        [DataMember]
        private string _Name;
        [DataMember]
        private double _earnings;
        //[DataMember]
        //private Geopoint _HomeLocation;
        [DataMember]
        private double longitude;
        [DataMember]
        private double latitude;

        public static string content;

        public AccountInfo(string name, double earnings, double longa, double lati)
        {
            _Name = name;
            _earnings = earnings;
            //_HomeLocation = homeLocation

            longitude = longa;
            latitude = lati;

        }

        public string GetName()
        {
            return _Name;
        }

        public double GetEarnings()
        {
            return _earnings;
        }

        public void setEarnings(double earnings)
        {
            _earnings = earnings;
        }
        public double getLongitude()
        {
            return longitude;
        }

        public double getLatitude()
        {
            return latitude;
        }
        public override string ToString()
        {
            return "\nPlayer name: " + _Name + "\nMoney: " + _earnings + "\nHome Location: " + longitude + ", Latitude: " + latitude;
        }
    }
}