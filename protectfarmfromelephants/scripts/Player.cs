using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	public int speed { get; set; } = 200;
	private bool playerIsAlive;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D _animatedSprite;

	private List<InventoryItem> inventory;

	private int max_inventory_size = 5;
	private int max_stack = 32;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		playerIsAlive = true;
		inventory = new List<InventoryItem>();
	}
	public void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = inputDirection * speed;
	}

	public void AddToInventory(InventoryItem item)
    {
		int index = FindItemFromInventory(item);
		if(inventory.Count < max_inventory_size)
		{
		if(index != -1)
		{
        	inventory.Add(item);
			GD.Print(inventory);
		} else
		{
			GD.Print(index);
			//InventoryItem currentItem = inventory[index];
			// int currentQuantity = currentItem.GetQuantity();
			// int max = currentItem.GetMaxQuantity();
			// if(currentQuantity < max)
			// {
			// 	inventory[index].SetQuantity(++currentQuantity);
			// }
		}
		} else
		{
			GD.Print("Inventory full, drop something!");
			
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

	public  List<InventoryItem> GetInventoryList()
	{
		return inventory;
	}

	public void Die()
    {
		GD.Print("You runned out of time!");
        playerIsAlive = false;
        _animatedSprite.Stop();
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
			_animatedSprite.Play("walk_left");
		}
		else if (Input.IsActionPressed("move_right"))
		{
			_animatedSprite.Play("walk_right");
		}
		else if (Input.IsActionPressed("move_up"))
		{
			_animatedSprite.Play("walk_backward");
		}
		else if (Input.IsActionPressed("move_down"))
		{
			_animatedSprite.Play("walk_forward");
		}
		else
		{
			_animatedSprite.Play("default");
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
