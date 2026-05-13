using Godot;
using System;

public partial class StartCutScene : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AnimatedSprite2D start_scene_player = GetNode<AnimatedSprite2D>("StartScenePlayer");
		start_scene_player.AnimationFinished += OnAnimationFinished;

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
}
