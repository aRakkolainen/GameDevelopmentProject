using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class Player : CharacterBody2D
{
	public int speed { get; set; } = 200;
	private bool playerIsAlive;

	private bool playerIsPaused;
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


	//[Export] public FarmManager farm_manager;

	[Signal] public delegate void PlayerSellFruitEventHandler();

	[Signal] public delegate void PlayerAddToInventoryEventHandler(int id, string name, int quantity, int maxQuantity);

	[Signal] public delegate void PlayerTriedToWaterPlantEventHandler();

	[Signal] public delegate void PlayerTriedToPlantSeedEventHandler();

	[Signal] public delegate void PlayerTriedToUseFertilizerEventHandler();

	[Signal] public delegate void PlayerTriedToPlantDistractionPlantEventHandler(string item_name);

	[Signal] public delegate void PlayerTriedToPlaceDefenseItemEventHandler(string item_name);

	[Signal] public delegate void PlayerTriedToPlaceDistractionItemEventHandler(string item_name);

	[Signal] public delegate void PlayerTriedToDropFruitEventHandler(string item_name);

	public override void _Ready()
	{
		level = LevelManager.Instance.GetLevelDataForActiveLevel();
		player = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
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
		/* InventoryItem watering_can_item = new InventoryItem(0, "watering_can", 1, 1);
		InventoryItem plant_seeds = new InventoryItem(1, level.GetPlantType()+"_seeds", level.GetLevelAvailableSeeds(), max_stack);
		inventory.Add(watering_can_item);
		inventory.Add(plant_seeds); */
		EmitSignal(SignalName.PlayerAddToInventory, 0, "watering_can", "tool", 1, 1);
		EmitSignal(SignalName.PlayerAddToInventory, 1, level.GetPlantType() + "_seeds", "seeds", level.GetLevelAvailableSeeds(), max_stack);
		//Only for development
		EmitSignal(SignalName.PlayerAddToInventory, 2, "pineapple", "fruit", 3, max_stack);
	}

	public void AddToInventory(InventoryItem item)
    {
		EmitSignal(SignalName.PlayerAddToInventory, item.GetID(), item.GetItemName(), item.GetItemType(), item.GetQuantity(), item.GetMaxQuantity());
    }

	public void OnSimpleInventoryItemSelected(int index)
	{
		
	}

	public void OnInventoryItemActivatedForUse(string item_name, string item_type, int quantity)
	{
		GD.Print("You are trying to use item: " + item_name);
		holdItem = true;
		selectedItem = item_name;
		if (item_name.Equals("watering_can"))
		{
			EmitSignal(SignalName.PlayerTriedToWaterPlant);

		} else if (item_type.Equals("seeds"))
		{
			EmitSignal(SignalName.PlayerTriedToPlantSeed);
			selectedItem = item_type;
		} else if (item_type.Equals("boost"))
			{
				EmitSignal(SignalName.PlayerTriedToUseFertilizer);
			
		} else if (item_type.Equals("defense"))
		{
			EmitSignal(SignalName.PlayerTriedToPlaceDefenseItem, item_name);
		} else if (item_type.Equals("distraction"))
		{
			EmitSignal(SignalName.PlayerTriedToPlaceDistractionItem, item_name);
		} else if (item_type.Equals("plant_distraction"))
		{
			EmitSignal(SignalName.PlayerTriedToPlantDistractionPlant, item_name);
		} else if (item_type.Equals("fruit"))
		{
			EmitSignal(SignalName.PlayerTriedToDropFruit, item_name);
		}
	}

	public void OnFarmSeedPlaced(bool isSuccess)
	{
		holdItem = false;
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
		if (Input.IsActionPressed("move_left"))
		{
			if (!holdItem)
				{
					player.Play("walk");
				} 
			else
                {
                    PlayAnimationWithItem(true);
                }
                player.FlipH = true;
		}
		else if (Input.IsActionPressed("move_right"))
		{
			player.FlipH = false;
			if (!holdItem)
				{
					player.Play("walk");
				} else
				{
					 PlayAnimationWithItem(true);
				}
		}
		else if (Input.IsActionPressed("move_up"))
		{
			player.Play("walk_backward");
		}
		else if (Input.IsActionPressed("move_down"))
		{
			player.Play("walk_forward");
		}
		else
		{
			if (!holdItem)
				{
					player.Play("default");
				}
			else
				{
					PlayAnimationWithItem(false);
				}
		}
		var collision = MoveAndCollide(Velocity * (float)delta);
			
		}

	}

    private void PlayAnimationWithItem(bool isMoving)
    {
        if (selectedItem.Equals("watering_can"))
        {
			if (isMoving)
			{
            	player.Play("walk_with_watering_can");
			} else
			{
				player.Play("stand_with_watering_can");
			}
        }
        else if (selectedItem.Equals("seeds"))
        {
			if (isMoving)
			{
            	player.Play("walk_with_seed_pack");
			} else
			{
				player.Play("stand_with_seed_pack");
			}
        }
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
