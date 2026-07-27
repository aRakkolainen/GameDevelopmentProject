using Godot;
using System;

public partial class StartCutScene : Node2D
{
	private int frame_number = 0;

	private int max_frames = 0;
	private AnimatedSprite2D start_scene_player;

	private Timer audio_timer;

	private BackgroundMusicPlayer musicPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		start_scene_player = GetNode<AnimatedSprite2D>("StartScenePlayer");
		musicPlayer = GetNode<BackgroundMusicPlayer>("BackgroundMusicPlayer");
		audio_timer = GetNode<Timer>("AudioTimer");
		start_scene_player.Animation = "start";
		start_scene_player.Stop();
		musicPlayer.Play();
		SpriteFrames frames = start_scene_player.GetSpriteFrames();
		max_frames = frames.GetFrameCount("start");
		audio_timer.Timeout += OnTimeOut;
		//max_frames = start_scene_player.GetFrameCount(start_scene_player.Animation);
	}

    private void OnTimeOut()
    {
        musicPlayer.Stop();
    }


    private void OnAnimationFinished()
    {
        LevelManager.Instance.LoadScene(Scenes.Levels.level_1);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void OnSkipButtonPressed()
	{
		LevelManager.Instance.LoadScene(Scenes.Levels.level_1);
	}

	public void OnNextButtonPressed()
	{
		frame_number++;
		if(frame_number < max_frames)
		{
			start_scene_player.SetFrame(frame_number);
			audio_timer.Start();
			musicPlayer.Play();
		} else
		{
			LevelManager.Instance.LoadScene(Scenes.Levels.level_1);
		}
	}
}
