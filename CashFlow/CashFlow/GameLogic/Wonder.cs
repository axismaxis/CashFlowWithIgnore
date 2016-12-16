using System;
using Windows.Devices.Geolocation;

namespace CashFlow.GameLogic
{
    public class Wonder :Building
    {
        public Wonder(string name) : base(name)
        {
            Name = name;
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