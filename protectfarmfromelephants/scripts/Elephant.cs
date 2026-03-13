using Godot;
using System;
using System.Linq.Expressions;
using System.Numerics;

public partial class Elephant : Area2D
{
	[Export] public float Speed = 1.0f;

	[Export] public Godot.Vector2 MoveDirection = Godot.Vector2.Right;
	private AnimatedSprite2D _animatedSprite;

	private CollisionShape2D _collisionShape;

	private bool firstCollisionWithFarm = false;
	
	[Signal] public delegate void CollidedWithFarmEventHandler(Vector2I tileCoords);

	[Signal] public delegate void CollidedWithItemEventHandler(Vector2I tileCoords, string itemType, Elephant elephant);

	uint originalMask;

	private bool hasCollidedWithFarmBefore = false;

	private int elephant_detection_area = 2;

	
	private const int tileSize = 32;
	private const int pushBackwardTiles = 1;


	public bool IsInitialized { get; private set; }

	public void Initialize()
	{
    	IsInitialized = true;
	}

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_animatedSprite.Play("walk");
		MoveDirection = MoveDirection.Normalized();
		
	}
	public override void _PhysicsProcess(double delta)
	{
		Position += MoveDirection * Speed;
		if (MoveDirection.Equals(Godot.Vector2I.Left)){
			_animatedSprite.FlipH = true;
		}
	}


	public async void OnBodyShapeEntered(Godot.Rid body_rid, Node2D body, int body_shape_index, int local_shape_index)
	{
		GD.Print("Entered body", body);
		if (body is not TileMapLayer tileMap)
		{
			return;
		}
		if (!IsInitialized)
		{
    		return;
		}
		Godot.Vector2I tileCoords = tileMap.GetCoordsForBodyRid(body_rid);
		GD.Print(tileCoords);
		int sourceId = tileMap.GetCellSourceId(tileCoords);
		if(sourceId == -1)
		{
			return;
		} 
		if (sourceId == 0)
		{
			GD.Print("Elephant collided with farm!");
			EmitSignal(SignalName.CollidedWithFarm, tileCoords);
		} else if (sourceId == 1)
		{
			GD.Print("Elephant collided with distraction item!");
			EmitSignal(SignalName.CollidedWithItem, tileCoords, "distraction");
		}else if (sourceId == 2)
		{
			Speed = 0;
			GD.Print("Elephant collided with defense item!");
			GD.Print("Emitting CollidedWithItem");
			EmitSignal(SignalName.CollidedWithItem, tileCoords, "defense", this);
		}

	}

	public void OnPushBack()
	{
		Speed = 1.0f;
			//Code created with assistance of Copilot
			if (MoveDirection.Equals(Godot.Vector2I.Left)){
				float direction = Vector2I.Right.X * pushBackwardTiles;
				GlobalPosition = new Godot.Vector2(GlobalPosition.X + direction, GlobalPosition.Y);
				MoveDirection = Vector2I.Right;
				Position += MoveDirection * Speed;
			} else
		{
			float direction = Vector2I.Left.X * pushBackwardTiles;
			GlobalPosition = new Godot.Vector2(GlobalPosition.X + direction, GlobalPosition.Y);	
			MoveDirection = Vector2I.Left;
			Position += MoveDirection * Speed;
		}
	}

	private bool CheckIfCloseToFarmTiles(Godot.Collections.Array<Vector2I> farm_tiles, Vector2I collisionPosition)
	{
		if (farm_tiles.Contains(collisionPosition)){
			return true;
		} else if (farm_tiles.Contains(new Vector2I(collisionPosition.X+elephant_detection_area, collisionPosition.Y + elephant_detection_area))) 
		{
			return true;
		} else if (farm_tiles.Contains(new Vector2I(collisionPosition.X+elephant_detection_area, collisionPosition.Y - elephant_detection_area)))
		{
			return true;
		} else if (farm_tiles.Contains(new Vector2I(collisionPosition.X-elephant_detection_area, collisionPosition.Y + elephant_detection_area)))
		{
			return true;
		}  else if (farm_tiles.Contains(new Vector2I(collisionPosition.X-elephant_detection_area, collisionPosition.Y - elephant_detection_area)))
		{
			return true;
		} else
		{
			return false;
		}

	}



}
