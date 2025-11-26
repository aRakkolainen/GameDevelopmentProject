using Godot;
using Godot.Collections;
using System;

public partial class Player : CharacterBody2D
{
	public int speed { get; set; } = 200;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D _animatedSprite;

	public Godot.Collections.Array<string> inventory = new Array<string>();

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
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
	public override void _PhysicsProcess(double delta)
	{
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
			//_animatedSprite.Play("walk_up");
		}
		else if (Input.IsActionPressed("move_down"))
		{
			//_animatedSprite.Play("walk_down");
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
}
