using Godot;
using System;

public partial class LevelTransitionAnimations : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private AnimatedSprite2D transition_level_animations;

	private int current_level_num;

	private string current_fruit;
	public override void _Ready()
	{
		AnimatedSprite2D transition_level_animations = GetNode<AnimatedSprite2D>("BetweenLevelsAnimations");
		LevelData level_data = LevelManager.Instance.GetLevelDataForActiveLevel();
		if (level_data != null)
		{
			current_level_num = level_data.GetLevelNumber();
			current_fruit = level_data.GetPlantType();
			switch (current_fruit)
			{
				case"pineapple":
					transition_level_animations.Play("eat_pineapple");
					break;
				case"mango":
					transition_level_animations.Play("eat_mango");
					break;
				case"watermelon":
					transition_level_animations.Play("eat_watermelon");
					break;
				default:
					break;
			}
		}

		transition_level_animations.AnimationFinished += OnAnimationFinished;
	}

    private void OnAnimationFinished()
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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		
	}
}
