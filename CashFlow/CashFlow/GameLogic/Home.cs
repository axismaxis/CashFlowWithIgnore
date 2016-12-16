using System;
using Windows.Devices.Geolocation;

namespace CashFlow.GameLogic
{
    public class Home : Building
    {
        public Home(string name) : base(name)
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
            return BuildingType.HomeType;
        }

        public override bool IsBought()
        {
            return true;
        }

        public void ChangeName(String name)
        {
            Name = name;
            //write to saveFile
        }

        public void setPosition(BasicGeoposition position)
        {
            this.Posistion = position;
            //write to saveFIle
        }
            
    
    }
}