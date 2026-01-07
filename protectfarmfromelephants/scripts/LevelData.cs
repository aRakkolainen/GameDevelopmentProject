using System.Collections.Generic;
using Godot;
public partial class LevelData : Node
{
    private int Level_Number;

    private int Level_Expected_Quota;

    private int Level_Sold_Quota = 0;

    private int Level_Total_Days;
    private int Level_Seeds_Available;

    private string Level_Plant_Type;

    private int Level_Minimum_Enemies;

    private int Level_Maximum_Enemies;

    private int Level_Starter_Money;

    private int Level_Fruit_Sell_Value;

    private List<UpgradeItem> Level_Upgrades = new List<UpgradeItem>();


    public LevelData(int num, int expected_quota, int current_quota, int days, int number_of_seeds, string type, int min, int max, int starter_money, int fruit_value, List<UpgradeItem> upgrades)
    {
        Level_Number = num;
        Level_Expected_Quota = expected_quota;
        Level_Sold_Quota = current_quota;
        Level_Total_Days = days;
        Level_Seeds_Available = number_of_seeds;
        Level_Plant_Type = type;
        Level_Minimum_Enemies = min;
        Level_Maximum_Enemies = max;
        Level_Starter_Money = starter_money;
        Level_Fruit_Sell_Value = fruit_value;
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

    public void SetLevelFruitSellValue(int value)
    {
        Level_Fruit_Sell_Value = value;
    }


     public int GetLevelFruitSellValue()
    {
        return Level_Fruit_Sell_Value;
    }

    public List<UpgradeItem> GetLevelUpgradeItems()
    {
        return Level_Upgrades;
    }



    
}