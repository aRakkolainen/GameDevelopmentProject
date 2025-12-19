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

	
	private Godot.Collections.Array<Vector2I> farm_tile_coordinates;

	private List<Plant> plants;

	private Dictionary<string, Dictionary<int, Vector2I>> plant_growth_phases_by_type;

	private string[] plant_types = {"pineapple", "watermelon"};

	private string active_level; 

	private string plant_type;
	private LevelData level_data;

	private LevelManager level_manager;

	[Export] Player _player;

	[Export] TimeManager timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		plants = new List<Plant>();
		level_manager = LevelManager.Instance;
		farm_tile_coordinates = GetUsedCellsById(farm_source_id);
		initializePlantTypesAndPhases();
		active_level = level_manager.GetCurrentActiveLevel();
		level_data = level_manager.GetLevelData(active_level);
		if(level_data != null)
		{
			plant_type = level_data.GetPlantType();
			GD.Print(level_data);
		}
		plant_type = "";

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
		if(farm_tile_coordinates.Count > 0)
		{
			//Checking if player tries to plant..

		if (Input.IsActionJustPressed("mouse_right_click"))
		{
			Godot.Vector2 mousePos = GetLocalMousePosition();
			Vector2I mouse_map_pos = LocalToMap(mousePos);
			Vector2I atlas_coords = GetCellAtlasCoords(mouse_map_pos);
			GD.Print(atlas_coords);
			//Checking if there already is a plant in clicked coordinates.

			//Checking player position
			Godot.Vector2 player_position = _player.GetPositionDelta();
			GD.Print("Checking if you can plant here..");
			GD.Print(atlas_coords);
			if (atlas_coords == new Vector2I(0, 0) || atlas_coords == new Vector2I(-1,-1))
				{
					return;
				}
			if(plants != null && plants.Count > 0 )
				{
					GD.Print("Need to check if this tile is already planted!");
					int index = FindPlantAtCoordinates(mouse_map_pos);
					if(index != -1)
				{
					GD.Print("This tile is already planted, checking if it is ready to pickup");
					if(plants[index].GetGrowthPhase() == 4)
					{
						GD.Print("Your plant is fully grown!");

						InventoryItem item = new InventoryItem(1, plants[index].GetPlantType(), 32, 1);
							if (_player.AddToInventory(item))
							{
								RemovePlantAtCoordinates(mouse_map_pos);
							}
						
						
					} else
					{
						GD.Print("Your plant is not ready yet!");
					}
				} else
					{
						GD.Print("You can plant here!");
						PlacePlant(mouse_map_pos);
					}
					} else
					{
						GD.Print("You can plant your plant here!");
						PlacePlant(mouse_map_pos);
					}
				}
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
		string plant_type = level_data.GetPlantType();
		int plantable_tiles = farm_tile_coordinates.Count;
		int id = (int) (GD.Randi() % plantable_tiles);
		if(plant_type == null || plant_type == "")
		{
			GD.Print("Plant type undefined!");
		} else
		{
			Plant newPlant = new Plant(id, plant_type, default_plant_phase, position); 
			plants.Add(newPlant);
			SetCell(position, 0, new Vector2I(2,0));
		}
	}

	private void CollectWater()
	{
		
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
			int newPhase = foundPlant.GetGrowthPhase() + 1;
			foundPlant.SetGrowthPhase(newPhase);
			Dictionary<int, Vector2I> phasesOfSelectedPlant = plant_growth_phases_by_type.GetValueOrDefault(foundPlant.GetPlantType());
			if (phasesOfSelectedPlant != null)
			{
				Vector2I correctTile = phasesOfSelectedPlant.GetValueOrDefault(newPhase);
				SetCell(foundPlant.GetCoordinates(), 0, correctTile);
				GD.Print("Plant " + foundPlant.GetPlantType() + " is updated to phase " + foundPlant.GetGrowthPhase());
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
			return;
		} else
		{
			Plant foundPlant = plants[index];
			if (foundPlant.GetGrowthPhase() == phase)
			{
				GD.Print("Plant is already in this phase!");
				return;
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

}
