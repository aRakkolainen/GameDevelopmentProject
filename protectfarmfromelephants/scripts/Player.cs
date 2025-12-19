using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	public int speed { get; set; } = 200;
	private bool playerIsAlive;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D player;

	private List<InventoryItem> inventory;

	private int max_inventory_size = 5;
	private int max_stack = 32;

	private bool watercan_filled;

	private int watercan_fill_level = 0;

	private int max_watercan_fill_level = 5;

	[Signal] public delegate void PlayerSellFruitEventHandler();
	public override void _Ready()
	{
		player = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		playerIsAlive = true;
		inventory = new List<InventoryItem>();
	}
	public void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = inputDirection * speed;
	}

	public bool AddToInventory(InventoryItem item)
    {
		int index = FindItemFromInventory(item);
		GD.Print(inventory.Count);
		if(inventory.Count < max_inventory_size)
		{
		if(index == -1)
		{
        	inventory.Add(item);
			GD.Print("You collected item" + item.GetItemName() + " and total quantity is " + item.GetQuantity());
			return true;
		} else
		{
			InventoryItem currentItem = inventory[index];
			int currentQuantity = currentItem.GetQuantity();
			int max = currentItem.GetMaxQuantity();
			if(currentQuantity < max)
			{
				currentItem.SetQuantity(++currentQuantity);
				GD.Print("You collected item" + currentItem.GetItemName() + " and total quantity is " + currentItem.GetQuantity());
				return true;
			}
			return false;
		}
		} else
		{
			GD.Print("Inventory full, drop something!");
			return false;
			
		}
    }

	public void RemoveFromInventory(InventoryItem item)
    {
        inventory.Remove(item);
    }

	public int FindItemFromInventory(InventoryItem item)
	{
		return inventory.FindIndex(i=> i.GetItemName() == item.GetItemName());
	}

	public void ClearInventory()
	{
		inventory.Clear();
	}

	public  List<InventoryItem> GetInventoryList()
	{
		return inventory;
	}

	public void Die()
    {
		GD.Print("You runned out of time!");
        playerIsAlive = false;
        player.Stop();
    }

	public void UseWaterCan()
	{
		player.Play();
	}

	public override void _PhysicsProcess(double delta)
	{
        if (!playerIsAlive)
        {
            return;
        }
		GetInput();
		if (Input.IsActionPressed("move_left"))
		{
			player.Play("walk_left");
		}
		else if (Input.IsActionPressed("move_right"))
		{
			player.Play("walk_right");
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


		/* var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null){
			GD.Print("Caollided with: " + collision.GetCollider());
		} */
	}

	public bool GetPlayerIsAlive()
    {
        return this.playerIsAlive;
    }

	public void SetPlayerIsAlive(bool status)
    {
        this.playerIsAlive = status;
    }

    
}
