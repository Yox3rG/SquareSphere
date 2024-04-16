using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class DataCollection
{
    public ProfileData profile;
    public StatisticsData stats;
    public ShopData shop;
    public SettingsData settings;
    public DailyRewardData dailyReward;

    private string fileLocation = "data";
    private string fileNameData = "data";
    private readonly string fileExtension = ".json";


    public DataCollection()
    {
        profile = new ProfileData();
        stats = new StatisticsData();
        shop = new ShopData();
        settings = new SettingsData();
        dailyReward = new DailyRewardData();
    }

    #region FileNameHandle
    public bool SaveData()
    {
        return SaveLoadHandlerJSON<DataCollection>.Save(this, GetDataFile());
    }

    public bool LoadData()
    {
        
        DataCollection temp = new DataCollection();
        bool dataOK = SaveLoadHandlerJSON<DataCollection>.Load(GetDataFile(), out temp);

        if (dataOK)
        {
            profile = temp.profile;
            stats = temp.stats;
            shop = temp.shop;
            settings = temp.settings;
            dailyReward = temp.dailyReward;

            return true;
        }

        return false;
    }

    private string GetDataFile()
    {
        return GetCurrentFile(fileNameData);
    }

    private string GetCurrentFile(string filename)
    {
        return Path.Combine(fileLocation, filename + fileExtension);
    }


    public string GetFileLocation()
    {
        return fileLocation;
    }
    #endregion
}
