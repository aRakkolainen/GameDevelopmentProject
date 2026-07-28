using Godot;
using System;
using System.Numerics;
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
	Timer sfxTimer;
	
	private const int tileSize = 32;
	private const int pushBackwardTiles = 1;


	public bool IsInitialized { get; private set; }

	private bool isInRangeOfDistractionItemWithSound = false;

	private Godot.Vector2? directionBeforeNoise = null;

	private float SpeedBeforeDefenseItem = 0;

	private Godot.Vector2 ElephantMoveDirectionBeforeDefenseItem;
    private bool _shouldBounce;
    private Vector2I eaten_fruit_coordinates;
    private bool eaten_chili;
    private bool elephant_was_surprised;
    private float speedAfterSurprise;
    private float speedAfterEating;
    private bool smelled_unpleasant_item;
    private bool currentlyInNoiseEffect = false;

	private SoundEffectPlayer soundEffectPlayer;


    [Signal] public delegate void ElephantSoundEffectStartedEventHandler(string starter, string effect);

    public void Initialize()
	{
    	IsInitialized = true;
	}

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_animatedSprite.Play("walk");
		soundEffectPlayer = GetNode<SoundEffectPlayer>("SoundEffectPlayer");
		OnSoundEffectStarted("elephant", "walk", 20);

		MoveDirection = MoveDirection.Normalized();
		_animatedSprite.AnimationFinished += OnEatingFinished;
		
	}
	public override void _PhysicsProcess(double delta)
	{
		var random = new RandomNumberGenerator();
        random.Randomize();
		Position += MoveDirection * Speed;
		
		if(smelled_unpleasant_item)
		{
			OnSoundEffectStarted("elephant", "frutrated_walk", 120);
		}
		if (_animatedSprite != null)
            _animatedSprite.FlipH = MoveDirection.X < 0f;

        /* This logic below is based on recommendations from Microsoft Copilot with the following two prompts
        Prompt 1: My current code is this and the elephant still appears to stop: public override void _PhysicsProcess(double delta)
	{
		Position += MoveDirection * Speed;
			if (MoveDirection.Equals(Godot.Vector2I.Left)){
				_animatedSprite.FlipH = true;
			}

		if (farm.GetActiveNoiseMaker() != null)
		{
			DistractionItem noise_maker = farm.GetActiveNoiseMaker();
			Vector2I elephant_position_local_to_map = farm.LocalToMap(Position);
			
			int distance_x = Math.Abs(elephant_position_local_to_map.X - noise_maker.GetCoordinates().X);
        	int distance_y = Math.Abs(elephant_position_local_to_map.Y - noise_maker.GetCoordinates().Y);
			if (distance_x <= noise_maker.GetEffectRange())
			{
				MoveDirection = -MoveDirection;
				//Position += MoveDirection * Speed;
			}
		} 
	}
		Prompt 2: What if I want to make the elephant turn back when the noise maker's effect has ended? 
		*/
		if (_debounceTimer > 0.0)
            _debounceTimer -= delta;

		if (farm.GetActiveDistractionItemWithSound() != null)
		{
			DistractionItem distraction_item_with_sound = farm.GetActiveDistractionItemWithSound();
			Vector2I elephant_position_local_to_map = farm.LocalToMap(Position);
			
			int distance_x = Math.Abs(elephant_position_local_to_map.X - distraction_item_with_sound.GetCoordinates().X);

			currentlyInNoiseEffect = distance_x < distraction_item_with_sound.GetEffectRange();
			if (_debounceTimer <= 0.0) 
			{
				
			if (currentlyInNoiseEffect && !isInRangeOfDistractionItemWithSound)
			{
				directionBeforeNoise = MoveDirection;
				MoveDirection = -MoveDirection;
				PauseMovementAndPlayAnimation("walk scared");
				PlaySoundEffect("afraid_elephant", 10);
				isInRangeOfDistractionItemWithSound = currentlyInNoiseEffect;
				_debounceTimer = BoundaryDebounceSeconds;
			} else if (!currentlyInNoiseEffect && !isInRangeOfDistractionItemWithSound)
			{
				if (directionBeforeNoise.HasValue)
				{
					MoveDirection = directionBeforeNoise.Value;
					directionBeforeNoise = null; 
					PlaySoundEffect("walk", 10);
					isInRangeOfDistractionItemWithSound = false;
					_debounceTimer = BoundaryDebounceSeconds;
				}
			}
			}
		} 

	}


	public void OnBodyShapeEntered(Godot.Rid body_rid, Node2D body, int body_shape_index, int local_shape_index)
	{
		if (body is not TileMapLayer tileMap)
		{
			return;
		}
		if (!IsInitialized)
		{
    		return;
		}
		Godot.Vector2I tileCoords = tileMap.GetCoordsForBodyRid(body_rid);
		int sourceId = tileMap.GetCellSourceId(tileCoords);
		if(sourceId == -1)
		{
			return;
		} 
		if (sourceId == 0)
		{
			GD.Print("Elephant collided with farm!");
			EmitSignal(SignalName.CollidedWithFarm, tileCoords, this);
		} else if (sourceId == 1)
		{
			GD.Print("Elephant collided with distraction item!");
			EmitSignal(SignalName.CollidedWithItem, tileCoords, "distraction");
		}else if (sourceId == 2)
		{
			ElephantMoveDirectionBeforeDefenseItem = MoveDirection;
			EmitSignal(SignalName.CollidedWithItem, tileCoords, "defense", this);
		} else if (sourceId == 4)
		{
			Speed = 0; 
			
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
		PushAwayAfterAttack();
        Timer timer = new Timer
        {
            WaitTime = 1.5,
			Autostart = true,
			OneShot = true,
        };
		if(ElephantAttackedDefenseItemCount < 2)
		{
			AddChild(timer);
        	ElephantAttackedDefenseItemCount++;
			timer.Timeout += FlipBack;
		}
		
		
	}

	public void PushAwayAfterAttack()
	{
		MoveDirection = -ElephantMoveDirectionBeforeDefenseItem;
	}

	public void PushAway()
	{
		MoveDirection = -MoveDirection;
	}

	public bool GetElephantCollidedWithPuddle()
	{
		return elephant_collided_with_puddle;
	}

	public bool GetElephantEatenChili()
	{
		return eaten_chili;
	}

	public void SetElephantEatenChili(bool eaten)
	{
		eaten_chili = eaten;
	}

	public bool GetElephantSmelledUnpleasantItem()
	{
		return smelled_unpleasant_item;
	}

	public void SetElephantSmelledUnpleasantItem(bool smelled)
	{
		smelled_unpleasant_item = smelled;
	}


	public bool GetElephantCollidedWithMudPuddle()
	{
		return elephant_collided_with_mud_puddle;
	}

	public void SetElephantCollidedWithMudPuddle(bool collided)
	{
		elephant_collided_with_mud_puddle = collided;
	}

	public int GetElephantAttackedDefenseItemCount()
	{
		return ElephantAttackedDefenseItemCount;
	}

	public void SetElephantAttackedDefenseItemCount(int num)
	{
		ElephantAttackedDefenseItemCount = num;
	}


    private void FlipBack()
    {
		MoveDirection = ElephantMoveDirectionBeforeDefenseItem;
    }


	public void PauseMovementAndPlayAnimation(string animation_name)
	{
		SetPhysicsProcess(false);
		_animatedSprite.Play(animation_name);
		if (animation_name.Contains("chili"))
		{
			eaten_chili = true;
			speedAfterEating = 2 * Speed;
			PlaySoundEffect("afraid_elephant",2);
		} else if (animation_name.Contains("smell"))
		{
			PlaySoundEffect("afraid_elephant", 2);
		} else if (animation_name.Contains("scared"))
		{
			elephant_was_surprised = true;
			speedAfterSurprise = 1.5f * Speed;
			
		}
	}

    private void OnEatingFinished()
    {
        SetPhysicsProcess(true);
		if (eaten_chili)
		{
			Speed = speedAfterEating;
			_animatedSprite.Play("walk annoyed");
		} else if (smelled_unpleasant_item)
		{
			_animatedSprite.Play("walk annoyed");
			PlaySoundEffect("frustrated_walk", 120);

		} else if (isInRangeOfDistractionItemWithSound && currentlyInNoiseEffect)
		{
			Speed = speedAfterSurprise;
			_animatedSprite.Play("walk annoyed");
			PlaySoundEffect("frustrated_walk", 120);
		}
			else
		{
			_animatedSprite.Play("walk");
			farm.PlaceElephantPoop(eaten_fruit_coordinates);
		}
    }

	public void OnSoundEffectStarted(string starter, string effect, int duration)
	{
		if (effect.Equals("walk"))
		{
			EmitSignal(SignalName.ElephantSoundEffectStarted, starter, effect, duration);
		} else
		{
			EmitSignal(SignalName.ElephantSoundEffectStarted, starter, effect, duration);
		}
	}

    internal void PlaySoundEffect(string effect, int duration)
    {
        EmitSignal(SignalName.ElephantSoundEffectStarted, "elephant", effect, duration);
    }
}
