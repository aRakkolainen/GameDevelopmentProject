using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

public partial class Player : CharacterBody2D
{
	public int speed { get; set; } = 200;
	private bool playerIsAlive;

	private bool playerIsPaused;

	private bool playerIsWalking = false;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D player;

	private List<InventoryItem> inventory;

	private int max_inventory_size = 10;
	private int max_stack = 32;

	private bool watercan_filled;

	private int watercan_fill_level = 0;

	private int max_watercan_fill_level = 5;


	private LevelData level;

	private bool holdItem = false;

	private string selectedItem;


	private SoundEffectPlayer soundEffectPlayer;

	[Signal] public delegate void PlayerSellFruitEventHandler();

	[Signal] public delegate void PlayerAddToInventoryEventHandler(int id, string name, int quantity, int maxQuantity);

	[Signal] public delegate void PlayerTriedToWaterPlantEventHandler();

	[Signal] public delegate void PlayerTriedToPlantSeedEventHandler();

	[Signal] public delegate void PlayerTriedToUseFertilizerEventHandler(bool isSuperFertilizer, int id);

	[Signal] public delegate void PlayerTriedToPlantDistractionPlantEventHandler(int id, string item_name);

	[Signal] public delegate void PlayerTriedToPlaceDefenseItemEventHandler(int id, string item_name);

	[Signal] public delegate void PlayerTriedToPlaceDistractionItemEventHandler(int id, string item_name);

	[Signal] public delegate void PlayerTriedToDropPlantEventHandler(int id, string item_name);

	[Signal] public delegate void PlayerPlayedSoundEffectEventHandler(string starter, string effect, int duration);

	[Signal] public delegate void PlayerSoundEffectEndedEventHandler();

