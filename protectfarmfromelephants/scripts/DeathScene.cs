using Godot;
using System;

public partial class DeathScene : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static void On_main_menu_button_pressed()
	{
		LevelManager.Instance.LoadLevel(Scenes.Menus.main_menu);
	}

	public static void On_restart_button_pressed()
	{
		string current_level = LevelManager.Instance.GetCurrentActiveLevel();
		switch (current_level)
		{
			case "level_1":
				LevelManager.Instance.LoadLevel(Scenes.Levels.level_1);
				break;
			case "level_2":
				LevelManager.Instance.LoadLevel(Scenes.Levels.level_2);
				break;
			case "level_3":
				LevelManager.Instance.LoadLevel(Scenes.Levels.level_3);
				break;
			case "":
				GD.Print("Active level not found, unable to restart, returning main menu");
				LevelManager.Instance.LoadLevel(Scenes.Menus.main_menu);
				break;

		}
	}
}
