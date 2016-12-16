using System;

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

        public void ChangeName(String name)
        {
            Name = name;
        }
    }
}