	public override void _Ready()
	{
		level = LevelManager.Instance.GetLevelDataForActiveLevel();
		player = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		soundEffectPlayer = GetNode<SoundEffectPlayer>("SoundEffectPlayer");
		playerIsAlive = true;
		inventory = new List<InventoryItem>();
		AddDefaultItemsToInventory();
		LevelManager.Instance.SetPlayerInventory(inventory);
	}
	public void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = inputDirection * speed;
	}

	public void AddDefaultItemsToInventory()
	{
		EmitSignal(SignalName.PlayerAddToInventory, 0, "watering_can", "tool", 1, 1);
		EmitSignal(SignalName.PlayerAddToInventory, 1, level.GetPlantType() + "_seeds", "seeds", level.GetLevelAvailableSeeds(), max_stack);
		//Only for development & testing
		/* EmitSignal(SignalName.PlayerAddToInventory, 2, "pineapple", "plant", 3, max_stack);
		EmitSignal(SignalName.PlayerAddToInventory, 3, "chili", "plant", 4, max_stack); */
	}

	public void AddToInventory(InventoryItem item)
    {
		EmitSignal(SignalName.PlayerAddToInventory, item.GetID(), item.GetItemName(), item.GetItemType(), item.GetQuantity(), item.GetMaxQuantity());
    }

	public void OnSimpleInventoryItemSelected(int index)
	{
		
	}

	public void OnSoundEffectStarted(string starter, string effect, int duration)
	{
		if (!effect.Equals("walk"))
		{
			soundEffectPlayer.GetSoundEffectTimer().Start();
			EmitSignal(SignalName.PlayerPlayedSoundEffect, starter, effect, duration);
		} else
		{
			EmitSignal(SignalName.PlayerPlayedSoundEffect, starter, effect, duration);
		}
	}

    public void OnInventoryItemActivatedForUse(int id, string item_name, string item_type, int quantity)
	{
		GD.Print("You are trying to use item: " + item_name);
		holdItem = true;
		selectedItem = item_name;
		if (item_name.Equals("watering_can"))
		{
			EmitSignal(SignalName.PlayerTriedToWaterPlant);
		} else if (item_type.Equals("seeds"))
		{
			EmitSignal(SignalName.PlayerTriedToPlantSeed, id);
		} else if (item_type.Equals("boost"))
			{
				if ("super_fertilizer".Equals(item_name))
			{
				EmitSignal(SignalName.PlayerTriedToUseFertilizer, true, id);
			} else
			{
				EmitSignal(SignalName.PlayerTriedToUseFertilizer, false, id);
			}
			
		} else if (item_type.Equals("defense"))
		{
			EmitSignal(SignalName.PlayerTriedToPlaceDefenseItem, id, item_name);
		} else if (item_type.Equals("distraction"))
		{
			EmitSignal(SignalName.PlayerTriedToPlaceDistractionItem, id, item_name);
		} else if (item_type.Equals("distraction_plant"))
		{
			if ("chili_seeds".Equals(item_name))
			{
				item_name = "chili";
			} else if ("sunflower_seeds".Equals(item_name))
			{
				item_name = "sunflower";
			} else
			{
				item_name = "";
			}
			EmitSignal(SignalName.PlayerTriedToPlantDistractionPlant, id, item_name);
		} else if (item_type.Equals("plant"))
		{
			EmitSignal(SignalName.PlayerTriedToDropPlant, id, item_name);
		}
	}

	public void OnFarmSeedPlaced(bool isSuccess)
	{
		holdItem = false;
	}

	public void OnInventoryItemRemoved(string item_name)
	{
		if(selectedItem == item_name)
		{
			holdItem = false;
		}
	}

	public void Die()
    {
		GD.Print("You runned out of time!");
        playerIsAlive = false;
        player.Stop();
    }

	public void Pause()
	{
		playerIsPaused = true;
	}

	public void Continue()
	{
		playerIsPaused = false;
	}

	public void UseWaterCan()
	{
		player.Play();
	}

	public override void _PhysicsProcess(double delta)
	{
        if (!playerIsAlive || playerIsPaused)
        {
            return;
        } else
		{
		GetInput();
		if (Input.IsActionPressed("move_left") || Input.IsActionJustPressed("move_left"))
		{
			playerIsWalking = true;
			if (holdItem)
				{
					if (selectedItem != null)
					{
						player.Play(GetAnimationName(true, false));
					}
				} 
			else
				{
					player.Play("walk");
				}
            player.FlipH = true;
		}
		else if (Input.IsActionPressed("move_right"))
		{
			playerIsWalking = true;
			player.FlipH = false;
			if (!holdItem)
				{
					player.Play("walk");
				} else
				{
					if (selectedItem != null)
					{
						player.Play(GetAnimationName(true, false));
					}
				}
		}
		else if (Input.IsActionPressed("move_up") || Input.IsActionJustPressed("move_up"))
		{
			player.Play("walk_backward");
		}
		else if (Input.IsActionPressed("move_down") || Input.IsActionJustPressed("move_down"))
		{
			playerIsWalking = true;
			if (!holdItem)
				{
					player.Play("walk_forward");
				}
				else
				{
					if (selectedItem != null)
					{
						player.Play(GetAnimationName(true, true));
					}
				}
		}
		else
		{
			if (!holdItem)
				{
					player.Play("default");
					playerIsWalking = false;
				}
			else
				{
					if (selectedItem != null)
					{
						playerIsWalking = false;
						player.Play(GetAnimationName(false, true));
					}
				}
		}

		if(Input.IsActionJustPressed("move_right") || Input.IsActionJustPressed("move_left") || Input.IsActionJustPressed("move_up") || Input.IsActionJustPressed("move_down"))
			{
				soundEffectPlayer.PlaySoundEffect("player", "walk", 1);
			}
		var collision = MoveAndCollide(Velocity * (float)delta);


	}
	}

    private string GetAnimationName(bool isMoving, bool isFacingForward)
    {
		string descr = "";
		if (isMoving)
		{
			if (isFacingForward)
			{
				descr = "walk_forward_with_" + selectedItem;
			} else
			{
				descr = "walk_with_" + selectedItem;
			}
		} else
		{
			descr = "stand_with_" + selectedItem;
		}

		return descr;
    }


    public bool GetPlayerIsAlive()
    {
        return playerIsAlive;
    }

	public void SetPlayerIsAlive(bool status)
    {
        playerIsAlive = status;
    }


}
