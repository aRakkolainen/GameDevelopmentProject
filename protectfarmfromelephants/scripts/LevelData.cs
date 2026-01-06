using Godot;
public partial class LevelData : Node
{
    private int Level_Number;

    private int Level_Expected_Quota;

    private int Level_Sold_Quota = 0;

    private int Level_Total_Days;
    private int Level_Seeds_Available;

    private string Level_Plant_Type;

    private int Level_minimum_enemies;

    private int Level_maximum_enemies;


    public LevelData(int num, int expected_quota, int current_quota, int days, int number_of_seeds, string type, int min, int max)
    {
        Level_Number = num;
        Level_Expected_Quota = expected_quota;
        Level_Sold_Quota = current_quota;
        Level_Total_Days = days;
        Level_Seeds_Available = number_of_seeds;
        Level_Plant_Type = type;
        Level_minimum_enemies = min;
        Level_maximum_enemies = max;
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
        return Level_minimum_enemies;
    }

    public void SetLevelMininumEnemies(int min)
    {
        Level_minimum_enemies = min;
    }


    public int GetLevelMaximumEnemies()
    {
        return Level_maximum_enemies;
    }

     public void SetLevelMaximumEnemies(int max)
    {
        Level_maximum_enemies = max;
    }



    
}