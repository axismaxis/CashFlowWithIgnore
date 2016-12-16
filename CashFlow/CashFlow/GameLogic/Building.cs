using System;
using Windows.Devices.Geolocation;
using Windows.Globalization.NumberFormatting;

namespace CashFlow.GameLogic
{
    public abstract class Building
    {
        public enum BuildingType {HomeType, HouseType, MonumentType, WonderType }

        public double price;
        public double EarningsP_S;
        public string Name;
        public BasicGeoposition Posistion;
        public bool Bought;

        protected Building(string name)
        {
            Name = name;
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