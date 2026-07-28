using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Godot;
using Microsoft.VisualBasic;
using ProtectFarm;

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

        public const string credits_scene = "uid://b31c4euhi2qco";

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
        public const string fence = "Wooden fence to protect the farm";

        public const string stone_wall = "Stonewall to defend the farm";

         public const string camp_fire = "Light source to distract elephants";

        public const string noise_maker = "Stereos to make noise distraction, can be activated only once!";

        public const string beehive = "Bees to scare the elephants away";

        public const string chili = "Smell and taste of chili is unpleasant for elephants \n so plant next to your fruits either on farm or grass or sell for extra money";

        public const string sunflower = "Smell of sunflowers is unpleasant for elephants \n so plant next to your fruits either on the farm or grass or sell for extra money";

        public const string extra_seeds = "10 extra seeds for the level";

        public const string fertilizer = "Use on the plant to boost its growth by one phase";

        public const string super_fertilizer = "Use on the plant to grow it fully but requires extra price of elephant poops gained by feeding fruits to elephants";

        public const string watering_can_upgrade = "Increase capacity of watering can by 5 tiles";

        public const string watering_can_puddle_upgrade = "Create water puddle on grass tiles, \n and enable elephants to water tiles. After 3 touches by elephant, \n it becomes mud and it might fertilize random plant when elephant touches it.";

    }

    public static class Constants
    {
        public  const string pineapple = "pineapple";

        public  const string watermelon = "watermelon";

        public  const string mango = "mango";

        public const string chili = "chili";

        public const string sunflower = "sunflower"; 
        public const string distraction = "distraction";

        public const string distraction_plant = "distraction_plant";


        public const string defense = "defense";

    }
}

// Source for singleton: https://csharpindepth.com/articles/singleton 
// Singleton is used because there is need for only one instance of level manager that handles all data related to the levels. 
public partial class LevelManager : Node
{
    public static LevelManager Instance {get; private set; }
    public bool SkipStartCutscene { get; private set; }


    private Dictionary<int, LevelData> levels;

    private List<InventoryItem> player_inventory = new List<InventoryItem>();

    private int current_active_Level;

    private int current_day;

    private int money_available;

    private int watering_can_level;

    private int watering_can_total_level = 10;

    private int watering_can_total_default_level = 10;

    private bool player_has_failed;
    private bool player_has_watering_can_puddle_upgrade;

    private bool game_started = false;
    private static List<UpgradeItem> all_upgrade_items;

    private int default_quota = 20;

    private int default_extra_seeds = 4;

    private int default_fruit_value = 3;

    private int default_distraction_plant_value = 4;

    private int default_minimum_elephants = 10; 

    private int default_maximum_elephants = 20;

    private int default_starter_money = 30;

    private int default_days = 5;
    private int days_left;
    private bool restart_pressed;

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
        SetRestartPressed(true);
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
        InitializeAllAvailableUpgradeItems();

