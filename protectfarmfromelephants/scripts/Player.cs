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


	//[Export] public FarmManager farm_manager;

	[Signal] public delegate void PlayerSellFruitEventHandler();

	[Signal] public delegate void PlayerAddToInventoryEventHandler(int id, string name, int quantity, int maxQuantity);

	[Signal] public delegate void PlayerTriedToWaterPlantEventHandler();

	[Signal] public delegate void PlayerTriedToPlantSeedEventHandler();
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
		if (item_name.Equals("watering_can"))
		{
			EmitSignal(SignalName.PlayerTriedToWaterPlant);
		} else if (item_type.Equals("seeds"))
		{
			EmitSignal(SignalName.PlayerTriedToPlantSeed);
		}
	}

	public void UseItem()
	{
		
	}

	/* public void RemoveFromInventory(InventoryItem item)
    {
        inventory.Remove(item);
    }

	public int FindIndexForItemInInventory(InventoryItem item)
	{
		return inventory.FindIndex(i=> i.GetItemName() == item.GetItemName());
	} */

	/* public int GetNumberOfSeedsAvailable()
	{
		
		int index = inventory.FindIndex(i => i.GetItemName() == level.GetPlantType()+"_seeds");
		if (index == -1)
		{
			GD.Print("No seeds in inventory!");
			return 0;
		} else
		{
			return inventory[index].GetQuantity();
		}
	}

	public void SetNumberOfSeedsAvailable(int count)
	{
		int index = inventory.FindIndex(i => i.GetItemName() == level.GetPlantType()+"_seeds");
		if (index == -1)
		{
			GD.Print("No seeds found, cannot update");
		} else
		{
			inventory[index].SetQuantity(count);
		}
	}



	public void ClearInventory()
	{
		inventory.Clear();
	}

	public  List<InventoryItem> GetInventoryList()
	{
		return inventory;
	}

	public int GetInventoryCount()
	{
		return inventory.Count;
	} */

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
			player.Animation = "walk";
			player.FlipH = true;
		}
		else if (Input.IsActionPressed("move_right"))
		{
			player.FlipH = false;
			player.Play("walk");
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
			player.Play("default");
		}
		var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null)
		{
			GD.Print("Collided with: " + collision.GetCollider());
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
