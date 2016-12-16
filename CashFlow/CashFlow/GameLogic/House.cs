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
    }
}