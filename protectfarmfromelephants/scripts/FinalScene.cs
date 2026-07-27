using Godot;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtectFarm;
public partial class FinalScene : Node2D
{
	Label levelInfoLabel1;

	Label levelInfoLabel2;

	Label levelInfoLabel3;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		levelInfoLabel1 = GetNode<Label>("%LevelInfoLabel1");
		levelInfoLabel2 = GetNode<Label>("%LevelInfoLabel2");
		levelInfoLabel3 = GetNode<Label>("%LevelInfoLabel3");
		if(LevelManager.Instance.GetAllLevels( ) != null)
		{
			
			LevelData level1 = LevelManager.Instance.GetLevelData(1);
			LevelData level2 = LevelManager.Instance.GetLevelData(2);
			LevelData level3 = LevelManager.Instance.GetLevelData(3);
			levelInfoLabel1.Text = "Fruits sold: " + level1.GetCurrentQuota() + "\n" + "Money left: " + level1.GetLevelCurrentMoney() + "\n" + "Day when quota filled: " + level1.GetLevelDayWhenQuotaFilled() + "\n" + "Used upgrade items count: " + level1.GetLevelUsedUpgradeItemsCount() + "\n";
			levelInfoLabel2.Text = "Fruits sold: " + level2.GetCurrentQuota() + "\n" + "Money left: " + level2.GetLevelCurrentMoney() + "\n" + "Day when quota filled: " + level2.GetLevelDayWhenQuotaFilled() + "\n" + "Used upgrade items count: " + level2.GetLevelUsedUpgradeItemsCount() + "\n";
			levelInfoLabel3.Text = "Fruits sold: " + level3.GetCurrentQuota() + "\n" + "Money left: " + level3.GetLevelCurrentMoney() + "\n" + "Day when quota filled: " + level3.GetLevelDayWhenQuotaFilled() + "\n" + "Used upgrade items count: " + level3.GetLevelUsedUpgradeItemsCount() + "\n";
		}
		
		
	}


	public static void OnMainMenuButtonPressed()
	{
		LevelManager.Instance.LoadScene(Scenes.Menus.main_menu);
	}

	public static void OnQuitButtonPressed()
	{
		GD.Print("Thanks for playing!");
		LevelManager.Instance.QuitGame();
	}

	public static void OnCreditsButtonPressed()
	{
		LevelManager.Instance.LoadScene(Scenes.Menus.credits_scene);
	}
}
