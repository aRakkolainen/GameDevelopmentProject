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
	
	[Signal] public delegate void CollidedWithFarmEventHandler(Vector2I elephant_pos);

	uint originalMask;

	private bool hasCollidedWithFarmBefore = false;

	private int elephant_detection_area = 2;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_animatedSprite.Play("walk");
		MoveDirection = MoveDirection.Normalized();
		originalMask = CollisionMask;
	}
	public override void _PhysicsProcess(double delta)
	{
		//Velocity = Vector2.Zero;
		//Velocity = MoveDirection * Speed;
		//if(Velocity.X < 0)
		//{
		Position += MoveDirection * Speed;
		if (MoveDirection.Equals(Godot.Vector2I.Left)){
			_animatedSprite.FlipH = true;
		}
		//}


		//var collisionInfo = MoveAndCollide(Velocity * (float) delta);
		/* if(collisionInfo != null && !hasCollidedWithFarmBefore && collisionInfo.GetCollider() is FarmManager)
		{
			Rid collisionRid =  collisionInfo.GetColliderRid();
			uint layer = GetCollisionLayerFromRid(collisionRid);

			FarmManager collidedFarmManager = (FarmManager)collisionInfo.GetCollider();
			Godot.Collections.Array<Vector2I> farm_tiles = collidedFarmManager.GetFarmTileCoordinates();
			Vector2I elephant_map_pos = collidedFarmManager.LocalToMap(Position);
			if ((layer & (1u << 1)) != 0) // //Copilot generated check that it is on layer 2
			{
				GD.Print(elephant_map_pos);
				CollisionMask &= ~(1u << 1);
				hasCollidedWithFarmBefore = true;
				GD.Print("Elephant collided with farm!");
				EmitSignal(SignalName.CollidedWithFarm, elephant_map_pos);
			}
			} else if ((layer & (1u << 2)) != 0) //Copilot generated!
			{
				Godot.Collections.Array<Vector2I> tiles_with_items = collidedFarmManager.GetTilesWithItemsCoordinates();
				if (tiles_with_items.Contains(elephant_map_pos) || tiles_with_items.Contains(new Godot.Vector2I(elephant_map_pos.X+1, elephant_map_pos.Y)) ||  tiles_with_items.Contains(new Godot.Vector2I(elephant_map_pos.X-1, elephant_map_pos.Y)))
				{
					GD.Print("Elephant collided with item!");
				
				}
			}
				
		} */
	}

	public void OnBodyEntered(Node2D collided_body)
	{
		GD.Print("You collided with farm!", collided_body);
		if (collided_body is not TileMapLayer)
		{
			
		Node layer = collided_body.GetParent();
		while (layer != null && layer is not TileMapLayer)
		{
			layer = layer.GetParent();
		}
		if (layer is not TileMapLayer)
		{
			return;
		}
		}
		TileMapLayer farmTilesLayer = (TileMapLayer) collided_body;
		Godot.Vector2 localPos = farmTilesLayer.ToLocal(GlobalPosition);
		Godot.Vector2I collidedCell = farmTilesLayer.LocalToMap(localPos);
		GD.Print("Elephant hit the cell: ", collidedCell);
		EmitSignal(SignalName.CollidedWithFarm, collidedCell);
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

//Based on Copilot generated code		
private static uint GetCollisionLayerFromRid(Godot.Rid rid)
    {
        // In many cases (StaticBody2D, TileMap-generated bodies, etc.) the RID is a body
        try
        {
            return PhysicsServer2D.BodyGetCollisionLayer(rid);
        }
        catch
        {
            // If it's actually an Area2D RID
            try
            {
                return PhysicsServer2D.AreaGetCollisionLayer(rid);
            }
            catch
            {
                // Unknown/unsupported RID type
                return 0u;
            }
        }
    }

}
