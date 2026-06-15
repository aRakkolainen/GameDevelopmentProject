using Godot;
using System;

public partial class SettingsMenu : Control
{
	private float current_music_volume; 

	private float current_sound_effects_volume;

	private Label MusicVolumeLabel;

	private Label SoundEffectsVolumeLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MusicVolumeLabel = GetNode<Label>("SelectedMusicVolumeLabel");
		SoundEffectsVolumeLabel = GetNode<Label>("SelectedSoundEffectsVolumeLabel");
		MusicVolumeLabel.Text = SettingsManager.Instance.GetMusicVolume().ToString() + " %";
		SoundEffectsVolumeLabel.Text = SettingsManager.Instance.GetSoundEffectsVolume().ToString() + " %";
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

	public  void OnMusicVolumeSliderValueChanged(float value)
	{
		current_music_volume = value;
		MusicVolumeLabel.Text = value.ToString() + " %";
	}

	public void OnSoundEffectVolumeSliderValueChanged(float value)
	{
		current_sound_effects_volume = value;
		SoundEffectsVolumeLabel.Text = value.ToString() + " %";
	}

	public void OnSaveSettingsButtonPressed()
	{
		GD.Print("Before:" + SettingsManager.Instance.GetMusicVolume());
		GD.Print(SettingsManager.Instance.GetSoundEffectsVolume());
		SettingsManager.Instance.SetMusicVolume(current_music_volume);
		SettingsManager.Instance.SetSoundEffectsVolume(current_sound_effects_volume);

		GD.Print("After: " + SettingsManager.Instance.GetMusicVolume());
		GD.Print(SettingsManager.Instance.GetSoundEffectsVolume());
	}

}
