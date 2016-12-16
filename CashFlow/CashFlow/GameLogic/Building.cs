using Windows.Globalization.NumberFormatting;

namespace CashFlow.GameLogic
{
    public abstract class Building
    {
        public double price;
        public double EarningsP_S;
        public string Name;

        protected Building(string name)
        {
            Name = name;
        }

        public abstract double getPrice();

        public abstract double getEarningsP_S();

        public string getName()
        {
            return Name;
        }

    }
}