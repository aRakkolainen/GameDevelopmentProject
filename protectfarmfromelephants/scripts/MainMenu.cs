using Godot;
using System;

public partial class MainMenu : Control
{
	// [Export] private Button start_button;

	// [Export] private Button quit_button;
	// // Called when the node enters the scene tree for the first time.
	

	public static void On_start_button_pressed()
	{
		GD.Print("Pressed Start Button");
		LevelManager.Instance.SetCurrentActiveLevel("level_1");
		LevelManager.Instance.InitializeLevelData();
		LevelManager.Instance.LoadLevel(Scenes.Levels.level_1);
		
		
	}
	public static void On_quit_button_pressed()
	{
		GD.Print("Pressed Quit Button");
	}
}
