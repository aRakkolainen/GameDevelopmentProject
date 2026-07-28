using Godot;
using System;

public partial class SoundEffectPlayer : AudioStreamPlayer2D
{
	//Player
	[Export] AudioStream watering_can_filled_sound; 

	[Export] AudioStream watering_plants;

	[Export] AudioStream walk;

	[Export] AudioStream placing_seed;

	//Elephants
	[Export] public AudioStream normal_walk;

	[Export] public AudioStream frustrated_walk;

	[Export] public AudioStream afraid_elephant;

	[Export] public AudioStream buy_item;

	[Export] public AudioStream sell_item;

	private Timer sfx_timer;
    private float volume;

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
	{
        ChangeVolume();
		sfx_timer = GetNode<Timer>("SoundEffectTimer");
		if(sfx_timer != null)
		{
			sfx_timer.Timeout += OnSoundEffectEnded;
		}
	
	}

    private void OnSoundEffectEnded()
    {
        Stop();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if(SettingsManager.Instance.GetSoundEffectsVolume() != volume)
        {
            ChangeVolume();
        }
    }

	public void OnSoundEffectStarted(string starter, string effect, int duration)
    {
        PlaySoundEffect(starter, effect, duration);
    }

    public void PlaySoundEffect(string starter, string effect, int duration)
    {
        if (starter.Equals("player"))
        {
			sfx_timer.WaitTime = duration / 0.5f;
            switch (effect)
            {
                case "watering_can_filled":
                    Stream = watering_can_filled_sound;
                    break;
                case "watering_plants":
                    Stream = watering_plants;
                    break;
                case "placing_seeds":
					Stream = placing_seed;
                    break;
                case "walk":
                    Stream = walk;
                    break;
                default:
                    Stream= walk;
                    break;
            }
			Play();
        }
        else if (starter.Equals("elephant"))
        {
			Stop();
			sfx_timer.WaitTime = duration;
            switch (effect)
            {
                case "walk":
                    Stream = normal_walk;
                    break;
                case "frustrated_walk":
                    Stream = frustrated_walk;
                    break;
                case "afraid_elephant":
                    Stream =afraid_elephant;
					sfx_timer.Start();
                    break;
                default:
                    Stream = normal_walk;
                    break;
            }
			Play();
        }
        else if (starter.Equals("UI"))
        {
            sfx_timer.WaitTime = duration;
            switch (effect)
            {
                case "sell_item":
                    Stream = sell_item;
                    break;
                case "buy_item":
                    Stream = buy_item;
                    break;
            }
            sfx_timer.Start();
			Play();
        }
    }


    public void OnTimerEnded()
    {
        Stop();
    }

    public void OnSoundEffectVolumeChanged()
    {
        ChangeVolume();
    }

	public Timer GetSoundEffectTimer()
	{
		return sfx_timer;
	}

    private void ChangeVolume()
    {
        volume = SettingsManager.Instance.GetSoundEffectsVolume() / 100.0f;
        VolumeDb = Mathf.LinearToDb(volume);
    }
}
