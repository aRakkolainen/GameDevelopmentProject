using Godot;
using System;

namespace ProtectFarm;
public partial class DistractionAudioPlayer : AudioStreamPlayer2D
{
	[Export] AudioStream noise_maker_sound; 

	[Export] AudioStream beehive_sound;

	[Export] AudioStream campfire_sound;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ChangeVolume();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayDistractionAudio(string name)
	{
		switch (name)
		{
			case "noise_maker":
				Stream = noise_maker_sound;
				break;
			case "beehive":
				Stream = beehive_sound;
				break; 
			case "camp_fire":
				Stream = campfire_sound;
				break;
		}
		Play();
	}

	public void OnSoundEffectVolumeChanged()
    {
        ChangeVolume();
    }

    private void ChangeVolume()
    {
        float volume = SettingsManager.Instance.GetSoundEffectsVolume() / 100.0f;
        VolumeDb = Mathf.LinearToDb(volume);
    }

}
