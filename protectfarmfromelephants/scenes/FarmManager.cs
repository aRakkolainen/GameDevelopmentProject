using Godot;
using ProtectFarm;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Numerics;
//Source for this was this YouTube tutorial: https://www.youtube.com/watch?v=4qEOdviP1yA
public partial class FarmManager : TileMapLayer
{
	private int default_plant_phase = 1; 

	private int farm_source_id = 0;

	private int water_lake_id = 3;

	private int usable_distraction_items_id = 1;
	private int usable_defense_items_id = 2;

	private Godot.Vector2I default_tile_atlas_coords = new Godot.Vector2I(0, 0);
	private Godot.Collections.Array<Vector2I> farm_tile_coordinates;

	private Godot.Collections.Array<Vector2I> water_tile_coordinates;

	private List<Plant> plants;

	private List<DefenseItem> placed_defense_items = new List<DefenseItem>();

	private List<DistractionItem> placed_distraction_items = new List<DistractionItem>();

	private Dictionary<string, Dictionary<int, Vector2I>> plant_growth_phases_by_type;

	private Dictionary<String, Dictionary<string, Vector2I>> upgrade_items_by_name;

	private string[] plant_types = {"pineapple", "watermelon"};

	private int active_level; 

	private string plant_type;

	private int number_of_seeds_in_player_inventory = 0;
	private LevelData level_data;

	private LevelManager level_manager;
	private bool seeds_clicked = false; 

	private bool watering_can_clicked = false;

	private bool defense_item_clicked = false;

	private bool distraction_item_clicked = false;

	private string selected_defense_item; 

	private string selected_distraction_item; 

	private DistractionItem active_noise_maker; 
	private int water_level = 0;

	private AudioStreamPlayer2D distractionAudioPlayer;
	[Export] Player _player;

	[Export] TimeManager timer;

	[Export] SimpleInventory _inventory;
    private Godot.Collections.Array<Vector2I> tiles_with_items_coordinates;

    [Signal] public delegate void UpdatedSeedCountEventHandler(int quantity, string update_type);

	[Signal] public delegate void SeedPlacedEventHandler(bool success);

	[Signal] public delegate void UpdatedItemCountEventHandler(string item_name, int quantity, string update_type);

	[Signal]
	public delegate void UpdatedWateringcanTextEventHandler();

	[Signal]
	public delegate void UpdatedInfoTextEventHandler(string message);

	[Signal] public delegate void PlayerTriedToPlaceDefenseItemEventHandler();

	[Signal] public delegate void PlayerTriedToPlaceDistractionItemEventHandler();

	[Signal] public delegate void CollidedWithFarmEventHandler(Vector2I tileCoords);

	[Signal] public delegate void CollidedWithItemEventHandler(Vector2I tileCoords, string itemType, Elephant elephant);

