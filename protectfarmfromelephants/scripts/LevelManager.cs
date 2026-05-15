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

    }

    public static class Menus
    {
        public const string main_menu = "uid://nedc84ruy5lu";

        public const string settings_menu = "uid://cl335captacg7";

        public const string end_of_day_menu = "uid://tgg87b6yngvj";
        public const string controls_menu = "uid://1v6ruobw5wj0";

    }

    public static class CutScenes
    {
        public const string start_cut_scene = "uid://m8plw4kg2tcy";
        public const string death_scene = "uid://bgu6ommdupkm";

        public const string final_scene = "uid://dkubfth2lqt5x";

        public const string between_levels_animations_scene = "uid://cugj4r5nmh6hj";   
    }

    public static class ItemTextures
    {
        public const string pineapple = "uid://ciktrkurgbo8u";

        public const string mango = "uid://vko05a0ksb6u";

        public const string watermelon = "uid://3apjvumdby6d";

        public const string watering_can = "uid://c3ihe0pog17dn";

        public const string watering_can_upgrade = "uid://djcjvikkirh2c";

        public const string watering_can_puddle_upgrade = "uid://bkjrt7gr332em";
        public const string pineapple_seeds = "uid://us6jgf8ncy05";

        public const string watermelon_seeds = "uid://dmknr85vipb6w";

        public const string mango_seeds = "uid://w6xhmdwunej0";

        public const string fertilizer_boost = "uid://c1spdm8mymlen";

        public const string elephant_poop = "uid://tmqnt4yqae2p";
    }

    public static class UpgradeItemTextures
    {
        public const string fence = "uid://c6okogpqskdwr";

        public const string stone_wall = "uid://sn0y812e1vnp";

        public const string camp_fire = "uid://b177mp24kn5jp";

        public const string noise_maker = "uid://cdvovtpom5knp";

        public const string beehive = "uid://d4l3pw27sxkrf";

        public const string chili = "uid://bf6y072vah72a";

        public const string chili_seeds ="uid://ci81i8pigwsuo";

        public const string sunflower = "uid://ddr1biyrusl2w";

        public const string sunflower_seeds = "uid://5e4lua67dtkc";

        public const string watering_can_upgrade = "uid://djcjvikkirh2c";

        public const string watering_can_puddle_upgrade = "uid://bkjrt7gr332em";

        public const string fertilizer_boost = "uid://c1spdm8mymlen";

        public const string super_fertilizer_boost = "uid://c11bupcfrui1p";

    }

    public static class UITextures
    {
        public const string checkbox_checked = "uid://kmxekxbj7n6f";

        public const string checkbox_unchecked = "uid://bo8pww66vxhi7";
    }

    public static class UpgradeItemDescriptions
    {
        public const string fence = "Wooden fence to protect the farm temporarily";

        public const string stone_wall = "Stonewall to defend the farm";

         public const string camp_fire = "Light source to distract elephants";

        public const string noise_maker = "Stereos to make noise distraction, can be activated only once!";

        public const string beehive = "Bees to scare the elephants away";

        public const string chili = "Seeds to grow chili because smell of chili is not appealing for elephants";

        public const string sunflower = "Elephants don't like sunflowers so grow them to protect farm";

        public const string extra_seeds = "Extra seeds for the level";

        public const string fertilizer = "Use on the plant to boost its growth by one phase";

        public const string super_fertilizer = "Use on the plant to grow it fully but requires extra price of elephant poops gained by feeding fruits to elephants";

        public const string watering_can_upgrade = "Increase capacity of watering can by 5 tiles";

        public const string watering_can_puddle_upgrade = "Water grass tiles to make puddles that may enable elephants to water or fertilize farm tiles instead of destroying.";


    }
}

// Source for singleton: https://csharpindepth.com/articles/singleton 
// Singleton is used because there is need for only one instance of level manager that handles all data related to the levels. 
public partial class LevelManager : Node
{
    public static LevelManager Instance {get; private set; }
    private Dictionary<int, LevelData> levels;

    private List<InventoryItem> player_inventory = new List<InventoryItem>();

    private int current_active_Level;

    private int money_available;

    private int watering_can_level;

    private int watering_can_total_level = 10;

    private int watering_can_total_default_level = 10;

    private bool player_has_failed;
    private bool player_has_watering_can_puddle_upgrade;


