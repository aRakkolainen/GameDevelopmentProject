using Godot;
using Godot.Collections;
using System;

public partial class Player : CharacterBody2D
{
	public int speed { get; set; } = 200;
	private bool playerIsAlive;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D _animatedSprite;

	private Godot.Collections.Array<string> inventory = new();

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		playerIsAlive = true;
	}
	public void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = inputDirection * speed;
	}

	public void AddToInventory(string item)
    {
        inventory.Add(item);
    }

	public void RemoveFromInventory(string item)
    {
        inventory.Remove(item);
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
