using Godot;
using ProtectFarm;
using System;
using System.Collections.Generic;
using System.Numerics;
//Source for this was this YouTube tutorial: https://www.youtube.com/watch?v=4qEOdviP1yA
public partial class FarmManager : TileMapLayer
{
	private int default_plant_phase = 1; 

	private int farm_source_id = 0;

	private int water_lake_id = 1;

	private int usable_items_id = 2;
	private Godot.Collections.Array<Vector2I> farm_tile_coordinates;

	private Godot.Collections.Array<Vector2I> water_tile_coordinates;

	private List<Plant> plants;

	private Dictionary<string, Dictionary<int, Vector2I>> plant_growth_phases_by_type;

	private Dictionary<String, Dictionary<string, Vector2I>> upgrade_items_by_name;

	private string[] plant_types = {"pineapple", "watermelon"};

	private string active_level; 

	private string plant_type;

	private int number_of_seeds_in_player_inventory = 0;
	private LevelData level_data;

	private LevelManager level_manager;
	private bool seeds_clicked; 

	private bool watering_can_clicked;

	private bool defense_item_clicked;

	private bool distraction_item_clicked;

	private string selected_defense_item; 

	private string selected_distraction_item; 


	private int water_level = 0;
	[Export] Player _player;

	[Export] TimeManager timer;

	[Export] SimpleInventory _inventory;

	[Signal] public delegate void UpdatedSeedCountEventHandler(int quantity, string update_type);

	[Signal]
	public delegate void UpdatedWateringcanTextEventHandler();

	[Signal] public delegate void PlayerTriedToPlaceDefenseItemEventHandler();

