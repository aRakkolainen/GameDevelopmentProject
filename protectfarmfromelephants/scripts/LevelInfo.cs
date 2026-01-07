using Godot;
using System;

public partial class LevelInfo : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	Label money;
	public override void _Ready()
	{
		LevelData level = LevelManager.Instance.GetLevelDataForActiveLevel();
		Label title = GetNode<Label>("LevelTitleLabel");
		title.Text = "Level " + level.GetLevelNumber();
		money = GetNode<Label>("TotalMoneyLabel");
		money.Text = "Money: " + LevelManager.Instance.GetMoneyAvailable();
		Label days = GetNode<Label>("TotalDaysLabel");
		days.Text = "Days: " + level.GetLevelTotalDays();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnUpdatedMoneyText()
	{
		money.Text = "Money: " + LevelManager.Instance.GetMoneyAvailable();
	}
}
