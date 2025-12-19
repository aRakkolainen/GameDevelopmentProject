using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Godot;

//Source for this code is this tutorial: https://www.youtube.com/watch?v=bIvxPawSup0
public static class Scenes
{
    public static class Levels
    {
        public const string level_1 = "uid://da2bvcg75chtw";
        public const string level_2 = "uid://dmy86o0h4u4se";

        public const string level_3 = "uid://c581mkch3llep";

        public const string death_scene = "uid://bgu6ommdupkm";

        public const string final_scene = "uid://dkubfth2lqt5x";
    }

    public static class Menus
    {
        public const string main_menu = "uid://nedc84ruy5lu";
    }
}

// Source for singleton: https://csharpindepth.com/articles/singleton 
// Singleton is used because there is need for only one instance of level manager. 
public partial class LevelManager : Node
{
    public static LevelManager Instance {get; private set; }
    private Dictionary<string, LevelData> levels;

    private string current_active_Level;

    public override void _Ready()
    {
        Instance = this;
    }

public void LoadLevel(string uid)
    {
        GetTree().ChangeSceneToFile(uid);
    }
public Dictionary<string, LevelData> GetAllLevels()
{
    return levels;
}

public void InitializeLevelData()
    {
        levels = new Dictionary<string, LevelData>();
        LevelData level_1 = new(1, 20, 0, 5, "pineapple");
        LevelData level_2 = new(2, 40, 0, 5, "watermelon");
        LevelData level_3 = new(3, 60, 0, 4, "mango");
        levels.Add("level_1", level_1);
        levels.Add("level_2", level_2);
        levels.Add("level_3", level_3);
    }

public string GetCurrentActiveLevel()
    {
        return current_active_Level;
    }
public void SetCurrentActiveLevel(string level)
    {
        current_active_Level = level;
    }



public LevelData GetLevelData(string level_num)
    {
        LevelData level = levels.GetValueOrDefault(level_num);
        if(level == null)
        {
            return null;
        }
        return level;
    }

public void UpdateLevelQuota(string level, int sold_fruits)
    {
        LevelData levelData = levels.GetValueOrDefault(level);
        if(level != null)
        {
            int current = levelData.GetCurrentQuota();
            levelData.SetCurrentQuota(current+sold_fruits);
        }
    }

}






