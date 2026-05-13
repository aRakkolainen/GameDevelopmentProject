using Godot;
using System;

public partial class MainMenu : Control
{	
	public static void OnStartButtonPressed()
	{
		GD.Print("Pressed Start Button");
		LevelManager.Instance.SetCurrentActiveLevel(1);
		LevelManager.Instance.InitializeLevelData();
		LevelManager.Instance.LoadScene(Scenes.CutScenes.start_cut_scene);
	}
	public static void OnQuitButtonPressed()
	{
		GD.Print("Thanks for playing!");
		LevelManager.Instance.QuitGame();
	}

	public static void OnSettingsButtonPressed(){
		LevelManager.Instance.LoadScene(Scenes.Menus.settings_menu);
	}
}