        List<UpgradeItem> level_1_upgrades = RandomizeUpgradeItemsForLevel(20);
        List<UpgradeItem> level_2_upgrades = RandomizeUpgradeItemsForLevel(40);
        List<UpgradeItem> level_3_upgrades = RandomizeUpgradeItemsForLevel(60);
        LevelData level_1 = new(1, default_quota*1, 0, default_days, 0, default_quota + default_extra_seeds * 2, Scenes.Constants.pineapple, default_minimum_elephants, default_maximum_elephants, default_starter_money, 0, default_fruit_value, level_1_upgrades.Find(item => item.GetItemType().Equals("distraction_plant")).GetItemName(), default_distraction_plant_value, level_1_upgrades);
        LevelData level_2 = new(2, default_quota*2, 0, default_days, 0, default_quota * 2 + default_extra_seeds * 2, Scenes.Constants.watermelon, default_minimum_elephants*2, default_maximum_elephants+10, default_starter_money-5, 0, default_fruit_value, level_2_upgrades.Find(item => item.GetItemType().Equals("distraction_plant")).GetItemName(), default_distraction_plant_value, level_2_upgrades);
        LevelData level_3 = new(3, default_quota*3, 0, default_days + 1, 0, default_quota * 3 + default_extra_seeds, Scenes.Constants.mango, default_minimum_elephants*3, default_minimum_elephants*2, default_starter_money-5, 0, default_fruit_value, level_3_upgrades.Find(item => item.GetItemType().Equals("distraction_plant")).GetItemName(), default_distraction_plant_value, level_3_upgrades);
        levels.Add(1, level_1);
        levels.Add(2, level_2);
        levels.Add(3, level_3);
        player_inventory = new List<InventoryItem>();
        money_available = GetStarterMoney();
        current_day = 1;
    }

    private static void InitializeAllAvailableUpgradeItems()
    {
        all_upgrade_items = new List<UpgradeItem>
        {
            new UpgradeItem("001", "seeds", Scenes.UpgradeItemDescriptions.extra_seeds, "seeds", 0, 0, 5),
            new UpgradeItem("002", "fence", Scenes.UpgradeItemDescriptions.fence, Scenes.Constants.defense, 0, 0, 1),
            new UpgradeItem("003", "stone_wall", Scenes.UpgradeItemDescriptions.stone_wall, Scenes.Constants.defense, 0, 0, 2),
            new UpgradeItem("004", "beehive", Scenes.UpgradeItemDescriptions.beehive, Scenes.Constants.distraction, 0, 0, 2),
            new UpgradeItem("005", "camp_fire", Scenes.UpgradeItemDescriptions.camp_fire, Scenes.Constants.distraction, 0, 0, 4),
            new UpgradeItem("006", "noise_maker",Scenes.UpgradeItemDescriptions.noise_maker, Scenes.Constants.distraction, 0, 0, 4),
            new UpgradeItem("007", "chili", Scenes.UpgradeItemDescriptions.chili, "distraction_plant", 0, 0, 2),
            new UpgradeItem("008", "sunflower", Scenes.UpgradeItemDescriptions.sunflower, "distraction_plant", 0, 0, 3),
            new UpgradeItem("009", "fertilizer", Scenes.UpgradeItemDescriptions.fertilizer, "boost", 0, 0, 4),
            new UpgradeItem("010", "watering_can_upgrade", Scenes.UpgradeItemDescriptions.watering_can_upgrade, "boost", 0,0, 5),
            new UpgradeItem("011", "watering_can_puddle_upgrade", Scenes.UpgradeItemDescriptions.watering_can_puddle_upgrade, "boost", 0, 0, 5)
        };

    }

    private static List<UpgradeItem> GetAllUpgradeItems()
    {
        return all_upgrade_items;
    }

    private static List<UpgradeItem> RandomizeUpgradeItemsForLevel(int level_expected_quota)
    {
        GD.Randomize();
        List<UpgradeItem> upgrades = new();
        //Randomizing extra seed count, how many packs of 10 seeds player are given (at least one or something up to 10% of level's quota)
        UpgradeItem extra_seeds = all_upgrade_items.Find(item => item.GetItemName().Equals("seeds"));
        int max_extras = level_expected_quota / 2 / 10;
        int extra_seeds_count = GD.RandRange(1, max_extras);
        extra_seeds.SetTotalForLevel(extra_seeds_count);
        extra_seeds.SetTotalInStock(extra_seeds_count);
        upgrades.Add(extra_seeds);

        List<UpgradeItem> all_defenses = all_upgrade_items.FindAll(item => item.GetItemType().Equals("defense"));
        int defense_items_count = GD.RandRange(1, all_defenses.Count);
        int minimum = (int) (level_expected_quota * 0.5f);
        int max = (int)(level_expected_quota * 0.75f);
        //randomizing if player gets both types of defense items and how many
        RandomizeUpgradeItem(upgrades, all_defenses, defense_items_count, minimum, max);

        
        List<UpgradeItem> all_noise_distractions = all_upgrade_items.FindAll(item => item.GetItemType().Equals("distraction"));
        int noise_distractions_count = GD.RandRange(1, all_noise_distractions.Count);
        int minimum_distraction_item_per_type = 1 + (int) (level_expected_quota * 0.10f);
        int maximum_distraction_item_per_type = (int) (level_expected_quota * 0.15f);
        RandomizeUpgradeItem(upgrades, all_noise_distractions, noise_distractions_count, minimum_distraction_item_per_type, maximum_distraction_item_per_type);

        List<UpgradeItem> all_plant_distractions = all_upgrade_items.FindAll(item => item.GetItemType().Equals("distraction_plant"));
        int distraction_plants_count = 1;
        int distraction_plant_minimum = (int) (level_expected_quota * 0.25f);
        int distraction_plant_max = (int)(level_expected_quota * 0.5f);
        RandomizeUpgradeItem(upgrades, all_plant_distractions, distraction_plants_count, distraction_plant_minimum, distraction_plant_max);

        int min_fertilizer_count = (int) (level_expected_quota * 0.25f);
        int max_fertilizer_count = (int) (level_expected_quota * 0.5f);
        int fertilizer_count = GD.RandRange(min_fertilizer_count, max_fertilizer_count);
        UpgradeItem fertilizer = all_upgrade_items.Find(item => item.GetItemName().Equals("fertilizer"));
        fertilizer.SetTotalForLevel(fertilizer_count);
        fertilizer.SetTotalInStock(fertilizer_count);
        upgrades.Add(fertilizer);

        int min_watering_can_size_upgrade_count = (int) (level_expected_quota * 0.5f)/10;
        int max_watering_can_size_upgrade_count = (int) (level_expected_quota * 0.80f) /10;
        int watering_can_size_upgrade_count = GD.RandRange(min_watering_can_size_upgrade_count, max_watering_can_size_upgrade_count);
        UpgradeItem watering_can_size_upgrade = all_upgrade_items.Find(item => item.GetItemName().Equals("watering_can_upgrade"));
        watering_can_size_upgrade.SetTotalForLevel(watering_can_size_upgrade_count);
        watering_can_size_upgrade.SetTotalInStock(watering_can_size_upgrade_count);
        upgrades.Add(watering_can_size_upgrade);
        return upgrades;
    }

    private static void RandomizeUpgradeItem(List<UpgradeItem> upgrades, List<UpgradeItem> available_upgrades, int upgrades_count, int minimum_amount, int maximum_count)
    {
        if (upgrades_count == 1)
        {
            int random_index = GD.RandRange(0, available_upgrades.Count-1);
            UpgradeItem item = available_upgrades[random_index];
            //Randomizing amount of items
            int item_count = GD.RandRange(minimum_amount, maximum_count);
            item.SetTotalForLevel(item_count);
            item.SetTotalInStock(item_count);
            upgrades.Add(item);
        }
        else
        {
            for (int i = 0; i < upgrades_count; i++)
            {
                int random_item_index = GD.RandRange(0, available_upgrades.Count-1);
                UpgradeItem new_item = available_upgrades[random_item_index];
                int item_count = GD.RandRange(minimum_amount, maximum_count);
                int already_added_index = upgrades.FindIndex(item => item.GetItemName().Equals(new_item.GetItemName()));
                if (already_added_index == -1)
                {
                    new_item.SetTotalForLevel(item_count);
                    new_item.SetTotalInStock(item_count);
                    upgrades.Add(new_item);
                }
            }
        }
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

public void ResetLevel(int level_number)
    {
        LevelData level = GetLevelData(level_number);
        if(level_number == 3)
        {
          level.SetLevelTotalDays(default_days+1);
          level.SetLevelAvailableSeeds(default_quota * level_number + default_extra_seeds);
        } else
        {
            level.SetLevelAvailableSeeds(default_quota * level_number + default_extra_seeds * 2);
            level.SetLevelTotalDays(default_days);
        }
        level.SetCurrentQuota(0);
        if(level_number == 1)
        {
            level.SetLevelCurrentMoney(level.GetLevelStarterMoney());
            money_available = level.GetLevelCurrentMoney();
        } else if(level_number > 1)
        {
            level.SetLevelCurrentMoney(0);
            LevelData previous_level = GetLevelData(current_active_Level-1);
            if(previous_level != null)
            {
                int new_money = level.GetLevelStarterMoney() + previous_level.GetLevelCurrentMoney();
                level.SetLevelCurrentMoney(new_money);
                money_available = new_money;
            } else
            {
                level.SetLevelCurrentMoney(level.GetLevelStarterMoney());
                money_available = level.GetLevelCurrentMoney();
            }
            
        }
        
        level.SetLevelDayWhenQuotaFilled(0);
        SetCurrentDay(1);
        SetWateringCanLevel(0);
        SetWateringCanTotalLevel(watering_can_total_default_level);
        level.GetLevelUpgradeItems().Clear();
        level.SetLevelUpgradeItems(RandomizeUpgradeItemsForLevel(level.GetExpectedQuota()));
        level.SetLevelUsedUpgradeItemsCount(0);
        UpgradeItem distractionPlant = level.GetLevelUpgradeItems().Find(item => Scenes.Constants.distraction_plant.Equals(item.GetItemType()));
        if(distractionPlant.GetItemName() != null)
        {
            level.SetLevelDistractionPlantType(distractionPlant.GetItemName());
        }
        player_has_failed = false;
        
        
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
        GetLevelDataForActiveLevel().SetLevelCurrentMoney(money_available);
    }

public void MinusFromTotalMoney(int amount)
    {
        money_available -= amount;
        GetLevelDataForActiveLevel().SetLevelCurrentMoney(money_available);
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
            case Scenes.Constants.pineapple:
				texture = Scenes.ItemTextures.pineapple;
				break;
			case Scenes.Constants.watermelon:
				texture = Scenes.ItemTextures.watermelon;
				break;
			case Scenes.Constants.mango:
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
				if (plant_type.Equals(Scenes.Constants.pineapple))
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

    public string GetCurrentLevelDistractionPlantType()
    {
        return GetLevelDataForActiveLevel().GetLevelDistractionPlantType();

    }

    public int GetCurrentDay()
    {
        return current_day;
    }

    public void SetCurrentDay(int day)
    {
        current_day = day;
    }

    public bool GetGameStarted()
    {
        return game_started;
    }

    public void SetGameStarted(bool started)
    {
        game_started = started;
    }


    public bool GetSkipStartCutScene()
    {
        return SkipStartCutscene;
    }

    public void SetSkipStartCutScene(bool toggled)
    {
        SkipStartCutscene = toggled;
    }

    public int GetDaysLeft()
    {
        return days_left;
    }

    public void SetDaysLeft(int days)
    {
        days_left = days;
    }

    public bool GetRestartPressed()
    {
        return restart_pressed;
    }

    public void SetRestartPressed(bool pressed)
    {
       restart_pressed = pressed;
    }
}










