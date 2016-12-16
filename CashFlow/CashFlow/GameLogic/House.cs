using System;
using Windows.Devices.Geolocation;
using CashFlow.GameLogic;

namespace CashFlow
{
    public class House : Building
    {
        public House(string name) : base(name)
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
           return  BuildingType.HouseType;
        }

        public override bool IsBought()
        {
            return Bought;
        }

        public void setPosistion(BasicGeoposition position)
        {
            this.Posistion = position;
            //write to saveFIle
        }

        public void setBought(bool isBought)
        {
            Bought = isBought;
            //write to saveFIle
        }
    }
}