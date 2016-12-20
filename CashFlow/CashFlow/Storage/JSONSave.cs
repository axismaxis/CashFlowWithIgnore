using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using CashFlow.Acount;
using CashFlow.GameLogic;
using Newtonsoft.Json;

namespace CashFlow.Storage
{


    public static class JsonSave
    {
        private const string PersonalDataFileName = "saveData.json";
        private const string BuildingDataFileName = "buildingData.json";

        public static async Task<bool> FileExist()
        {
            try
            {
                var folders = ApplicationData.Current.LocalFolder;
                var file = await folders.GetFileAsync(PersonalDataFileName);
                var buildingFile = await folders.GetFileAsync(BuildingDataFileName);
                if (file.Path != null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return true;
            }
        }

        public static async void SavePersonalDataToJson(AccountInfo accountInfo)
        {
            // Serialize our Product class into a string
            // Changed to serialze the List

            // Get the app data folder and create or replace the file we are storing the JSON in.
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile textFile = await localFolder.CreateFileAsync(PersonalDataFileName, CreationCollisionOption.ReplaceExisting);

            // Open the file...
            string jsonContents = JsonConvert.SerializeObject(accountInfo);
            using (IRandomAccessStream mysteream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter textWriter = new DataWriter(mysteream))
                {
                    textWriter.WriteString(jsonContents);
                    await textWriter.StoreAsync();
                }

            }
        }

        public static async Task<AccountInfo> LoadPersonalDataFromJson()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile textFile = await localFolder.GetFileAsync(BuildingDataFileName);

            using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
            {
                // Read text stream     
                using (DataReader textReader = new DataReader(textStream))
                {
                    //get size                       
                    uint textLength = (uint)textStream.Size;
                    await textReader.LoadAsync(textLength);
                    // read it                    
                    string jsonContents = textReader.ReadString(textLength);
                    // deserialize back to our product!  
                    AccountInfo accountInfoFromJsonFile = JsonConvert.DeserializeObject<AccountInfo>(jsonContents);
                    // and show it                     
                    return accountInfoFromJsonFile;
                }
            }
        }

        public static async void saveBuildingdata(List<Building> list)
        {
            // Serialize our Product class into a string
            // Changed to serialze the List

            // Get the app data folder and create or replace the file we are storing the JSON in.
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile textFile = await localFolder.CreateFileAsync(BuildingDataFileName, CreationCollisionOption.ReplaceExisting);

            // Open the file...

            string jsonContents = JsonConvert.SerializeObject(BuildingToBuildingData(list));
            using (IRandomAccessStream mysteream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter textWriter = new DataWriter(mysteream))
                {
                    textWriter.WriteString(jsonContents);
                    await textWriter.StoreAsync();
                }

            }
        }

        public static async Task<List<Building>> getBuildingList()
        {

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile textFile = await localFolder.GetFileAsync(BuildingDataFileName);

            using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
            {
                // Read text stream     
                using (DataReader textReader = new DataReader(textStream))
                {
                    //get size                       
                    uint textLength = (uint)textStream.Size;
                    await textReader.LoadAsync(textLength);
                    // read it                    
                    string jsonContents = textReader.ReadString(textLength);
                    // deserialize back to our product!  
                    // List<Building> BuildingList = JsonConvert.DeserializeObject<List<Building>>(jsonContents);


                    List<BuildingData> BuildingList = JsonConvert.DeserializeObject<List<BuildingData>>(jsonContents);

                    List<Building> ConvertedBuildingList = BuildingDataToBuilding(BuildingList);
                    // and show it    

                    return ConvertedBuildingList;

                }
            }
        }

        public static List<BuildingData> BuildingToBuildingData(List<Building> list)
        {
            List<BuildingData> newList = new List<BuildingData>();

            foreach (Building building in list)
            {
                newList.Add(new BuildingData
                (
                    building.Name,
                    building.price,
                    building.EarningsP_S,
                    building.Posistion,
                    building.Bought,
                    building.type
                ));
            }
            return newList;
        }



        public static List<Building> BuildingDataToBuilding(List<BuildingData> list)
        {
            List<Building> buildingList = new List<Building>();

            foreach (BuildingData building in list)
            {
                switch (building.Type)
                {
                    case Building.BuildingType.HomeType:
                        buildingList.Add(new Home(
                        building.name,
                        building.Price,
                        building.Earnings,
                        building.Position,
                        building.Bought
                            ));
                        break;

                    case Building.BuildingType.HouseType:
                        buildingList.Add(new House(
                        building.name,
                        building.Price,
                        building.Earnings,
                        building.Position,
                        building.Bought
                            ));
                        break;

                    case Building.BuildingType.MonumentType:
                        buildingList.Add(new Monument(
                        building.name,
                        building.Price,
                        building.Earnings,
                        building.Position,
                        building.Bought
                            ));
                        break;


                    case Building.BuildingType.WonderType:
                        buildingList.Add(new Wonder(
                        building.name,
                        building.Price,
                        building.Earnings,
                        building.Position,
                        building.Bought
                            ));
                        break;
                }
            }
            return buildingList;
        }

    }

    public class BuildingData
    {

        public string name { get; set; }
        public double Price { get; set; }
        public double Earnings { get; set; }
        public BasicGeoposition Position { get; set; }
        public bool Bought { get; set; }
        public Building.BuildingType Type { get; set; }

        public BuildingData(string name, double Price, double Earnings, BasicGeoposition Position, bool Bought, Building.BuildingType Type)
        {
            this.name = name;
            this.Price = Price;
            this.Earnings = Earnings;
            this.Position = Position;
            this.Bought = Bought;
            this.Type = Type;
        }

    }
}