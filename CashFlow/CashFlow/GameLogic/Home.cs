using System;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;

namespace CashFlow.GameLogic
{
    [DataContract]
    public class Home : Building
    {


        public Home(string name, double price, double earnings, BasicGeoposition position, bool isBought) : base(name)
        {
            base.price = price;
            EarningsP_S = earnings;
            Posistion = position;
            Bought = isBought;
            Name = name;
            type = BuildingType.HomeType;
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