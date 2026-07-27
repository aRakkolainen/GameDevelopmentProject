using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Godot;
using ProtectFarm;

public partial class SettingsManager : Node
{
    public static SettingsManager Instance {get; private set; }

    private Dictionary<string, float> Settings;
    
    public override void _Ready()
    {
        Instance = this;
        InitializeDefaultSettings();
    }

    public Dictionary<string, float> GetAllSettings()
{
    return Settings;
}

    public float GetMusicVolume()
    {
        return  Settings["music"];
    }

    public void SetMusicVolume(float volume)
    {
        if (Settings != null)
        {
            Settings["music"] = volume;
        }
    }

    public float GetSoundEffectsVolume()
    {
        return  Settings["sound_effects"];
    }

    public void SetSoundEffectsVolume(float volume)
    {
        if (Settings != null)
        {
            Settings["sound_effects"] = volume;
        }
    }

    public void InitializeDefaultSettings()
    {
        Settings = new Dictionary<string, float>
        {
            { "music", 100 },
            { "sound_effects", 100 },
            {"skip_start_cut_scene", 0}
        };
    }

    public bool GetSkipStartCutScene()
    {
        if(Settings["skip_start_cut_scene"] == 1)
        {
            return true;
        } else
        {
            return false;
        }
    }

    internal void SetSkipStartCutScene(bool skipStartCutscene)
    {
        if (skipStartCutscene)
        {
            Settings["skip_start_cut_scene"] = 1;
        } else
        {
            Settings["skip_start_cut_scene"] = 0;
        }
    }
}