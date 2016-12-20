using System;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;

namespace CashFlow.GameLogic
{
    [DataContract]
    public class Monument : Building
    {
   

        public Monument(string name, double price, double earnings, BasicGeoposition position, bool isBought) : base(name)
        {
            base.price = price;
            EarningsP_S = earnings;
            Posistion = position;
            Bought = isBought;
            Name = name;
            type = BuildingType.MonumentType;
        }

        public override double getPrice()
        {
            throw new System.NotImplementedException();
        }

        public override double getEarningsP_S()
        {
            throw new System.NotImplementedException();
        }

        public override BasicGeoposition getPosistion()
        {
            return Posistion;
        }

        public override Enum GetBuidlingType()
        {
            return BuildingType.MonumentType;
        }

        public override bool IsBought()
        {
            return Bought;
        }

        public void setPosition(BasicGeoposition Position)
        {
            this.Posistion = Position;
            // write to save file
        }

        public void setBought(bool isBought)
        {
            Bought = isBought;
            // write to save file
        }
    }
}