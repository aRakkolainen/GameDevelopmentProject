using Godot;
public partial class LevelData : Node
{
    private int Level_Number;

    private int Level_Expected_Quota;

    private int Level_Sold_Quota = 0;

    private int Level_Total_Days;

    private string Level_Plant_Type;

    public LevelData(int num, int expected_quota, int current_quota, int days, string type)
    {
        Level_Number = num;
        Level_Expected_Quota = expected_quota;
        Level_Sold_Quota = current_quota;
        Level_Total_Days = days;
        Level_Plant_Type = type;
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

    public string GetPlantType()
    {
        return Level_Plant_Type;
    }

    public void SetLevelPlantType(string plant)
    {
        Level_Plant_Type = plant;
    }

    
}