	[Signal] public delegate void PlayerTriedToPlaceDistractionItemEventHandler();
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
		_inventory = GetNode<SimpleInventory>("%SimpleInventory");
		_player ??= GetNode<Player>("%Player");
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
		}

		if (water_tile_coordinates != null && water_tile_coordinates.Count > 0 && Input.IsActionJustPressed("mouse_left_click") && watering_can_clicked)
		{
			Godot.Vector2 mousePos = GetLocalMousePosition();
			Vector2I mouse_map_pos = LocalToMap(mousePos);
			Vector2I atlas_coords = GetCellAtlasCoords(mouse_map_pos);
			if (atlas_coords == new Vector2I(0, 0) || atlas_coords == new Vector2I(-1,-1))
				{
					GD.Print("You cannot collect water from here");
					return;
				}
			if (atlas_coords == new Vector2I(2, 0))
			{
				CollectWater();
				GD.Print("Water collected!");
			}
		} 
		
		if (farm_tile_coordinates != null && farm_tile_coordinates.Count > 0 && Input.IsActionJustPressed("mouse_right_click") && _player.GetPlayerIsAlive()) 
		{
			number_of_seeds_in_player_inventory = _inventory.GetNumberOfSeedsInInventory();
			Godot.Vector2 mousePos = GetLocalMousePosition();
			Vector2I mouse_map_pos = LocalToMap(mousePos);
			Vector2I atlas_coords = GetCellAtlasCoords(mouse_map_pos);
			//Checking if there already is a plant in clicked coordinates.

				if (seeds_clicked)
				{
					if (atlas_coords == new Vector2I(0, 0) || atlas_coords == new Vector2I(-1,-1) && atlas_coords != new Vector2I(2,0))
					{
							GD.Print("You cannot plant here!");
							return;
						} else
					{
						PlacePlant(mouse_map_pos);
					}
					
				}
				if (watering_can_clicked)
						{
							WaterPlant(mouse_map_pos);
						}



			//Checking if there already are plants in this farm:
			if(plants != null && plants.Count > 0 )
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
					int index = FindPlantAtCoordinates(mouse_map_pos);

					if(index != -1)
				{
					GD.Print("This tile is already planted, checking if it is ready to pickup");
					if(plants[index].GetGrowthPhase() == 4)
					{
						GD.Print("Your plant is fully grown!");
						int inventory_size = _inventory.GetChildCount();
						InventoryItem item = new InventoryItem(inventory_size+1, plants[index].GetPlantType(), "fruit", 1, 32);
						_player.AddToInventory(item);
						RemovePlantAtCoordinates(mouse_map_pos);
						
					} else
					{
						GD.Print("Your plant is not ready yet!");
					}
				}

	}
	}
	}

    private void PlaceDefensiveItem(Vector2I mouse_map_pos)
    {
		Dictionary<string, Vector2I> defenses = upgrade_items_by_name.GetValueOrDefault("defense");
        if (selected_defense_item.Equals("fence"))
		{
			Vector2I fence_tile = defenses.GetValueOrDefault("fence");
			SetCell(mouse_map_pos, usable_items_id, fence_tile);
		} else if (selected_defense_item.Equals("stone_wall"))
		{
			Vector2I stonewall_tile = defenses.GetValueOrDefault("stone_wall");
			SetCell(mouse_map_pos, usable_items_id, stonewall_tile);
		} 
    }

	 private void PlaceDistractionItem(Vector2I mouse_map_pos)
    {
		Dictionary<string, Vector2I> defenses = upgrade_items_by_name.GetValueOrDefault("distraction");
        if (selected_distraction_item.Equals("camp_fire"))
		{
			Vector2I item_tile = defenses.GetValueOrDefault("camp_fire");
			SetCell(mouse_map_pos, usable_items_id, item_tile);
		} else if (selected_distraction_item.Equals("noise_maker"))
		{
			Vector2I noise_maker_tile = defenses.GetValueOrDefault("noise_maker");
			SetCell(mouse_map_pos, usable_items_id, noise_maker_tile);
		} else if (selected_distraction_item.Equals("beehive"))
		{
			Vector2I beehive_tile = defenses.GetValueOrDefault("beehive");
			SetCell(mouse_map_pos, usable_items_id, beehive_tile);
		} 
    }

    public void OnPlayerTriedToPlantSeed()
	{
		GD.Print("You are trying to plant seed");
		watering_can_clicked = false;
		seeds_clicked = true;

	}


	public void OnPlayerTriedToWaterPlant()
	{
		GD.Print("You tried to water plant");
		watering_can_clicked = true;
		seeds_clicked = false;
	} 

	public void OnPlayerTriedToPlaceDefenseItem(string name)
	{
		watering_can_clicked = false;
		seeds_clicked = false;
		selected_defense_item = name;
		defense_item_clicked = true;
		GD.Print("You tried to place defense item");
	}

	public void OnPlayerTriedToPlaceDistractionItem(string name)
	{
		watering_can_clicked = false;
		seeds_clicked = false;
		distraction_item_clicked = true;
		selected_distraction_item = name;
		GD.Print("You tried to place distraction item");
	}

	public void OnCollidedWithFarm()
	{
		int plantCount = plants.Count;
		if (plantCount > 0 )
		{
			
		int plantsToBeDestroyedMaximum = (int) plantCount/4;
		int plantsToBeDestroyedTotal = GD.RandRange(1, plantsToBeDestroyedMaximum);
		for (int i=0; i < plantsToBeDestroyedTotal; i++)
		{
			int randomIndex = (int) GD.RandRange(0, plants.Count-1);
			Plant plantToBeDestroyed = plants[randomIndex];
			RemovePlantAtCoordinates(plantToBeDestroyed.GetCoordinates());
		}
		GD.Print("Elephant destroyed " + plantsToBeDestroyedTotal + " plants.");
		}
		
		
	}


	public Godot.Collections.Array<Vector2I> GetFarmTileCoordinates()
	{
		farm_tile_coordinates ??= GetUsedCellsById(0);
		return farm_tile_coordinates;
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
				seeds_clicked = false;
			} else
			{
				GD.Print("Cannot plant, you don't have enough seeds!");
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
            { "fence", new Vector2I(1,2) },
            { "stone_wall", new Vector2I(0, 2) },
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
