using Godot;
using System;

public partial class SettingsMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static void OnQuitButtonPressed()
	{
		LevelManager.Instance.QuitGame();
	}

	public static void OnMainMenuButtonPressed()
	{
		LevelManager.Instance.LoadScene(Scenes.Menus.main_menu);
	}

	public static void OnEditControlsButtonPressed()
	{
		LevelManager.Instance.LoadScene(Scenes.Menus.controls_menu);
	}

}
