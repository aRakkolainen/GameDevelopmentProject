using Godot;
using System;

public partial class BackgroundMusicPlayer : AudioStreamPlayer2D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ChangeVolume();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	 private void ChangeVolume()
    {
        float volume = SettingsManager.Instance.GetMusicVolume() / 100.0f;
        VolumeDb = Mathf.LinearToDb(volume);
    }

	public void OnMusicVolumeChanged()
	{
		ChangeVolume();
	}

}
