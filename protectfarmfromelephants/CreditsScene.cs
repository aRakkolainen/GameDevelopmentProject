using Godot;
using System;

namespace ProtectFarm;
public partial class CreditsScene : Node2D
{

	public static void OnMainMenuButtonPressed()
	{
		LevelManager.Instance.LoadScene(Scenes.Menus.main_menu);
	}

	public static void OnQuitButtonPressed()
	{
		GD.Print("Thanks for playing!");
		LevelManager.Instance.QuitGame();
	}

	public static void OnStatsMenuButtonPressed()
	{
		LevelManager.Instance.LoadScene(Scenes.CutScenes.final_scene);
	}
}
