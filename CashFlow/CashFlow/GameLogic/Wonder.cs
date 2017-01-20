using System;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;

namespace CashFlow.GameLogic
{
    [DataContract]
    public class Wonder :Building
    {


        public Wonder(string name, double price, double earnings, BasicGeoposition position, bool isBought) : base(name)
        {
            base.price = price;
            EarningsP_S = earnings;
            Posistion = position;
            Bought = isBought;
            Name = name;
            type = BuildingType.WonderType;
        }

        public Wonder(string name, double price, double earnings, BasicGeoposition position, bool isBought, DateTime boughtTime) : base(name)
        {
            base.price = price;
            EarningsP_S = earnings;
            Posistion = position;
            Bought = isBought;
            Name = name;
            type = BuildingType.WonderType;
            timeLastCollected = boughtTime;
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
            return BuildingType.WonderType;
        }

        public override bool IsBought()
        {
            return Bought;
        }

        public void setPosition(BasicGeoposition position)
        {
            this.Posistion = position;
            //write to saveFile
        }

        public void setBought(bool isBougt)
        {
            this.Bought = isBougt;
            //write to saveFile
        }
    }
}