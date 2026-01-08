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

    public static class UpgradeItemTextures
    {
        public const string fence = "uid://c6okogpqskdwr";

        public const string stone_wall = "uid://sn0y812e1vnp";

        public const string camp_fire = "uid://b177mp24kn5jp";

        public const string noise_maker = "uid://cdvovtpom5knp";

        public const string beehive = "uid://d4l3pw27sxkrf";

        public const string chili = "uid://bf6y072vah72a";

        public const string sunflower = "uid://ddr1biyrusl2w";

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

    private int money_available;

    private int watering_can_level;

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

        List<UpgradeItem> level_1_upgrades = new()

        {
            new UpgradeItem("002", "fence", "defense", 10, 1),
            new UpgradeItem("003", "stone_wall", "defense", 10, 2),
            new UpgradeItem("004", "noise_maker", "distraction", 2, 4),
            new UpgradeItem("005", "camp_fire", "distraction", 1, 4),
            new UpgradeItem("006", "chili", "distraction", 10, 6),
            new UpgradeItem("001", "seeds", "boost", 10, 10),
        };

        List<UpgradeItem> level_2_upgrades = new()

        {
            new UpgradeItem("002", "fence", "defense", 10, 1),
            new UpgradeItem("003", "stone_wall", "defense", 10, 2),
            new UpgradeItem("004", "camp_fire", "distraction", 2, 4),
            new UpgradeItem("005", "sun_flower", "distraction", 15, 6),
            new UpgradeItem("001", "seeds", "boost", 10, 10),
        };

        List<UpgradeItem> level_3_upgrades = new()

        {
            new UpgradeItem("002", "stone_wall", "defense", 10, 1),
            new UpgradeItem("003", "beehive", "distraction", 10, 2),
            new UpgradeItem("004", "camp_fire", "distraction", 1, 4),
            new UpgradeItem("005", "noise_maker", "distraction", 1, 4),
            new UpgradeItem("006", "sun_flower", "distraction", 15, 8),
            new UpgradeItem("001", "seeds", "boost", 15, 8),
        };

        LevelData level_1 = new(1, 20, 0, 5, 28, "pineapple", 2, 5, 30, 1, level_1_upgrades);
        LevelData level_2 = new(2, 40, 0, 5, 48, "watermelon", 5, 10, 25, 2, level_2_upgrades);
        LevelData level_3 = new(3, 60, 0, 4, 64, "mango", 10, 15, 20, 2, level_3_upgrades);
        levels.Add("level_1", level_1);
        levels.Add("level_2", level_2);
        levels.Add("level_3", level_3);
        player_inventory = new List<InventoryItem>();
        money_available = GetStarterMoney();
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
public int GetStarterMoney()
{
     LevelData levelData = GetLevelDataForActiveLevel();
        if(levelData == null)
        {
            return 0;
        }
            return levelData.GetLevelStarterMoney();
}

public int GetMoneyAvailable()
{
     return money_available;
}

public void AddToTotalMoney(int amount)
    {
        money_available += amount;
    }

public void MinusFromTotalMoney(int amount)
    {
        money_available -= amount;
    }

public string GetTextureByItemName(string item_type)
	{
		string texture = "";
		switch (item_type)
		{
            case "watering_can":
				texture = Scenes.ItemTextures.watering_can;
				break;
            case "pineapple_seeds":
				texture = Scenes.ItemTextures.pineapple_seeds;
				break;
			case "watermelon_seeds":
				texture = Scenes.ItemTextures.watermelon_seeds;
				break;
			case "mango_seeds":
				texture = Scenes.ItemTextures.mango_seeds;
				break;
            case "pineapple":
				texture = Scenes.ItemTextures.pineapple;
				break;
			case "watermelon":
				texture = Scenes.ItemTextures.watermelon;
				break;
			case "mango":
				texture = Scenes.ItemTextures.mango;
				break;
			case "fence":
				texture = Scenes.UpgradeItemTextures.fence;
				break;
			case"stone_wall":
				texture = Scenes.UpgradeItemTextures.stone_wall;
				break;
			case "camp_fire":
				texture = Scenes.UpgradeItemTextures.camp_fire;
				break;
			case "noise_maker":
				texture = Scenes.UpgradeItemTextures.noise_maker;
				break;
			case "beehive":
				texture = Scenes.UpgradeItemTextures.beehive;
				break;
			case "chili":
				texture = Scenes.UpgradeItemTextures.chili;
				break;	
			case "sunflower":
				texture = Scenes.UpgradeItemTextures.sunflower;
				break;	

			case "seeds":
				LevelData level = LevelManager.Instance.GetLevelDataForActiveLevel();
				if (level == null || (level != null && level.GetPlantType() == null))
				{
					break;
				} else
				{
					
				string plant_type = LevelManager.Instance.GetLevelDataForActiveLevel().GetPlantType();
				if (plant_type.Equals("pineapple"))
				{
					texture = Scenes.ItemTextures.pineapple_seeds;
				} else if (plant_type.Equals("watermelon"))
				{
					texture = Scenes.ItemTextures.watermelon_seeds;
				} else if (plant_type.Equals("mango"))
				{
					texture = Scenes.ItemTextures.mango_seeds;
				}
				}
				break;

		}
		return texture;
	}

    public int GetWateringCanLevel()
    {
        return watering_can_level;
    }

    public void SetWateringCanLevel(int num)
    {
        watering_can_level = num;
    }

}










