using Godot;
using System;

public partial class MainMenu : Control
{	
	[Signal] public delegate void SettingsPopupOpenedEventHandler();

	public  override void _Ready()
	{
	}
	public static void OnStartButtonPressed()
	{
		GD.Print("Pressed Start Button");
		LevelManager.Instance.SetCurrentActiveLevel(2);
		LevelManager.Instance.InitializeLevelData();
		LevelManager.Instance.SetGameStarted(true);
		if(SettingsManager.Instance.GetSkipStartCutScene())
		{
			LevelManager.Instance.LoadScene(Scenes.Levels.level_2);
		} else
		{
			LevelManager.Instance.LoadScene(Scenes.CutScenes.start_cut_scene);
		}
	}
	public static void OnQuitButtonPressed()
	{
		GD.Print("Thanks for playing!");
		LevelManager.Instance.QuitGame();
	}

	public void OnSettingsButtonPressed(){
		EmitSignal(SignalName.SettingsPopupOpened);
	}
}
