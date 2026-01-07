using Godot;
using System;

public partial class LevelInfo : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	Label money;
	Label watering_can_level;

	Label days;

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
}
