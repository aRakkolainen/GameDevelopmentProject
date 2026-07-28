using Godot;
using System;

namespace ProtectFarm;
public partial class SettingsMenuCanvasLayer : CanvasLayer
{

	private float current_music_volume = 100; 

	private Slider MusicVolumeSlider;

	private Slider SFXVolumeSlider;

	private float current_sound_effects_volume = 100;

	private Label MusicVolumeLabel;

	private Label SoundEffectsVolumeLabel;

	private Button ResumeGameButton;

	private bool SkipStartCutscene;

	private CheckButton SkipStartCutSceneCheckBox;
	[Signal] public delegate void ControlsPopupOpenedEventHandler();

	[Signal] public delegate void UpdatedMusicVolumeEventHandler();

	[Signal] public delegate void UpdatedSFXVolumeEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		MusicVolumeLabel = GetNode<Label>("SelectedMusicVolumeLabel");
		SoundEffectsVolumeLabel = GetNode<Label>("SelectedSoundEffectsVolumeLabel");
		ResumeGameButton = GetNode<Button>("%ResumeGameButton");
		MusicVolumeSlider = GetNode<Slider>("%MusicVolumeSlider");
		SFXVolumeSlider = GetNode<Slider>("%SoundEffectVolumeSlider");
		SkipStartCutSceneCheckBox = GetNode<CheckButton>("%StartCutSceneSkipCheckBox");
		MusicVolumeSlider.Value = SettingsManager.Instance.GetMusicVolume();
		SFXVolumeSlider.Value = SettingsManager.Instance.GetSoundEffectsVolume();
		MusicVolumeLabel.Text = SettingsManager.Instance.GetMusicVolume().ToString() + " %";
		SoundEffectsVolumeLabel.Text = SettingsManager.Instance.GetSoundEffectsVolume().ToString() + " %";
		if(SkipStartCutSceneCheckBox != null)
		{
			SkipStartCutSceneCheckBox.ButtonPressed = SettingsManager.Instance.GetSkipStartCutScene();
		}
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

	public void OnEditControlsButtonPressed()
	{
		Hide();
		EmitSignal(SignalName.ControlsPopupOpened);
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
        SaveSettings();

    }

    private void SaveSettings()
    {
        if (!current_music_volume.Equals(SettingsManager.Instance.GetMusicVolume()))
        {
            SettingsManager.Instance.SetMusicVolume(current_music_volume);
            EmitSignal(SignalName.UpdatedMusicVolume);
        }

        if (!current_sound_effects_volume.Equals(SettingsManager.Instance.GetSoundEffectsVolume()))
        {
            SettingsManager.Instance.SetSoundEffectsVolume(current_sound_effects_volume);
            EmitSignal(SignalName.UpdatedSFXVolume);
        }

		if(SkipStartCutscene != SettingsManager.Instance.GetSkipStartCutScene())
		{
			SettingsManager.Instance.SetSkipStartCutScene(SkipStartCutscene);
		}
    }

    public void OnStartCutSceneSkipCheckBoxToggled(bool toggled)
	{
		SkipStartCutscene = toggled;
		LevelManager.Instance.SetSkipStartCutScene(SkipStartCutscene);
	}



	public void OnLevelSettingsMenuOpened()
	{
		if (!LevelManager.Instance.GetGameStarted())
		{
			ResumeGameButton.Hide();
		}
		Show();
	}

	public void OnResumeGameButtonPressed()
	{
		SaveSettings();
		Hide();
	}
}
