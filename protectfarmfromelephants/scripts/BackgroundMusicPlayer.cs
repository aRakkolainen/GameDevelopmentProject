using Godot;
using System;

public partial class BackgroundMusicPlayer : AudioStreamPlayer2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		/* float volume = SettingsManager.Instance.GetMusicVolume();
		if(volume == 100)
		{
			VolumeDb = 0;
		} else
		{
			VolumeDb = (100 - volume)*(-1);
		} */

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