	[Signal] public delegate void SentElephantBackEventHandler(Vector2I tileCoords);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		plants = new List<Plant>();
		level_manager = LevelManager.Instance;
		farm_tile_coordinates = GetUsedCellsById(farm_source_id);
		water_tile_coordinates = GetUsedCellsById(water_lake_id);
		initializePlantTypesAndPhases();
		initializeUpgradeItems();
		active_level = level_manager.GetCurrentActiveLevel();
		level_data = level_manager.GetLevelData(active_level);
		if(level_data != null)
		{
			plant_type = level_data.GetPlantType();
		} else
		{
			plant_type = "";
		}
		distractionAudioPlayer = GetNode<AudioStreamPlayer2D>("DistractionAudioPlayer");
		_inventory = GetNode<SimpleInventory>("%SimpleInventory");
		_player ??= GetNode<Player>("%Player");
		CollidedWithFarm += OnElephantCollidedWithFarm;
        CollidedWithItem += OnElephantCollidedWithItem;
		//Connect(Player.SignalName.PlayerTriedToPlaceDefenseItem, new Callable(this, nameof(OnPlayerTriedToPlaceDefenseItem)));
		//Connect(Player.SignalName.PlayerTriedToPlaceDistractionItem, new Callable(this, nameof(OnPlayerTriedToPlaceDistractionItem)));
		//Connect(Elephant.SignalName.CollidedWithFarm, new Callable(this, nameof(DestroyPlants)));

	}


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
    {
        var random = new RandomNumberGenerator();
        random.Randomize();
        if (!_player.GetPlayerIsAlive())
        {
            return;
        } else
		{
        	ListenForPlayerInteractionsWithFarm();
		}

        
    }

	public DistractionItem GetActiveNoiseMaker()
	{
		return active_noise_maker;
	}

    private void ListenForPlayerInteractionsWithFarm()
    {
        Godot.Vector2 player_pos = _player.Position;
        Vector2I player_local_map_pos = LocalToMap(player_pos);

		//Checking if player is trying to collect water
        if (water_tile_coordinates != null && water_tile_coordinates.Count > 0 && Input.IsActionJustPressed("mouse_left_click") && watering_can_clicked)
        {
            Godot.Vector2 mousePos = GetLocalMousePosition();
            Vector2I mouse_map_pos = LocalToMap(mousePos);

            bool canInteract = IsPlayerCloseEnough(mouse_map_pos, player_local_map_pos);
            if (!canInteract)
            {
                return;
            }
            if (water_tile_coordinates.IndexOf(mouse_map_pos) != -1)
            {
                CollectWater();
                GD.Print("Water collected!");
            }
        }
		// checking if player is trying to plant or pick up plants
        if (farm_tile_coordinates != null && farm_tile_coordinates.Count > 0 && Input.IsActionJustPressed("mouse_right_click") && _player.GetPlayerIsAlive())
        {
            number_of_seeds_in_player_inventory = _inventory.GetNumberOfSeedsInInventory();
            Godot.Vector2 mousePos = GetLocalMousePosition();
            Vector2I mouse_map_pos = LocalToMap(mousePos);
            //Vector2I atlas_coords = GetCellAtlasCoords(mouse_map_pos);

			bool canInteract = IsPlayerCloseEnough(mouse_map_pos, player_local_map_pos);
            if (!canInteract)
            {
                return;
            }
			
			//Checking if there already is a plant in clicked coordinates.

           
            if (farm_tile_coordinates.IndexOf(mouse_map_pos) == -1)
            {
                GD.Print("You cannot plant here!");
				UpdateInfoText("You are trying to plant outside the farm tiles!");
            }
            else
                {
					int plant_index = CheckIfAlreadyPlanted(mouse_map_pos);
					if (seeds_clicked)
					{
						if (plant_index == -1)
					{
						PlacePlant(mouse_map_pos);
					}	else
					{
						GD.Print("Already planted!");
					}
					}
					if (plants.Count > 0 && plant_index != -1)
				{
					
					PickUpPlant(mouse_map_pos, plant_index);
				}
					
                }

            if (watering_can_clicked)
            {
                WaterPlant(mouse_map_pos);
            }

			if (plants.Count == 0 && (defense_item_clicked || distraction_item_clicked))
			{
				GD.Print("You have to plant at least one seed before you can protect them!");
				UpdateInfoText("You have to plant at least one seed before you can protect them!");
			}

            //Checking if there already are plants in this farm:
            if (plants != null && plants.Count > 0)
            {
                if (defense_item_clicked)
                {
                    GD.Print("Trying to place defense item ");
                    PlaceDefensiveItem(mouse_map_pos);
                }

                if (distraction_item_clicked)
                {
                    GD.Print("Trying to place distraction item ");
                    PlaceDistractionItem(mouse_map_pos);
					
                }


            }
        }

    }

    private void ActivateNoiseMaker(DistractionItem noise_maker)
    {
        GD.Print(noise_maker.GetEffectDuration());
		if (active_noise_maker == null)
		{
			Timer distractionDurationTimer = GetNode<Timer>("DistractionDurationTimer");
			distractionDurationTimer.WaitTime = noise_maker.GetEffectDuration();
			distractionDurationTimer.Start();
			distractionAudioPlayer.Play();
			active_noise_maker = noise_maker;
		} else
		{
			GD.Print("Only one noise maker can be active at once!");
		}
    }

	private void OnDistractionDurationTimerTimeout()
	{
		distractionAudioPlayer.Stop();
		if (active_noise_maker != null)
		{
			DestroyItemAtCoordinates("distraction", active_noise_maker.GetCoordinates());
			active_noise_maker = null;
			
		}
	}


    private static bool IsPlayerCloseEnough(Vector2I mouse_map_pos, Vector2I player_local_map_pos)
    {
        int distance_x = Math.Abs(mouse_map_pos.X - player_local_map_pos.X);
        int distance_y = Math.Abs(mouse_map_pos.Y - player_local_map_pos.Y);
        if (distance_x > 1 || distance_y > 1)
        {
            return false;
        }

        return true;
    }


    private int CheckIfAlreadyPlanted(Vector2I mouse_map_pos)
    {
        int index = FindPlantAtCoordinates(mouse_map_pos);

        return index;
    }

	private void UpdateInfoText(string message)
	{
		EmitSignal(SignalName.UpdatedInfoText, message);
	}
    private void PickUpPlant(Vector2I mouse_map_pos, int index)
    {
		string infoMessage = "";
        if (plants.Count > 0 && plants[index].GetGrowthPhase() == 4)
        {
            GD.Print("Your plant is fully grown!");
			infoMessage = "Your plant is fully grown!";
            int inventory_size = _inventory.GetChildCount();
            InventoryItem item = new InventoryItem(inventory_size + 1, plants[index].GetPlantType(), "fruit", 1, 32);
            _player.AddToInventory(item);
            RemovePlantAtCoordinates(mouse_map_pos);
        }
        else
        {
			infoMessage = "Your plant is not ready yet!";
            GD.Print("Your plant is not ready yet!");
        }
		UpdateInfoText(infoMessage);
    }


    private void PlaceDefensiveItem(Vector2I mouse_map_pos)
    {
		Dictionary<string, Vector2I> defenses = upgrade_items_by_name.GetValueOrDefault("defense");
		string infoMessage = "";
		if (farm_tile_coordinates.IndexOf(mouse_map_pos) != -1 || water_tile_coordinates.IndexOf(mouse_map_pos) != -1)
		{
			infoMessage = "Cannot place item at farm or water tiles!";
			GD.Print("Cannot place item at farm or water!");
		} else
		{
		if (_inventory != null && _inventory.GetItemQuantityInInvetory(selected_defense_item) > 0)
			{
        if (selected_defense_item.Equals("fence"))
		{
			Vector2I fence_tile = defenses.GetValueOrDefault("fence");
			placed_defense_items.Add(new DefenseItem(placed_defense_items.Count+1, selected_defense_item, true, 2, mouse_map_pos));
			SetCell(mouse_map_pos, usable_defense_items_id, fence_tile);
			EmitSignal(SignalName.UpdatedItemCount, selected_defense_item, 1, "decrease");
		} else if (selected_defense_item.Equals("stone_wall"))
		{
			Vector2I stonewall_tile = defenses.GetValueOrDefault("stone_wall");
			placed_defense_items.Add(new DefenseItem(placed_defense_items.Count+1, selected_defense_item, true, 4, mouse_map_pos));
			SetCell(mouse_map_pos, usable_defense_items_id, stonewall_tile);
			EmitSignal(SignalName.UpdatedItemCount, selected_defense_item, 1, "decrease");
		} else
			{
				GD.Print("Unknown item type!");
				return;
			} 
				
			}
		}
			UpdateInfoText(infoMessage);
    }

	 private void PlaceDistractionItem(Vector2I mouse_map_pos)
    {
		Dictionary<string, Vector2I> distractions = upgrade_items_by_name.GetValueOrDefault("distraction");
		if (farm_tile_coordinates.Contains(mouse_map_pos) || water_tile_coordinates.Contains(mouse_map_pos))
		{
			UpdateInfoText("Cannot place item at farm or water!");
			GD.Print("Cannot place item at farm or water!");
		} else
		{
			if (_inventory != null && _inventory.GetItemQuantityInInvetory(selected_distraction_item) > 0)
			{
				
        	if (selected_distraction_item.Equals("camp_fire"))
			{
				Vector2I item_tile = distractions.GetValueOrDefault("camp_fire");
				placed_distraction_items.Add(new DistractionItem(placed_distraction_items.Count+1, selected_distraction_item, true, 4, 3, true, 2, mouse_map_pos));
				SetCell(mouse_map_pos, usable_distraction_items_id, item_tile);
				EmitSignal(SignalName.UpdatedItemCount, selected_distraction_item, 1, "decrease");
			} else if (selected_distraction_item.Equals("noise_maker"))
			{
				Vector2I noise_maker_tile = distractions.GetValueOrDefault("noise_maker");
				DistractionItem noise_maker = new DistractionItem(placed_distraction_items.Count+1, selected_distraction_item, true, 25, 5, false, 0, mouse_map_pos);
				if (active_noise_maker == null)
					{
						placed_distraction_items.Add(noise_maker);
						SetCell(mouse_map_pos, usable_distraction_items_id, noise_maker_tile);
						ActivateNoiseMaker(noise_maker);
						EmitSignal(SignalName.UpdatedItemCount, selected_distraction_item, 1, "decrease");
				}
			} else if (selected_distraction_item.Equals("beehive"))
			{
				Vector2I beehive_tile = distractions.GetValueOrDefault("beehive");
				placed_distraction_items.Add(new DistractionItem(placed_distraction_items.Count+1, selected_distraction_item, true, 10, 2, true, 5, mouse_map_pos));
				SetCell(mouse_map_pos, usable_distraction_items_id, beehive_tile);
				EmitSignal(SignalName.UpdatedItemCount, selected_distraction_item, 1, "decrease");
			} else
				{
					GD.Print("Unknown item type!");
				}
			} 
		}
    }

	private void DestroyItemAtCoordinates(string item_type, Vector2I coordinates){
		int index = FindItemAtCoordinates(coordinates, item_type);
		if(index == -1)
		{
			GD.Print("Item not found!");
		} else
		{	
			SetCell(coordinates,farm_source_id, default_tile_atlas_coords);
			if ("defense".Equals(item_type))
		{
			placed_defense_items.RemoveAt(index);
		} else if ("distraction".Equals(item_type))
		{
			placed_distraction_items.RemoveAt(index);
		}
	}
	}

    private int FindItemAtCoordinates(Vector2I coordinates, string item_type)
    {
		if ("defense".Equals(item_type))
		{
			return placed_defense_items.FindIndex(item => item.GetCoordinates() == coordinates);
		} else if ("distraction".Equals(item_type))
		{
			return placed_distraction_items.FindIndex(item => item.GetCoordinates() == coordinates);
		} else
		{
			return -1;
		}
    }


    public void OnPlayerTriedToPlantSeed()
	{
		GD.Print("You are trying to plant seed");
		SelectNewAndResetOtherSelections("seeds");
	}


	public void OnPlayerTriedToWaterPlant()
	{
		GD.Print("You tried to water plant");
		SelectNewAndResetOtherSelections("watering_can");

	} 

	public void OnPlayerTriedToPlaceDefenseItem(string name)
	{
		SelectNewAndResetOtherSelections("defense");
		selected_defense_item = name;
		GD.Print("You tried to place defense item");
	}

	public void OnPlayerTriedToPlaceDistractionItem(string name)
	{
		SelectNewAndResetOtherSelections("distraction");
		selected_distraction_item = name;
		GD.Print("You tried to place distraction item");
	}

	public void SelectNewAndResetOtherSelections(string selected)
	{
		switch (selected)
		{
			case "seeds":
				seeds_clicked = true;
				watering_can_clicked = false;
				defense_item_clicked = false; 
				distraction_item_clicked = false;
				break;
			case "watering_can":
				watering_can_clicked = true;
				seeds_clicked = false;
				defense_item_clicked = false; 
				distraction_item_clicked = false;
				break;
			case "defense":
				defense_item_clicked = true; 
				watering_can_clicked = false;
				seeds_clicked = false;
				distraction_item_clicked = false;
				break;
			case "distraction":
				distraction_item_clicked = true;
				defense_item_clicked = false; 
				watering_can_clicked = false;
				seeds_clicked = false;
				break;
		}
	}


	/*
		Probabilities for how many plants are destroyed in line if multiple
		50% 1 plant
		35% 2 plants
		15% all plants

	*/
	public void OnElephantCollidedWithFarm(Vector2I elephant_position)
	{
		int plantCount = plants.Count;
		if (plantCount > 0 )
		{
			GD.Print(elephant_position);
			int elephant_walking_line = elephant_position.Y;
			List<Plant> plants_on_line = plants.FindAll(plant => plant.GetCoordinates().Y == elephant_position.Y);
			if (plants_on_line.Count > 0)
			{
				var plant_roll = GD.Randf();
				if (plants_on_line.Count == 1)
				{
					if (plant_roll > 0.50f)
					{
						Plant plant_to_be_destroyed = plants_on_line[0];
						RemovePlantAtCoordinates(plant_to_be_destroyed.GetCoordinates());
					}
				} else
				{
					if (plant_roll <= 0.60f)
					{
						DestroyRandomPlant(plants_on_line);
					}
				}
				//50% chance to be destroyed if it is the only plant
				/* if (plants_on_line.Count == 1)
				{
					if (plant_roll > 0.50f)
					{
						Plant plant_to_be_destroyed = plants_on_line[0];
						RemovePlantAtCoordinates(plant_to_be_destroyed.GetCoordinates());
					}
				} else if (plants_on_line.Count > 1)
				{
					if (plant_roll > 0.60f)
					{
						//Common case so one plant is randomly destroyed
                        DestroyRandomPlant(plants_on_line);
					}
					} else if (plant_roll > 0.80f)
					{
						int half_plants = (int) plants_on_line.Count / 2;
						for (int i=0; i < half_plants; i++)
					{
						DestroyRandomPlant(plants_on_line);
					}
					}
					else if (plant_roll < 0.10f)
					{
						foreach(Plant plant in plants_on_line)
						{
							RemovePlantAtCoordinates(plant.GetCoordinates());
						} */
					/* if (plant_roll < 0.50f)
                    {
                        //Common case so one plant is randomly destroyed
                        DestroyRandomPlant(plants_on_line);
                    }
                    else if (plant_roll < 0.30f)
					{
						DestroyRandomPlant(plants_on_line);
						DestroyRandomPlant(plants_on_line);
					} else
					{
						foreach(Plant plant in plants_on_line)
						{
							RemovePlantAtCoordinates(plant.GetCoordinates());
						}
					} */
				}
			}
					
	}

	public void OnElephantCollidedWithItem(Vector2I itemTileCoords, string itemType, Elephant elephant)
	{
		GD.Print("Elephant collided with an item here!");
		int index = FindItemAtCoordinates(itemTileCoords, itemType);
		if (index == -1)
		{
			return;
		}
		if ("distraction".Equals(itemType))
		{
			DistractionItem item = placed_distraction_items[index];
			GD.Print("Elephant collided with distraction item of type:", item.GetType());
			
		} else if ("defense".Equals(itemType))
		{
			DefenseItem item = placed_defense_items[index];
			if (item.GetIsBreakable())
			{
				int currentHealth = item.GetHealth();
				GD.Print("Item: " + item.GetItemName() + " has health left: " + currentHealth);
				if (currentHealth-1 == 0 || currentHealth == 0)
				{
					item.SetHealth(currentHealth-1);
					DestroyItemAtCoordinates(itemType, item.GetCoordinates());
				} else if (currentHealth > 0)
				{
					item.SetHealth(currentHealth-1);
				}
					elephant.OnPushBack();

			}

			
		}
	}

    private void DestroyRandomPlant(List<Plant> plants_on_line)
    {
        	int random_index = GD.RandRange(0, plants_on_line.Count-1);
			if (random_index == -1 || random_index > plants_on_line.Count)
			{
				return;
			} else
			{
        		Plant plant_to_be_destroyed = plants_on_line[random_index];
        		RemovePlantAtCoordinates(plant_to_be_destroyed.GetCoordinates());
				plants_on_line.Remove(plant_to_be_destroyed);
			}
    }

    public Godot.Collections.Array<Vector2I> GetFarmTileCoordinates()
	{
		farm_tile_coordinates ??= GetUsedCellsById(farm_source_id);
		return farm_tile_coordinates;
	}

	public Godot.Collections.Array<Vector2I> GetTilesWithItemsCoordinates()
	{
		tiles_with_items_coordinates ??= GetUsedCellsById(usable_distraction_items_id) + GetUsedCellsById(usable_defense_items_id);
		return tiles_with_items_coordinates;
	}
	private void PlacePlant(Vector2I position)
	{
		level_data = level_manager.GetLevelData(active_level);
		plant_type = level_data.GetPlantType();
		int plantable_tiles = farm_tile_coordinates.Count;
		int id = (int) (GD.Randi() % plantable_tiles);
		if(plant_type == null || plant_type == "")
		{
			GD.Print("Plant type undefined!");
		} else
		{
			int seeds_in_inventory = _inventory.GetNumberOfSeedsInInventory();
			if (seeds_in_inventory > 0)
			{
				Plant newPlant = new Plant(id, plant_type, default_plant_phase, position, false); 
				plants.Add(newPlant);
				SetCell(position, 0, new Vector2I(2,0));
				EmitSignal(SignalName.UpdatedSeedCount, 1, "decrease");
				EmitSignal(SignalName.SeedPlaced, true);
				seeds_clicked = false;
			} else
			{
				GD.Print("Cannot plant, you don't have enough seeds!");
				EmitSignal(SignalName.SeedPlaced, false);
			}
		}
	}

	private void WaterPlant(Vector2I position)
	{
		int index = FindPlantAtCoordinates(position);
		if (index == -1)
		{
			GD.Print("Plant not found");
			return;
		} else
		{
			Plant foundPlant = plants[index];
			if(foundPlant.GetGrowthPhase() == 4)
			{
				return;
			}
			Dictionary<int, Vector2I> phasesOfSelectedPlant = plant_growth_phases_by_type.GetValueOrDefault(foundPlant.GetPlantType());
			if (phasesOfSelectedPlant != null && water_level > 0)
			{
				Vector2I wateredtTile = phasesOfSelectedPlant.GetValueOrDefault(foundPlant.GetGrowthPhase());
				GD.Print(foundPlant.GetGrowthPhase());
				SetCell(foundPlant.GetCoordinates(), 0, wateredtTile, 1);
				foundPlant.SetIsWatered(true);
				water_level--;
				LevelManager.Instance.SetWateringCanLevel(water_level);
				EmitSignal(SignalName.UpdatedWateringcanText);
			}
			
		}
	}

	private void CollectWater()
	{
		water_level = 10;
		LevelManager.Instance.SetWateringCanLevel(water_level);
		EmitSignal(SignalName.UpdatedWateringcanText);
	}


	public void RemovePlantAtCoordinates(Vector2I coordinates)
	{
		int index = FindPlantAtCoordinates(coordinates);
		if(index == -1)
		{
			GD.Print("Plant not found!");
			return;
		} else
		{	
			SetCell(coordinates,0, new Vector2I(1,0));
			plants.RemoveAt(index);
		}
	}

	private int FindPlantAtCoordinates(Vector2I coordinates)
	{
		return plants.FindIndex(plant => plant.GetCoordinates() == coordinates);
	}

	private int FindDefenseItemAtCoordinates(Vector2I coordinates)
	{
		return placed_defense_items.FindIndex(item => item.GetCoordinates() == coordinates);
	}

	private int FindDistractionItemAtCoordinates(Vector2I coordinates)
	{
		return placed_distraction_items.FindIndex(item => item.GetCoordinates() == coordinates);
	}

	public void UpdatePlantToNextPhase(Vector2I coordinates)
	{
		int index = FindPlantAtCoordinates(coordinates);
		if (index == -1)
		{
			GD.Print("Plant not found");
			return;
		} else
		{
			Plant foundPlant = plants[index];
			if(foundPlant.GetGrowthPhase() == 4)
			{
				return;
			}
			if (foundPlant.GetIsWatered())
			{
				int newPhase = foundPlant.GetGrowthPhase() + 1;
				Dictionary<int, Vector2I> phasesOfSelectedPlant = plant_growth_phases_by_type.GetValueOrDefault(foundPlant.GetPlantType());
				if (phasesOfSelectedPlant != null)
				{
					foundPlant.SetGrowthPhase(newPhase);
					Vector2I correctTile = phasesOfSelectedPlant.GetValueOrDefault(newPhase);
					SetCell(foundPlant.GetCoordinates(), 0, correctTile);
					GD.Print("Plant " + foundPlant.GetPlantType() + " is updated to phase " + foundPlant.GetGrowthPhase());
					foundPlant.SetIsWatered(false);
				}

			}
			
		}
	}
	//This method can be used in case of implementing fertilizer so that the plant would skip some phase.
	private void UpdatePlantToCustomPhase(Vector2I coordinates, int phase)
	{
		int index = FindPlantAtCoordinates(coordinates);
		if (index == -1)
		{
			GD.Print("Plant not found at this position!");
		} else
		{
			Plant foundPlant = plants[index];
			if (foundPlant.GetGrowthPhase() == phase)
			{
				GD.Print("Plant is already in this phase!");
			} else if (phase < foundPlant.GetGrowthPhase())
			{
				GD.Print("Plant cannot grow backwards!");
			} else
			{
				GD.Print("Changing plant phase!");
				SetCell(coordinates, 0, new Vector2I(phase,0));
			}
		}
	}

	private void initializePlantTypesAndPhases()
	{
		plant_growth_phases_by_type = new Dictionary<string, Dictionary<int, Vector2I>>();
		Dictionary<int, Vector2I> pineAppleDic = new Dictionary<int, Vector2I>
        {
            { 1, new Vector2I(2, 0) },
            { 2, new Vector2I(3, 0) },
            { 3, new Vector2I(4, 0) },
            { 4, new Vector2I(5, 0) }
        };
		plant_growth_phases_by_type.Add("pineapple", pineAppleDic);

		Dictionary<int, Vector2I> watermelonDic = new Dictionary<int, Vector2I>
        {
            { 1, new Vector2I(2, 0) },
            { 2, new Vector2I(3, 0) },
            { 3, new Vector2I(4, 0) },
            { 4, new Vector2I(6, 0) }
        };

		plant_growth_phases_by_type.Add("watermelon", watermelonDic);

		Dictionary<int, Vector2I> mangoDic = new()
        {
            { 1, new Vector2I(3, 3) },
            { 2, new Vector2I(4, 3) },
            { 3, new Vector2I(5, 3) },
            { 4, new Vector2I(6, 3) }
        };
		plant_growth_phases_by_type.Add("mango", mangoDic);

	}

	private void initializeUpgradeItems()
	{
		upgrade_items_by_name = new Dictionary<string, Dictionary<string, Vector2I>>();
		Dictionary<string, Vector2I> distractionDic = new()

        {
            { "camp_fire", new Vector2I(0, 0) },
            { "noise_maker", new Vector2I(0, 1) },
            { "noise_maker_2", new Vector2I(1, 1) },
            { "noise_maker_3", new Vector2I(2, 1) },
			 { "beehive", new Vector2I(1, 0) },
			{ "beehive_angry", new Vector2I(2, 0) },
			{ "beehive_angry_2", new Vector2I(3, 0) }
        };
		upgrade_items_by_name.Add("distraction", distractionDic);

		Dictionary<string, Vector2I> defenseDic = new()

        {
            { "fence", new Vector2I(1,0) },
            { "stone_wall", new Vector2I(0, 0) },
        };
		upgrade_items_by_name.Add("defense", defenseDic);

		/* plant_growth_phases_by_type.Add("watermelon", watermelonDic);

		Dictionary<int, Vector2I> mangoDic = new()
        {
            { 1, new Vector2I(3, 3) },
            { 2, new Vector2I(4, 3) },
            { 3, new Vector2I(5, 3) },
            { 4, new Vector2I(6, 3) }
        };
		plant_growth_phases_by_type.Add("mango", mangoDic); */

	}

	private void PlantDefensiveItem(string item_name)
	{
		
	}

	private void PlantDistractionItem(string item_name)
	{
		
	}

}
