using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public partial class LevelInfo : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	Label money;
	Label watering_can_level;

	Label days;

	Label info;
    CheckBox puddleUpgradeCheckBox;

	ItemList passiveUpgrades;

    private int watering_can_upgrades_total;


    public override void _Ready()
	{
		LevelData level = LevelManager.Instance.GetLevelDataForActiveLevel();
		Label title = GetNode<Label>("LevelTitleLabel");
		title.Text = "Level " + level.GetLevelNumber();
		money = GetNode<Label>("TotalMoneyLabel");
		money.Text = "Money: " + LevelManager.Instance.GetMoneyAvailable();
		days = GetNode<Label>("DaysLeftLabel");
		days.Text = "Days left: " + level.GetLevelTotalDays();
		watering_can_level = GetNode<Label>("WateringCanLabel");
		watering_can_level.Text = "Enough water for " + LevelManager.Instance.GetWateringCanLevel() + " tile(s)";
		info = GetNode<Label>("InfoLabel");
		info.Text = "";
		passiveUpgrades = GetNode<ItemList>("PassiveUpgrades");

		watering_can_upgrades_total = LevelManager.Instance.GetCurrentLevelWatercanUpgradeTotal();
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnUpdatedMoneyText()
	{
		money.Text = "Money: " + LevelManager.Instance.GetMoneyAvailable();
	}
	
	public void OnUpdatedWateringCanText()
	{
		if (LevelManager.Instance.GetWateringCanLevel() == 0)
		{
			watering_can_level.Text = "Collect more water!";
		} else
		{
			watering_can_level.Text = "Enough water for " + LevelManager.Instance.GetWateringCanLevel() + " tile(s)";

		}
	}
	public void OnUpdatedTimerText(int days_left)
	{
		if (days_left == 1)
		{
			days.Text = "Days left: " + days_left + " (Final day)";
		} else
		{
			days.Text = "Days left: " + days_left;
			
		}
	}

	public void OnUpdatedInfoText(string text)
	{
		info.Text = text;
	}

	public void OnUpdatedPassiveUpgradesList(string passive_upgrade_name, int update_times)
	{
		GD.Print(passiveUpgrades);
		int itemCount = passiveUpgrades.GetItemCount();
		for (int i=0; i < itemCount; i++)
		{
			GD.Print(passiveUpgrades.GetItemText(i));
				var texture = LevelManager.Instance.GetTextureByItemName("checkbox_checked");
            	var icon = (Texture2D)GD.Load(texture);
				string item_description = passiveUpgrades.GetItemText(i);
				if ("watering_can_upgrade".Equals(passive_upgrade_name) && item_description.Contains("Watering can size increased"))
				{
					passiveUpgrades.SetItemIcon(i, icon);
	
					string description = "Watering can size increased " + update_times + "/" + watering_can_upgrades_total;
					passiveUpgrades.SetItemText(i, description);
				} else if ("watering_can_puddle_upgrade".Equals(passive_upgrade_name) && item_description.Contains("Watering can puddle"))
				{
					passiveUpgrades.SetItemIcon(i, icon);
				}
		}
		
	}

	public static void OnQuitGameButtonPressed()
	{
		LevelManager.Instance.QuitGame();
	}

	public static void OnRestartButtonPressed()
	{
		LevelManager.Instance.RestartLevel();
	}
}
