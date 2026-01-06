using Godot;
using System;

public partial class Elephant : CharacterBody2D
{
	[Export] public float Speed = 100.0f;

	[Export] public Vector2 MoveDirection = Vector2.Right;
	private AnimatedSprite2D _animatedSprite;

	private CollisionShape2D _collisionShape;

	[Signal]

	public delegate void CollidedWithFarmEventHandler();


	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_animatedSprite.Play("walk");
		MoveDirection = MoveDirection.Normalized();
	}
	public override void _PhysicsProcess(double delta)
	{

		Velocity = MoveDirection * Speed;
		if(Velocity.X < 0)
		{
			_animatedSprite.FlipH = true;
		}


		var collisionInfo = MoveAndCollide(Velocity * (float) delta);
		if(collisionInfo != null && collisionInfo.GetCollider() is TileMapLayer)
		{
				GD.Print("Collided with the farm!");
				EmitSignal(SignalName.CollidedWithFarm);
				_collisionShape.Disabled = true;
				
		}
		
	}
}
