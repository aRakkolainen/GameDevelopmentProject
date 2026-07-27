using System.Collections.Generic;
using Godot;
public partial class LevelData : Node
{
    private int Level_Number;

    private int Level_Expected_Quota;

    private int Level_Sold_Quota = 0;

    private int Level_Total_Days;
    private int Level_Day_When_Quota_Filled;

    private int Level_Seeds_Available;

    private string Level_Plant_Type;

    private int Level_Minimum_Enemies;

    private int Level_Maximum_Enemies;

    private int Level_Starter_Money;

    private int Level_Current_Money;

    private int Level_Used_Upgrade_Items_Count;

    private int Level_Fruit_Sell_Value;

    private List<UpgradeItem> Level_Upgrades = new List<UpgradeItem>();

    private string Level_Distraction_Plant_Type;
    private int Level_Distraction_Plant_Sell_Value;

    public LevelData(int num, int expected_quota, int current_quota, int days, int day_number, int number_of_seeds, string type, int min, int max, int starter_money, int items_count, int fruit_value, string distraction_plant, int plant_value, List<UpgradeItem> upgrades)
    {
        Level_Number = num;
        Level_Expected_Quota = expected_quota;
        Level_Sold_Quota = current_quota;
        Level_Total_Days = days;
        Level_Day_When_Quota_Filled = day_number;
        Level_Seeds_Available = number_of_seeds;
        Level_Plant_Type = type;
        Level_Minimum_Enemies = min;
        Level_Maximum_Enemies = max;
        Level_Starter_Money = starter_money;
        Level_Current_Money = starter_money;
        Level_Used_Upgrade_Items_Count = items_count;
        Level_Fruit_Sell_Value = fruit_value;
        Level_Distraction_Plant_Type = distraction_plant;
        Level_Distraction_Plant_Sell_Value = plant_value;
        Level_Upgrades = upgrades;
    }   

    public int GetLevelNumber()
    {
        return Level_Number;
    }

    public void SetLevelNumber(int num)
    {
        Level_Number = num;
    }

    public int GetExpectedQuota()
    {
        return Level_Expected_Quota;
    }

    public void SetExpectedQuota(int expect)
    {
        Level_Expected_Quota = expect;
    }

    public int GetCurrentQuota()
    {
        return Level_Sold_Quota;
    }

    public void SetCurrentQuota(int quota)
    {
        Level_Sold_Quota = quota;   
    }

    public int GetLevelTotalDays()
    {
        return Level_Total_Days;
    }

    public void SetLevelTotalDays(int days)
    {
        Level_Total_Days = days;
    }

    public int GetLevelDayWhenQuotaFilled()
    {
        return Level_Day_When_Quota_Filled;
    }

    public void SetLevelDayWhenQuotaFilled(int day)
    {
        Level_Day_When_Quota_Filled = day;
    }

    public int GetLevelAvailableSeeds()
    {
        return Level_Seeds_Available;
    }

    public void SetLevelAvailableSeeds(int seeds_num)
    {
        Level_Seeds_Available = seeds_num;
    }



    public string GetPlantType()
    {
        return Level_Plant_Type;
    }

    public void SetLevelPlantType(string plant)
    {
        Level_Plant_Type = plant;
    }

    public int GetLevelMininumEnemies()
    {
        return Level_Minimum_Enemies;
    }

    public void SetLevelMininumEnemies(int min)
    {
        Level_Minimum_Enemies = min;
    }


    public int GetLevelMaximumEnemies()
    {
        return Level_Maximum_Enemies;
    }

     public void SetLevelMaximumEnemies(int max)
    {
        Level_Maximum_Enemies = max;
    }

    public int GetLevelStarterMoney()
    {
        return Level_Starter_Money;
    }

    public void SetLevelStarterMoney(int money)
    {
        Level_Starter_Money = money;
    }

     public int GetLevelCurrentMoney()
    {
        return Level_Current_Money;
    }

    public void SetLevelCurrentMoney(int money)
    {
        Level_Current_Money = money;
    }

    public int GetLevelUsedUpgradeItemsCount()
    {
        return Level_Used_Upgrade_Items_Count;
    }

    public void SetLevelUsedUpgradeItemsCount(int count)
    {
        Level_Used_Upgrade_Items_Count = count;
    }

    public void IncreaseLevelUsedUpgradeItemsCountByOne()
    {
        Level_Used_Upgrade_Items_Count++;
    }

    public void DecreaseLevelUsedUpgradeItemsCountByOne()
    {
        Level_Used_Upgrade_Items_Count--;
    }

    public void SetLevelFruitSellValue(int value)
    {
        Level_Fruit_Sell_Value = value;
    }


     public int GetLevelFruitSellValue()
    {
        return Level_Fruit_Sell_Value;
    }

     public void SetLevelDistractionPlantSellValue(int value)
    {
        Level_Distraction_Plant_Sell_Value = value;
    }


     public int GetLevelDistractionPlantSellValue()
    {
        return Level_Distraction_Plant_Sell_Value;
    }

     public void SetLevelDistractionPlantType(string type)
    {
        Level_Distraction_Plant_Type = type;
    }


     public string GetLevelDistractionPlantType()
    {
        return Level_Distraction_Plant_Type;
    }

    public List<UpgradeItem> GetLevelUpgradeItems()
    {
        return Level_Upgrades;
    }

    public void SetLevelUpgradeItems(List<UpgradeItem> items)
    {
        Level_Upgrades = items;
    }



    
}