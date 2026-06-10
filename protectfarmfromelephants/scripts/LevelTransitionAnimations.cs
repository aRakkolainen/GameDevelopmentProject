using Godot;
using System;

public partial class LevelTransitionAnimations : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private AnimatedSprite2D transition_level_animations;

	private int current_level_num;

	private string current_fruit;

	private int frame_number = 0;

	private int max_frames = 0;
	public override void _Ready()
	{

		transition_level_animations = GetNode<AnimatedSprite2D>("BetweenLevelsAnimations");
		LevelData level_data = LevelManager.Instance.GetLevelDataForActiveLevel();
		if (level_data != null)
		{
			current_level_num = level_data.GetLevelNumber();
			current_fruit = level_data.GetPlantType();
			SpriteFrames frames = transition_level_animations.GetSpriteFrames();
			string animation_name = "";
			if (LevelManager.Instance.GetPlayerHasFailed())
			{
				animation_name = "eat_player";
			} else
			{
			switch (current_fruit)
			{
				case"pineapple":
					animation_name = "eat_pineapple";
					break;
				case"mango":
					animation_name ="eat_mango";
					break;
				case"watermelon":
					animation_name = "eat_watermelon";
					break;
				default:
					break;
			}
			}
				transition_level_animations.Stop();
				transition_level_animations.Animation = animation_name;
				max_frames = frames.GetFrameCount(animation_name);
		}

	}

    private void AnimationFinished()
    {
		if (LevelManager.Instance.GetPlayerHasFailed())
		{
			LevelManager.Instance.LoadScene(Scenes.CutScenes.death_scene);
		} else
		{
			
       LevelManager.Instance.SetCurrentActiveLevel(current_level_num + 1);

	   if (LevelManager.Instance.GetCurrentActiveLevel() == 2)
        {
            LevelManager.Instance.LoadScene(Scenes.Levels.level_2);
        }
        else if (LevelManager.Instance.GetCurrentActiveLevel() == 3)
        {
            LevelManager.Instance.LoadScene(Scenes.Levels.level_3);
        }
		}
    }

	public void OnSkipButtonPressed()
	{
		AnimationFinished();
	}

	public void OnNextButtonPressed()
	{
		frame_number++;
		if(frame_number < max_frames)
		{
			transition_level_animations.SetFrame(frame_number);
		} else
		{
			AnimationFinished();
		}
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		
	}
}
