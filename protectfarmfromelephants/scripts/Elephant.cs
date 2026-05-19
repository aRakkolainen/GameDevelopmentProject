using Godot;
using System;
namespace ProtectFarm;

public partial class Elephant : Area2D
{
	[Export] public float Speed = 1.0f;

	[Export] public Godot.Vector2 MoveDirection = Godot.Vector2.Right;

	[Export] public FarmManager farm;
	private AnimatedSprite2D _animatedSprite;

	private CollisionShape2D _collisionShape;

	private bool firstCollisionWithFarm = false;
	
	[Signal] public delegate void CollidedWithFarmEventHandler(Vector2I tileCoords, Elephant elephant);

	[Signal] public delegate void CollidedWithItemEventHandler(Vector2I tileCoords, string itemType, Elephant elephant);

	

	uint originalMask;

	private bool hasCollidedWithFarmBefore = false;

	private int elephant_detection_area = 2;

	private int ElephantAttackedDefenseItemCount = 0;

	
	[Export] public float BoundaryDebounceSeconds = 0.1f;
    private double _debounceTimer = 0.0;

	private bool elephant_collided_with_puddle = false;

	private bool elephant_collided_with_mud_puddle = false;


	
	private const int tileSize = 32;
	private const int pushBackwardTiles = 1;


	public bool IsInitialized { get; private set; }

	private bool isInRangeOfNoiseMaker = false;

	private bool isInRangeOfCampfire = false;

	private Godot.Vector2? directionBeforeNoise = null;

	private Godot.Vector2? directionBeforeCampfire = null;

