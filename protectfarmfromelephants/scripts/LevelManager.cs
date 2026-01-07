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

    public static class ItemTextures
    {
        public const string pineapple = "uid://ciktrkurgbo8u";

        public const string mango = "uid://vko05a0ksb6u";

        public const string watermelon = "uid://3apjvumdby6d";

        public const string watering_can = "uid://c3ihe0pog17dn";

        public const string pineapple_seeds = "uid://us6jgf8ncy05";

        public const string watermelon_seeds = "uid://dmknr85vipb6w";

        public const string mango_seeds = "uid://w6xhmdwunej0";

    }
}

// Source for singleton: https://csharpindepth.com/articles/singleton 
// Singleton is used because there is need for only one instance of level manager. 
public partial class LevelManager : Node
{
    public static LevelManager Instance {get; private set; }
    private Dictionary<string, LevelData> levels;

    private List<InventoryItem> player_inventory = new List<InventoryItem>();

    private string current_active_Level;

    public override void _Ready()
    {
        Instance = this;
    }

public void LoadLevel(string uid)
    {
        GetTree().ChangeSceneToFile(uid);
    }

public void QuitGame()
    {
        GetTree().Quit();
    }
public Dictionary<string, LevelData> GetAllLevels()
{
    return levels;
}

public void InitializeLevelData()
    {
        levels = new Dictionary<string, LevelData>();
        LevelData level_1 = new(1, 20, 0, 5, 28, "pineapple", 2, 5);
        LevelData level_2 = new(2, 40, 0, 5, 48, "watermelon", 5, 10);
        LevelData level_3 = new(3, 60, 0, 4, 64, "mango", 10, 15);
        levels.Add("level_1", level_1);
        levels.Add("level_2", level_2);
        levels.Add("level_3", level_3);
        player_inventory = new List<InventoryItem>();
    }

public string GetCurrentActiveLevel()
    {
        return current_active_Level;
    }
public void SetCurrentActiveLevel(string level)
    {
        current_active_Level = level;
    }
public List<InventoryItem> GetPlayerInventory()
    {
        return player_inventory;
    }

public void SetPlayerInventory(List<InventoryItem> items)
    {
        player_inventory = items;
    }

public LevelData GetLevelDataForActiveLevel()
    {
        LevelData level = levels.GetValueOrDefault(current_active_Level);
        if(level == null)
        {
            return null;
        }
        return level;
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

public bool UpdateLevelQuota(int sold_fruits)
    {
        LevelData levelData = GetLevelDataForActiveLevel();
        if(levelData != null)
        {
            int current = levelData.GetCurrentQuota();
            levelData.SetCurrentQuota(current+sold_fruits);
            return true;
        }
        return false;
    }

public void ResetLevelQuota()
    {
        LevelData levelData = GetLevelDataForActiveLevel();
        if(levelData != null)
        {
            levelData.SetCurrentQuota(0);
        }
    }

}






