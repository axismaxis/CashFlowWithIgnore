using System;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Globalization.NumberFormatting;

namespace CashFlow.GameLogic
{
    [DataContract]
    public abstract class Building
    {
        public enum BuildingType {HomeType, HouseType, MonumentType, WonderType }

        [DataMember]
        public double price;
        [DataMember]
        public double EarningsP_S;
        [DataMember]
        public string Name;
        [DataMember]
        public BasicGeoposition Posistion;
        [DataMember]
        public bool Bought;
        [DataMember]
        public BuildingType type;
        [DataMember]
        public DateTime timeLastCollected;


        protected Building(string name)
        {
            Name = name;
            timeLastCollected = DateTime.Now;
        }
        public abstract double getPrice();

        public abstract double getEarningsP_S();

        public abstract BasicGeoposition getPosistion();

        public abstract Enum GetBuidlingType();

        public abstract bool IsBought();

        public string getName()
        {
            return Name;
        }

    }
}