	private float SpeedBeforeDefenseItem = 0;
    private bool _shouldBounce;
    private Vector2I eaten_fruit_coordinates;
    private bool eaten_chili;
    private float speedAfterEating;
    private bool smelled_sunflower;

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
		_animatedSprite.AnimationFinished += OnEatingFinished;
		
	}
	public override void _PhysicsProcess(double delta)
	{
		var random = new RandomNumberGenerator();
        random.Randomize();
		Position += MoveDirection * Speed;
		
		if (_animatedSprite != null)
            _animatedSprite.FlipH = MoveDirection.X < 0f;

        // 3) Debounce timer (optional but helps near the edge)
        if (_debounceTimer > 0.0)
            _debounceTimer -= delta;

		
		bool currentlyInNoiseEffect = false;

		if (farm.GetActiveNoiseMaker() != null)
		{
			DistractionItem noise_maker = farm.GetActiveNoiseMaker();
			Vector2I elephant_position_local_to_map = farm.LocalToMap(Position);
			
			int distance_x = Math.Abs(elephant_position_local_to_map.X - noise_maker.GetCoordinates().X);
        	int distance_y = Math.Abs(elephant_position_local_to_map.Y - noise_maker.GetCoordinates().Y);

			currentlyInNoiseEffect = distance_x < noise_maker.GetEffectRange();
			if (_debounceTimer <= 0.0)
			{
				
			if (currentlyInNoiseEffect && !isInRangeOfNoiseMaker)
			{
				directionBeforeNoise = MoveDirection;
				MoveDirection = -MoveDirection;
				isInRangeOfNoiseMaker = currentlyInNoiseEffect;
				_debounceTimer = BoundaryDebounceSeconds;
			} else if (!currentlyInNoiseEffect && !isInRangeOfNoiseMaker)
			{
				if (directionBeforeNoise.HasValue)
				{
					MoveDirection = directionBeforeNoise.Value;
					directionBeforeNoise = null; 
					isInRangeOfNoiseMaker = false;
					_debounceTimer = BoundaryDebounceSeconds;
				}
			}
			}
		} 

		bool currentlyInCampfireEffect = false;
		if (farm.GetActiveCampfire() != null)
		{
			DistractionItem camp_fire = farm.GetActiveCampfire();
			Vector2I elephant_position_local_to_map = farm.LocalToMap(Position);
			
			int distance_x = Math.Abs(elephant_position_local_to_map.X - camp_fire.GetCoordinates().X);
        	int distance_y = Math.Abs(elephant_position_local_to_map.Y - camp_fire.GetCoordinates().Y);

			currentlyInCampfireEffect = distance_x < camp_fire.GetEffectRange();
			if (currentlyInCampfireEffect && !isInRangeOfCampfire)
			{
				directionBeforeCampfire = MoveDirection;
				MoveDirection = -MoveDirection;
				Speed *= (float)1.5;
				isInRangeOfCampfire = currentlyInCampfireEffect;
			} else if(!currentlyInCampfireEffect && !isInRangeOfCampfire && directionBeforeCampfire.HasValue)
			{
				MoveDirection = directionBeforeCampfire.Value;
				directionBeforeCampfire = null; 
				isInRangeOfCampfire = false; 
			}

			
		}
	}


	public void OnBodyShapeEntered(Godot.Rid body_rid, Node2D body, int body_shape_index, int local_shape_index)
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
			if (ElephantAttackedDefenseItemCount <= 3)
			{
				EmitSignal(SignalName.CollidedWithFarm, tileCoords, this);
			}
		} else if (sourceId == 1)
		{
			GD.Print("Elephant collided with distraction item!");
			EmitSignal(SignalName.CollidedWithItem, tileCoords, "distraction");
		}else if (sourceId == 2)
		{
			_shouldBounce = true;

			EmitSignal(SignalName.CollidedWithItem, tileCoords, "defense", this);
		} else if (sourceId == 4)
		{
			Speed = 0; 
			GD.Print("Collided with border!");
			QueueFree();
			
		} else if (sourceId == 5)
		{
			Vector2I atlasCoords = tileMap.GetCellAtlasCoords(tileCoords);
			if (atlasCoords == new Vector2I(1, 1))
			{
				elephant_collided_with_puddle = true;
				EmitSignal(SignalName.CollidedWithItem, tileCoords, "puddle", this);
			} else if (atlasCoords == new Vector2I(2,1))
			{
				elephant_collided_with_mud_puddle = true;
				EmitSignal(SignalName.CollidedWithItem, tileCoords, "puddle", this);
			} else
			{
				eaten_fruit_coordinates = tileCoords;
				EmitSignal(SignalName.CollidedWithItem, tileCoords, "dropped_plant", this);
			}

		}

	}

	public void OnPushBack()
	{
		MoveDirection = -MoveDirection;

        Timer timer = new Timer
        {
            WaitTime = 1.5,
			Autostart = true,
			OneShot = true,
        };
		AddChild(timer);
        timer.Timeout += FlipBack;
		
	}

	public bool GetElephantCollidedWithPuddle()
	{
		return elephant_collided_with_puddle;
	}

	public bool GetElephantCollidedWithMudPuddle()
	{
		return elephant_collided_with_mud_puddle;
	}

	public void SetElephantCollidedWithMudPuddle(bool collided)
	{
		elephant_collided_with_mud_puddle = collided;
	}

    private void FlipBack()
    {
		MoveDirection = -MoveDirection;
    }


	public void PauseMovementAndPlayAnimation(string animation_name)
	{
		SetPhysicsProcess(false);
		_animatedSprite.Play(animation_name);
		if (animation_name.Contains("chili"))
		{
			eaten_chili = true;
			speedAfterEating = 2 * Speed;
		} else if (animation_name.Contains("sunflower"))
		{
			smelled_sunflower = true;
		}
	}

    private void OnEatingFinished()
    {
        SetPhysicsProcess(true);
		if (eaten_chili)
		{
			Speed = speedAfterEating;
			var poop_roll = GD.Randf();
			if (poop_roll > 0.60f)
			{
				farm.PlaceElephantPoop(eaten_fruit_coordinates);
			}

			_animatedSprite.Play("walk");
		} else if (smelled_sunflower)
		{
			_animatedSprite.Play("walk annoyed");
		} else
		{
			_animatedSprite.Play("walk");
			farm.PlaceElephantPoop(eaten_fruit_coordinates);
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