    public override void _Ready()
    {
        Instance = this;
    }



public void LoadScene(string uid)
    {
        GetTree().ChangeSceneToFile(uid);
    }

public void QuitGame()
    {
        GetTree().Quit();
    }

public void RestartLevel()
    {
        int current_level = Instance.GetCurrentActiveLevel();
		switch (current_level)
		{
			case 1:
                Instance.LoadScene(Scenes.Levels.level_1);
				break;
			case 2:
                Instance.LoadScene(Scenes.Levels.level_2);
				break;
			case 3:
                Instance.LoadScene(Scenes.Levels.level_3);
				break;
			case 0:
				GD.Print("Active level not found, unable to restart, returning main menu");
                Instance.LoadScene(Scenes.Menus.main_menu);
				break;

		}
    }
public Dictionary<int, LevelData> GetAllLevels()
{
    return levels;
}

//Future development idea: randomize upgrade items per level to make every level different and unique what combinations of upgrades to get!
public void InitializeLevelData()
    {
        levels = new Dictionary<int, LevelData>();

        List<UpgradeItem> level_1_upgrades = new()

        {
            new UpgradeItem("001", "seeds", Scenes.UpgradeItemDescriptions.extra_seeds, "boost", 10, 10, 10, 0, ""),
            new UpgradeItem("002", "fence", Scenes.UpgradeItemDescriptions.fence, "defense", 10, 10, 1, 0, ""),
            new UpgradeItem("003", "stone_wall", Scenes.UpgradeItemDescriptions.stone_wall, "defense", 10, 10, 2, 0, ""),
            new UpgradeItem("004", "noise_maker", Scenes.UpgradeItemDescriptions.noise_maker,"distraction", 2, 2, 4, 0, ""),
            new UpgradeItem("006", "chili", Scenes.UpgradeItemDescriptions.chili, "plant_distraction", 10, 10, 6, 0, ""),
            new UpgradeItem("007", "fertilizer", Scenes.UpgradeItemDescriptions.fertilizer, "boost", 5, 5, 3, 0, ""),
            new UpgradeItem("005", "super_fertilizer", Scenes.UpgradeItemDescriptions.super_fertilizer, "boost", 2, 2, 5, 1, "elephant poop"),
            new UpgradeItem("008", "watering_can_upgrade", Scenes.UpgradeItemDescriptions.watering_can_upgrade, "boost", 2,2, 5, 0, ""),
            new UpgradeItem("009", "watering_can_puddle_upgrade", Scenes.UpgradeItemDescriptions.watering_can_puddle_upgrade, "boost", 1, 1, 10, 0, "")
        };

        List<UpgradeItem> level_2_upgrades = new()

        {
            new UpgradeItem("002", "fence", Scenes.UpgradeItemDescriptions.fence, "defense", 10, 10, 1, 0, ""),
            new UpgradeItem("003", "stone_wall", Scenes.UpgradeItemDescriptions.stone_wall, "defense", 10, 10, 2,0, ""),
            new UpgradeItem("004", "camp_fire", Scenes.UpgradeItemDescriptions.camp_fire, "distraction", 2, 2, 4, 0, ""),
            new UpgradeItem("005", "sun_flower", Scenes.UpgradeItemDescriptions.sunflower, "plant_distraction", 15, 15, 6, 0, ""),
            new UpgradeItem("001", "seeds", Scenes.UpgradeItemDescriptions.extra_seeds, "boost", 10, 10, 10, 0, ""),
            new UpgradeItem("006", "fertilizer", Scenes.UpgradeItemDescriptions.fertilizer, "boost", 2, 2, 20, 2, " elephant poo"),
            new UpgradeItem("008", "watering_can_upgrade", Scenes.UpgradeItemDescriptions.watering_can_upgrade, "boost", 2, 2, 10, 0, "")
        };

        List<UpgradeItem> level_3_upgrades = new()

        {
            new UpgradeItem("001", "seeds", Scenes.UpgradeItemDescriptions.extra_seeds,"boost", 15, 15, 8, 0, ""),
            new UpgradeItem("002", "stone_wall", Scenes.UpgradeItemDescriptions.stone_wall, "defense", 10, 10, 1, 0, ""),
            new UpgradeItem("003", "beehive", Scenes.UpgradeItemDescriptions.beehive, "distraction", 10, 10, 2, 0, ""),
            new UpgradeItem("004", "camp_fire", Scenes.UpgradeItemDescriptions.camp_fire, "distraction", 1, 1, 4, 0, ""),
            new UpgradeItem("005", "noise_maker",Scenes.UpgradeItemDescriptions.noise_maker, "distraction", 1, 1, 4, 0, ""),
            new UpgradeItem("006", "sun_flower", Scenes.UpgradeItemDescriptions.sunflower, "plant_distraction", 15, 15, 8, 0, ""),
            new UpgradeItem("007", "fertilizer", Scenes.UpgradeItemDescriptions.fertilizer, "boost", 3, 3, 25, 3, " elephant poo"),
            new UpgradeItem("008", "watering_can_upgrade", Scenes.UpgradeItemDescriptions.watering_can_upgrade, "boost", 2, 2, 15, 0, "")
        };

        LevelData level_1 = new(1, 20, 0, 5, 28, "pineapple", 15, 30, 30, 1, level_1_upgrades);
        LevelData level_2 = new(2, 40, 0, 5, 48, "watermelon", 5, 15, 25, 2, level_2_upgrades);
        LevelData level_3 = new(3, 60, 0, 4, 64, "mango", 10, 15, 30, 2, level_3_upgrades);
        levels.Add(1, level_1);
        levels.Add(2, level_2);
        levels.Add(3, level_3);
        player_inventory = new List<InventoryItem>();
        money_available = GetStarterMoney();
    }

public int GetCurrentActiveLevel()
    {
        return current_active_Level;
    }
public void SetCurrentActiveLevel(int level)
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

public bool GetPlayerHasFailed()
    {
        return player_has_failed;
    }

public void SetPlayerHasFailed(bool failed)
    {
        player_has_failed = failed;
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
public LevelData GetLevelData(int level_num)
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

public void ResetLevel()
    {
        levels.Clear();
        InitializeLevelData();
        player_has_failed = false;
        watering_can_level = 0;
        watering_can_total_level = watering_can_total_default_level;
        
    }

public void ResetLevelQuota()
    {
        LevelData levelData = GetLevelDataForActiveLevel();
        if(levelData != null)
        {
            levelData.SetCurrentQuota(0);
        }
    }

/**TO-DO: Edit this to transitions between days instead of simple pop up*/
public void MoveToNextDay()
    {
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

            case "watering_can_upgrade":
                texture = Scenes.ItemTextures.watering_can_upgrade;
                break;

            case "watering_can_puddle_upgrade":
                texture = Scenes.ItemTextures.watering_can_puddle_upgrade;
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

            case "chili_seeds":
				texture = Scenes.UpgradeItemTextures.chili_seeds;
				break;	
			case "sunflower":
				texture = Scenes.UpgradeItemTextures.sunflower;
				break;	
            case "sunflower_seeds":
				texture = Scenes.UpgradeItemTextures.sunflower_seeds;
				break;
            case "fertilizer":
                texture = Scenes.UpgradeItemTextures.fertilizer_boost;
                break;
             case "super_fertilizer":
                texture = Scenes.UpgradeItemTextures.super_fertilizer_boost;
                break;
            case "elephant_poop":
                texture = Scenes.ItemTextures.elephant_poop;
                break;
            case "checkbox_checked":
                texture = Scenes.UITextures.checkbox_checked;
                break;
            case "checkbox_unchecked":
                 texture = Scenes.UITextures.checkbox_unchecked;
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

    public int GetWateringCanTotalLevel()
    {
        return watering_can_total_level;
    }

    public void SetWateringCanTotalLevel(int num)
    {
        watering_can_total_level = num;
    }

    public bool GetWateringCanPuddleUpgrade()
    {
        return player_has_watering_can_puddle_upgrade;
    }

	public void SetWateringCanPuddleUpgrade(bool upgraded)
    {
        player_has_watering_can_puddle_upgrade = upgraded;
    }

    public int GetCurrentLevelWatercanUpgradeTotal()
    {
        List<UpgradeItem> upgradeItems = GetLevelDataForActiveLevel().GetLevelUpgradeItems();
        if (upgradeItems != null || upgradeItems.Count > 0)
        {
            UpgradeItem watering_can_size_upgrade = upgradeItems.Find(item => "watering_can_upgrade".Equals(item.GetItemName()));
            if(watering_can_size_upgrade != null)
            {
                return watering_can_size_upgrade.GetTotalForLevel();
            } else
            {
                return 0;
            }
        }
        return GetLevelDataForActiveLevel().GetLevelUpgradeItems().Find(item => "watering_can_upgrade".Equals(item.GetItemName())).GetTotalInStock();
    }

    public string GetCurrentLevelPlantType()
    {
        return GetLevelDataForActiveLevel().GetPlantType();

    }

}










