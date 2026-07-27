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

	private LevelData level_data;
    private Label title;

	private Timer updateTimer;

    [Signal] public delegate void SettingsMenuOpenedEventHandler();


    public override void _Ready()
    {
        level_data = LevelManager.Instance.GetLevelDataForActiveLevel();
		updateTimer = GetNode<Timer>("UpdateTimer");
		updateTimer.Start();
        title = GetNode<Label>("LevelTitleLabel");
        UpdateTitle();
        money = GetNode<Label>("TotalMoneyLabel");
        UpdateMoneyText();
        days = GetNode<Label>("DaysLeftLabel");
        UpdateTimerText(level_data.GetLevelTotalDays());
        watering_can_level = GetNode<Label>("WateringCanLabel");
		UpdateWateringCanText();
        info = GetNode<Label>("InfoLabel");
        info.Text = "";
        passiveUpgrades = GetNode<ItemList>("PassiveUpgrades");
        watering_can_upgrades_total = LevelManager.Instance.GetCurrentLevelWatercanUpgradeTotal();


    }

    private void UpdateTitle()
    {
        title.Text = "Level " + level_data.GetLevelNumber();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void OnUpdatedMoneyText()
	{
		UpdateMoneyText();
	}
	
	public void OnUpdatedWateringCanText()
    {
        UpdateWateringCanText();
    }

    private void UpdateWateringCanText()
    {
        if (LevelManager.Instance.GetWateringCanLevel() == 0)
        {
            watering_can_level.Text = "Collect more water!";
        }
        else
        {
            watering_can_level.Text = "Enough water for " + LevelManager.Instance.GetWateringCanLevel() + " tile(s)";
        }
    }

    public void OnUpdatedTimerText(int days_left)
    {
        UpdateTimerText(days_left);
    }

    private void UpdateTimerText(int days_left)
    {
        if (days_left == 1)
        {
            days.Text = "Days left: " + days_left + " (Final day)";
        }
        else
        {
            days.Text = "Days left: " + days_left;

        }
    }

    public void OnUpdatedInfoText(string text)
	{
		info.Visible = true;
		info.Text = text;
		Timer timer = new Timer
        {
            WaitTime = 10,
			Autostart = true,
			OneShot = true,
        };
		AddChild(timer);
        timer.Timeout += HideInfoText;
	}

    private void HideInfoText()
    {
        info.Visible = false;
    }

    public void OnUpdatedPassiveUpgradesList(string passive_upgrade_name, int update_times)
	{
		int itemCount = passiveUpgrades.GetItemCount();
		for (int i=0; i < itemCount; i++)
		{
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

	 public void OnUpdateTimerTimeout()
    {
		UpdateTitle();
        UpdateMoneyText();
        UpdateWateringCanText();

    }

    private void UpdateMoneyText()
    {
        money.Text = "Money: " + LevelManager.Instance.GetMoneyAvailable();
    }

    public static void OnQuitGameButtonPressed()
	{
		LevelManager.Instance.QuitGame();
	}

	public static void OnRestartButtonPressed()
	{
		LevelManager.Instance.RestartLevel();
		int current_level = LevelManager.Instance.GetCurrentActiveLevel();
		LevelManager.Instance.ResetLevel(current_level);

	}

	public void OnSettingsButtonPressed()
	{
		EmitSignal(SignalName.SettingsMenuOpened);
	}
}
