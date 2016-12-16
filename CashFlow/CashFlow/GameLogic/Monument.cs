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
    }
}