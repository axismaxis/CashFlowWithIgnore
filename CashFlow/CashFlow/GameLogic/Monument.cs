using System;
using Windows.Devices.Geolocation;

namespace CashFlow.GameLogic
{
    public class Monument : Building
    {
        public Monument(string name) : base(name)
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