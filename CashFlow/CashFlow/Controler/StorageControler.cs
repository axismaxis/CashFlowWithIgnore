using System.Collections.Generic;
using CashFlow.Controler;
using CashFlow.GameLogic;

namespace CashFlow
{
    public class StorageControler
    {
        public List<Building> BuildingList = new List<Building>();


        public void BuildingHandler(Building building)
        {
            if (building.GetBuidlingType().Equals(Building.BuildingType.HouseType))
            {
                building = new House(building.Name);
            }
            else if (building.GetBuidlingType().Equals(Building.BuildingType.MonumentType))
            {
                building = new Monument(building.Name);
            }
            else if (building.GetBuidlingType().Equals(Building.BuildingType.WonderType))
            {
                building = new Wonder(building.Name);
            }
            else
            {
                building  = new Home("Home");
            }

            BuildingList.Add(building);
        }
    }
}