using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Godot;
using ProtectFarm;

public partial class SettingsManager : Node
{
    public static SettingsManager Instance {get; private set; }

    private Dictionary<string, float> AudioSettings;
    
    public override void _Ready()
    {
        Instance = this;
        InitializeDefaultAudioSettings();
    }

    public Dictionary<string, float> GetAllAudioSettings()
{
    return AudioSettings;
}

    public float GetMusicVolume()
    {
        return  AudioSettings["music"];
    }

    public void SetMusicVolume(float volume)
    {
        if (AudioSettings != null)
        {
            AudioSettings["music"] = volume;
        }
    }

    public float GetSoundEffectsVolume()
    {
        return  AudioSettings["sound_effects"];
    }

    public void SetSoundEffectsVolume(float volume)
    {
        if (AudioSettings != null)
        {
            AudioSettings["sound_effects"] = volume;
        }
    }

    public void InitializeDefaultAudioSettings()
    {
        AudioSettings = new Dictionary<string, float>
        {
            { "music", 100 },
            { "sound_effects", 100 }
        };
    }


}