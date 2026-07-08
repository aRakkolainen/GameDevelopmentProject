using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;

namespace ProtectFarm;
//Source for this was this YouTube tutorial: https://www.youtube.com/watch?v=4qEOdviP1yA
public partial class FarmManager : TileMapLayer
{
	private int default_plant_phase = 1; 

	private int farm_source_id = 0;

	private int water_lake_id = 3;

	private int usable_distraction_items_id = 1;
	private int usable_defense_items_id = 2;

	private int dropped_items_id = 5;

	private int dropped_poop_id = 6;

	private Godot.Vector2I default_tile_atlas_coords = new Godot.Vector2I(0, 0);
	private Godot.Collections.Array<Vector2I> farm_tile_coordinates;

	private Godot.Collections.Array<Vector2I> water_tile_coordinates;

	private Godot.Collections.Array<Vector2I> ground_tile_coordinates;

	private List<Puddle> puddles = new();

	private List<Plant> plants = new();

	private List<DroppedPlant> elephant_poops = new();

	private Vector2I puddle_atlas_coordinates = new(1,1);

	private Vector2I mud_puddle_atlas_coordinates = new(2,1);

	private Vector2I grass_tile_atlas_coordinates = new(3,1);

	private List<DefenseItem> placed_defense_items = new();

	private List<DistractionItem> placed_distraction_items = new();

	private List<DroppedPlant> dropped_plants = new();

	private Dictionary<string, Dictionary<int, Vector2I>> plant_growth_phases_by_name;

	private Dictionary<string, Vector2I> dropped_plants_dict;

	private Dictionary<String, Dictionary<string, Vector2I>> upgrade_items_by_name;

	private string[] plant_types = {"pineapple", "watermelon", "mango", "chili", "sunflower"};

	private int active_level; 

	private string plant_type;

	private string distraction_plant_type;

	private int number_of_seeds_in_player_inventory = 0;
	private LevelData level_data;

	private LevelManager level_manager;
	private bool seeds_clicked = false; 

	private bool other_plant_type_clicked = false;
    private bool plant_clicked;

    private bool watering_can_clicked = false;
    private bool super_fertilizer_clicked;

    private bool fertilizer_clicked = false;

	private bool defense_item_clicked = false;

	private bool distraction_item_clicked = false;

	private string selected_defense_item; 

	private string selected_distraction_item; 

	private DistractionItem active_distraction_item_with_sound; 

	private DistractionItem active_campfire;

	private List<DistractionItem> active_beehives; 
	private int water_level = 0;

	private int selected_item_id = -1;

	private DistractionAudioPlayer distractionAudioPlayer;
	[Export] Player _player;

	[Export] TimeManager timer;

	[Export] SimpleInventory _inventory;
    private Godot.Collections.Array<Vector2I> tiles_with_items_coordinates;
    private string dropped_plant_type;
    private Vector2I droppedPlantCoordinates;
    private int item_index_to_be_destroyed;

	private const string elephant_poop = "elephant_poop";


    [Signal] public delegate void UpdatedSeedCountEventHandler(int quantity, string update_type);

	[Signal] public delegate void SeedPlacedEventHandler(bool success);

	[Signal] public delegate void UpdatedItemCountEventHandler(int id, string item_name, int quantity, string update_type);

	[Signal] public delegate void UpdatedItemCountAfterPickupEventHandler(string item_name, int quantity, string update_type, int index);

	[Signal]
	public delegate void UpdatedWateringcanTextEventHandler();

	[Signal]
	public delegate void UpdatedInfoTextEventHandler(string message);

	[Signal] public delegate void PlayerTriedToPlaceDefenseItemEventHandler();

	[Signal] public delegate void PlayerTriedToPlaceDistractionItemEventHandler();

	[Signal] public delegate void CollidedWithFarmEventHandler(Vector2I tileCoords, Elephant elephant);

	[Signal] public delegate void CollidedWithItemEventHandler(Vector2I tileCoords, string itemType, Elephant elephant);

	[Signal] public delegate void SentElephantBackEventHandler(Vector2I tileCoords);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		plants = new List<Plant>();
		puddles = new List<Puddle>();
		dropped_plants = new List<DroppedPlant>();
		level_manager = LevelManager.Instance;
		farm_tile_coordinates = GetUsedCellsById(farm_source_id);
		water_tile_coordinates = GetUsedCellsById(water_lake_id);
		ground_tile_coordinates = GetUsedCellsById(dropped_items_id);
		InitializePlantTypesAndPhases();
		InitializeUpgradeItems();
		InitializeDroppedPlants();
		active_level = level_manager.GetCurrentActiveLevel();
		level_data = level_manager.GetLevelData(active_level);
		if(level_data != null)
		{
			plant_type = level_data.GetPlantType();
		} else
		{
			plant_type = "";
		}
		distractionAudioPlayer = GetNode<DistractionAudioPlayer>("DistractionAudioPlayer");
		_inventory = GetNode<SimpleInventory>("%SimpleInventory");
		_player ??= GetNode<Player>("%Player");
		CollidedWithFarm += OnElephantCollidedWithFarm;
        CollidedWithItem += OnElephantCollidedWithItem;


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
        if (_player.GetPlayerIsAlive())
        {
        	ListenForPlayerInteractionsWithFarm();
		}

        
    }

	public DistractionItem GetActiveDistractionItemWithSound()
	{
		return active_distraction_item_with_sound;
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
            number_of_seeds_in_player_inventory = _inventory.GetNumberOfSeedsInInventory(selected_item_id);
            Godot.Vector2 mousePos = GetLocalMousePosition();
            Vector2I mouse_map_pos = LocalToMap(mousePos);
            //Vector2I atlas_coords = GetCellAtlasCoords(mouse_map_pos);

			bool canInteract = IsPlayerCloseEnough(mouse_map_pos, player_local_map_pos);
            if (!canInteract)
            {
                return;
            }
			
			//Checking if there already is a plant in clicked coordinates.
			int sourceId = GetCellSourceId(mouse_map_pos);
			if(sourceId == farm_source_id)
			{
                int plant_index = CheckIfAlreadyPlanted(mouse_map_pos);

				
                InteractWithPlant(mouse_map_pos, plant_index);

				if (plant_index != -1 && fertilizer_clicked && _inventory.GetItemQuantityInInvetory(selected_item_id, "fertilizer") > 0)
				{
					if(plant_index >= plants.Count)
					{
						return;
					}
					bool fertilizingSucceeded = UpdatePlantToCustomPhase(plants[plant_index].GetCoordinates(), plants[plant_index].GetGrowthPhase()+1);
					if (fertilizingSucceeded)
					{
						EmitSignal(SignalName.UpdatedItemCount, selected_item_id, "fertilizer", 1, "decrease");
					}
				}

				/* if (plant_index != -1 && super_fertilizer_clicked && _inventory.GetItemQuantityInInvetory("super_fertilizer") > 0)
				{
					Dictionary<int, Vector2I> growth_phases =  plant_growth_phases_by_name.GetValueOrDefault(plants[plant_index].GetName());
					int growth_phases_count = 0;
					if(growth_phases != null)
					{
						growth_phases_count = growth_phases.Count;
					}
					bool fertilizingSucceeded = UpdatePlantToCustomPhase(plants[plant_index].GetCoordinates(), growth_phases_count);
					if (fertilizingSucceeded)
					{
						EmitSignal(SignalName.UpdatedItemCount, "super_fertilizer", 1, "decrease");
					}
				} */

            } else if (sourceId == dropped_items_id)
			{
				if (plant_clicked)
				{
            		if (canInteract && GetCellAtlasCoords(mouse_map_pos) == grass_tile_atlas_coordinates && _inventory.GetItemQuantityInInvetory(selected_item_id, dropped_plant_type) > 0)
            	{
					DropPlant(mouse_map_pos, dropped_plant_type);
				}
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

		if (Input.IsActionJustPressed("mouse_left_click") && _player.GetPlayerIsAlive())
        {
            Godot.Vector2 mousePos = GetLocalMousePosition();
            Vector2I mouse_map_pos = LocalToMap(mousePos);
			bool canInteract = IsPlayerCloseEnough(mouse_map_pos, player_local_map_pos);
            if (canInteract && GetCellSourceId(mouse_map_pos) == dropped_items_id || GetCellSourceId(mouse_map_pos) == usable_defense_items_id || GetCellSourceId(mouse_map_pos) == usable_distraction_items_id ||  GetCellSourceId(mouse_map_pos) == dropped_poop_id)
			{
				int defense_item_index = FindItemAtCoordinates(mouse_map_pos, "defense");
				int distraction_item_index = FindItemAtCoordinates(mouse_map_pos, "distraction");
				int dropped_plant_item_index = FindItemAtCoordinates(mouse_map_pos, "dropped_plant");
				int elephant_poop_index = FindItemAtCoordinates(mouse_map_pos, elephant_poop);
				if(defense_item_index != -1)
				{
					PickUpItem(mouse_map_pos, "defense", defense_item_index);
					
				} else if (distraction_item_index != -1)
				{
					PickUpItem(mouse_map_pos, "distraction", distraction_item_index);
				} else if (dropped_plant_item_index != -1)
				{
					PickUpItem(mouse_map_pos, "dropped_plant", dropped_plant_item_index);
				} else if (elephant_poop_index != -1)
				{
					PickUpItem(mouse_map_pos, "elephant_poop", elephant_poop_index);
				} else
				{
					GD.Print("There is no item to be picked up!");
				}
			}
		}


    }

    private void PickUpItem(Vector2I mouse_map_pos, string type, int item_index)
    {
        GD.Print("Trying to pickup item ", type);
		switch (type)
        {
            case "defense":
				DefenseItem defenseItem = placed_defense_items[item_index];
				if (defenseItem != null && defenseItem.GetIsPickable()){
					if(_inventory.GetItemQuantityInInvetory(selected_item_id, defenseItem.GetName()) == 0)
					{
						 int inventory_size = _inventory.GetChildCount();
						InventoryItem item = new InventoryItem(inventory_size+1, defenseItem.GetName(), defenseItem.GetType(), 1, _inventory.GetMaxStack());
						_player.AddToInventory(item);

					} else
					{
						EmitSignal(SignalName.UpdatedItemCount, selected_item_id, defenseItem.GetName(), 1, "increase");
					}
					DestroyItemAtCoordinates(type, mouse_map_pos);
				}
				break;
			case "distraction":
				DistractionItem distractionItem = placed_distraction_items[item_index];
				if (distractionItem != null && distractionItem.GetIsPickable()){
					if(_inventory.GetItemQuantityInInvetory(selected_item_id,distractionItem.GetName()) == 0)
					{
						 int inventory_size = _inventory.GetChildCount();
						InventoryItem item = new InventoryItem(inventory_size+1, distractionItem.GetName(), distractionItem.GetType(), 1, _inventory.GetMaxStack());
						_player.AddToInventory(item);
					} else
					{
						EmitSignal(SignalName.UpdatedItemCount, selected_item_id, distractionItem.GetName(), 1, "increase");
					}
					DestroyItemAtCoordinates(type, mouse_map_pos);
				}

				break;
			case "dropped_plant":
				DroppedPlant droppedPlant = dropped_plants[item_index];
				if (droppedPlant != null && droppedPlant.GetIsPickable()){
					if(_inventory.GetItemQuantityInInvetory(selected_item_id, droppedPlant.GetName()) == 0)
					{
						 int inventory_size = _inventory.GetChildCount();
						InventoryItem item = new InventoryItem(inventory_size+1, droppedPlant.GetName(), "plant", 1, _inventory.GetMaxStack());
						_player.AddToInventory(item);
					} else
					{
						EmitSignal(SignalName.UpdatedItemCount, selected_item_id, droppedPlant.GetName(), 1, "increase");
					}
					DestroyItemAtCoordinates(type, mouse_map_pos);
					
				}
				break;
			case elephant_poop:
				DroppedPlant droppedPoop = elephant_poops[item_index];
				if (droppedPoop != null && droppedPoop.GetIsPickable()){
					if(_inventory.GetItemQuantityInInvetory(selected_item_id, droppedPoop.GetName()) == 0)
					{
						 int inventory_size = _inventory.GetChildCount();
						InventoryItem item = new InventoryItem(inventory_size+1, droppedPoop.GetName(), droppedPoop.GetType(), 1, _inventory.GetMaxStack());
						_player.AddToInventory(item);
					} else
					{
						EmitSignal(SignalName.UpdatedItemCount, selected_item_id, droppedPoop.GetName(), 1, "increase");
					}
					DestroyItemAtCoordinates(type, mouse_map_pos);
				}
				break;
		}
    }


    private Vector2I GetItemCoordinatesAtIndex(string type, int index)
    {
        switch (type)
        {
            case "defense":
				return placed_defense_items[index].GetCoordinates();
			case "distraction":
				return placed_distraction_items[index].GetCoordinates();
			case "dropped_plant":
				return dropped_plants[index].GetCoordinates();
			default:
				return new Vector2I(0,0);
		}
    }


    private void DropPlant(Vector2I mouse_map_pos, string plant_name)
    {
        SetCell(mouse_map_pos, dropped_items_id, dropped_plants_dict.GetValueOrDefault(plant_name));
		if ("chili".Equals(plant_name) || "sunflower".Equals(plant_name))
		{
			if ("chili".Equals(plant_name))
			{
				dropped_plants.Add(new DroppedPlant(dropped_plants.Count + 1, plant_name, "plant", mouse_map_pos,  dropped_plants_dict.GetValueOrDefault(plant_name), "distraction", true));
			} else
			{
				dropped_plants.Add(new DroppedPlant(dropped_plants.Count + 1, plant_name, "plant", mouse_map_pos, dropped_plants_dict.GetValueOrDefault(plant_name), "distraction", true));
			}
		} else
		{
			dropped_plants.Add(new DroppedPlant(dropped_plants.Count + 1, plant_name, "plant", mouse_map_pos, dropped_plants_dict.GetValueOrDefault(plant_name), "fertiler_boost", true));	
		}
		EmitSignal(SignalName.UpdatedItemCount, selected_item_id, plant_name, 1, "decrease");
		
    }


    private void InteractWithPlant(Vector2I mouse_map_pos, int plant_index)
    {
		string plant_type = "";
		if (distraction_plant_type != null)
		{
			plant_type = distraction_plant_type;
		} else
		{
			plant_type = LevelManager.Instance.GetCurrentLevelPlantType();
		}
		
        if (plant_index == -1 && seeds_clicked )
        {
            PlacePlant(mouse_map_pos, plant_type);
        }
        
        if (plants.Count > 0 && plant_index != -1)
        {
            PickUpPlant(mouse_map_pos, plant_index);
        }
    }


    private void ActivateDistractionItemWithSound(DistractionItem item)
    {
		if (active_distraction_item_with_sound == null)
		{
            Godot.Timer distractionItemWithSoundDuractionTimer = new Godot.Timer
            {
                WaitTime = item.GetEffectDuration(),
				OneShot = true
            };
			AddChild(distractionItemWithSoundDuractionTimer);
			distractionItemWithSoundDuractionTimer.Timeout +=  OnNoiseMakerTimeOut;
            distractionItemWithSoundDuractionTimer.Start();
			if ("noise_maker".Equals(item.GetName())){
				distractionAudioPlayer.PlayDistractionAudio("noise_maker");
			} else if ("camp_fire".Equals(item.GetName()))
			{
				distractionAudioPlayer.PlayDistractionAudio("camp_fire");
			} else if ("beehive".Equals(item.GetName()))
			{
				distractionAudioPlayer.PlayDistractionAudio("beehive");
			}
			active_distraction_item_with_sound = item;
		} else
		{
			GD.Print("Only one noise maker can be active at once!");
		}
    }

	private void OnNoiseMakerTimeOut()
	{
		distractionAudioPlayer.Stop();
		if (active_distraction_item_with_sound != null)
		{
			DestroyItemAtCoordinates("distraction", active_distraction_item_with_sound.GetCoordinates());
			active_distraction_item_with_sound = null;
			
		}
	}

	/* private void ActivateCampfire(DistractionItem camp_fire)
    {
            Godot.Timer durationTimer = new Godot.Timer
            {
                WaitTime = camp_fire.GetEffectDuration(),
				OneShot = true
            };
			AddChild(durationTimer);
			durationTimer.Timeout +=  OnCampfireTimeOut;
            durationTimer.Start();
			active_campfire = camp_fire;
	}

	private void OnCampfireTimeOut()
	{
		if (active_campfire != null)
		{
			
			DestroyItemAtCoordinates("distraction", active_campfire.GetCoordinates());
			active_campfire = null;
			
		}
	} */


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
        if (plants.Count > 0)
        {
			Plant plant = plants[index];
			Dictionary<int, Vector2I> phases = plant_growth_phases_by_name.GetValueOrDefault(plant.GetName());
			int lastPhase = 0;
			if(phases != null)
			{
				lastPhase = phases.Count;
			}
			if(lastPhase > 0 && plant.GetGrowthPhase()== lastPhase) 
			{
            	GD.Print("Your plant is fully grown!");
            	int inventory_size = _inventory.GetChildCount();
            	InventoryItem item = new InventoryItem(inventory_size + 1, plant.GetName(), plant.GetType(), 1, 32);
            	_player.AddToInventory(item);
            	RemovePlantAtCoordinates(mouse_map_pos);
			}
        }
        else
        {
			//infoMessage = "Your plant is not ready yet!";
            GD.Print("Your plant is not ready yet!");
        }
		UpdateInfoText(infoMessage);
    }


    private void PlaceDefensiveItem(Vector2I mouse_map_pos)
    {
		Dictionary<string, Vector2I> defenses = upgrade_items_by_name.GetValueOrDefault("defense");
		string infoMessage = "";
		if (farm_tile_coordinates.IndexOf(mouse_map_pos) != -1 || water_tile_coordinates.IndexOf(mouse_map_pos) != -1 && FindPlantAtCoordinates(mouse_map_pos) == -1)
		{
			infoMessage = "Cannot place item at farm or water tiles!";
			GD.Print("Cannot place item at farm or water!");
		} else
		{
		if (_inventory != null && _inventory.GetItemQuantityInInvetory(selected_item_id, selected_defense_item) > 0)
			{
        if (selected_defense_item.Equals("fence"))
		{
			Vector2I fence_tile = defenses.GetValueOrDefault("fence");
			placed_defense_items.Add(new DefenseItem(placed_defense_items.Count+1, "defense", selected_defense_item, mouse_map_pos, true, Scenes.UpgradeItemTextures.fence, true, 2));
			SetCell(mouse_map_pos, usable_defense_items_id, fence_tile);
			EmitSignal(SignalName.UpdatedItemCount, selected_item_id, selected_defense_item, 1, "decrease");
		} else if (selected_defense_item.Equals("stone_wall"))
		{
			Vector2I stonewall_tile = defenses.GetValueOrDefault("stone_wall");
			placed_defense_items.Add(new DefenseItem(placed_defense_items.Count+1, "defense", selected_defense_item, mouse_map_pos, true, Scenes.UpgradeItemTextures.stone_wall, true, 4));
			SetCell(mouse_map_pos, usable_defense_items_id, stonewall_tile);
			EmitSignal(SignalName.UpdatedItemCount, selected_item_id, selected_defense_item, 1, "decrease");
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
			if (_inventory != null && _inventory.GetItemQuantityInInvetory(selected_item_id, selected_distraction_item) > 0)
			{
				if(placed_defense_items.FindIndex(item => item.GetCoordinates() == mouse_map_pos) != -1){
					GD.Print("Item already placed on these coordinates: ", mouse_map_pos);
					return;
				}
				
        	if (selected_distraction_item.Equals("camp_fire"))
			{
				Vector2I item_tile = distractions.GetValueOrDefault("camp_fire");
				DistractionItem campfire = new DistractionItem(placed_distraction_items.Count+1, "distraction", selected_distraction_item, mouse_map_pos, true, Scenes.UpgradeItemTextures.camp_fire, true, 15, 3, true, 2);
				placed_distraction_items.Add(campfire);
				SetCell(mouse_map_pos, usable_distraction_items_id, item_tile);
				EmitSignal(SignalName.UpdatedItemCount, selected_item_id, selected_distraction_item, 1, "decrease");
				ActivateDistractionItemWithSound(campfire);
			} else if (selected_distraction_item.Equals("noise_maker"))
			{
				Vector2I noise_maker_tile = distractions.GetValueOrDefault("noise_maker");
				DistractionItem noise_maker = new DistractionItem(placed_distraction_items.Count+1, "distraction", selected_distraction_item, mouse_map_pos, false, Scenes.UpgradeItemTextures.noise_maker, true, 15, 5, false, 0);
				if (active_distraction_item_with_sound == null)
					{
						placed_distraction_items.Add(noise_maker);
						SetCell(mouse_map_pos, usable_distraction_items_id, noise_maker_tile);
						ActivateDistractionItemWithSound(noise_maker);
						EmitSignal(SignalName.UpdatedItemCount, selected_item_id, selected_distraction_item, 1, "decrease");
				}
			} else if (selected_distraction_item.Equals("beehive"))
			{
				Vector2I beehive_tile = distractions.GetValueOrDefault("beehive");
				DistractionItem beehive = new DistractionItem(placed_distraction_items.Count+1, "distraction", selected_distraction_item, mouse_map_pos, true, Scenes.UpgradeItemTextures.beehive, false, 15, 2, true, 5);
				if (active_distraction_item_with_sound == null)
					{
					placed_distraction_items.Add(beehive);
					SetCell(mouse_map_pos, usable_distraction_items_id, beehive_tile);
					ActivateDistractionItemWithSound(beehive);
					EmitSignal(SignalName.UpdatedItemCount, selected_item_id, selected_distraction_item, 1, "decrease");
					}
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
		} else if ("dropped_plant".Equals(item_type))
			{
				dropped_plants.RemoveAt(index);
			} 
		else if ("puddle".Equals(item_type))
			{
				puddles.RemoveAt(index);
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
		} else if ("puddle".Equals(item_type)){
			return puddles.FindIndex(puddle => puddle.GetCoordinates() == coordinates);
			
		}else if ("dropped_plant".Equals(item_type)){
			return dropped_plants.FindIndex(plant => plant.GetCoordinates() == coordinates);
		} else if ("elephant_poop".Equals(item_type))
		{
			return elephant_poops.FindIndex(poop => poop.GetCoordinates() == coordinates);
		}
		else
		{
			return -1;
		}
    }


    public void OnPlayerTriedToPlantSeed(int id)
	{
		distraction_plant_type = null;
		selected_item_id = id;
		GD.Print("You are trying to plant seed");
		SelectNewAndResetOtherSelections("seeds");
	}

	public void OnPlayerTriedToUseFertilizer(bool isSuperFertilizer, int id)
	{
		GD.Print("You are trying to fertilize plant");
		selected_item_id = id;
		if (isSuperFertilizer)
		{
			SelectNewAndResetOtherSelections("super_fertilizer");
		} else
		{
			SelectNewAndResetOtherSelections("fertilizer");
		}
	}

	public void OnPlayerTriedToPlantDistractionPlant(int id, string plant_name)
	{
		GD.Print("You are trying to plant " + plant_name);
		selected_item_id = id;
		if(plant_name != ""){
			distraction_plant_type = plant_name;
		}
		SelectNewAndResetOtherSelections("distraction_plant");
	}

	public void OnPlayerTriedToWaterPlant()
	{
		GD.Print("You tried to water plant");
		SelectNewAndResetOtherSelections("watering_can");

	} 

	public void OnPlayerTriedToPlaceDefenseItem(int id, string name)
	{
		SelectNewAndResetOtherSelections("defense");
		selected_defense_item = name;
		selected_item_id = id;
		GD.Print("You tried to place defense item");
	}

	public void OnPlayerTriedToPlaceDistractionItem(int id, string name)
	{
		SelectNewAndResetOtherSelections("distraction");
		selected_distraction_item = name;
		selected_item_id = id;
		GD.Print("You tried to place distraction item");
	}

	public void OnPlayerTriedToDropPlant(int id, string name)
	{
		SelectNewAndResetOtherSelections("plant");
		selected_item_id = id;
		dropped_plant_type = name;
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
				plant_clicked = false; 
				fertilizer_clicked = false;
				super_fertilizer_clicked = false;
				break;
			case "watering_can":
				watering_can_clicked = true;
				seeds_clicked = false;
				defense_item_clicked = false; 
				distraction_item_clicked = false;
				plant_clicked = false; 
				fertilizer_clicked = false;
				super_fertilizer_clicked = false;
				break;

			case "fertilizer":
				super_fertilizer_clicked = false;
				fertilizer_clicked = true;
				watering_can_clicked = false;
				seeds_clicked = false;
				defense_item_clicked = false; 
				distraction_item_clicked = false;
				plant_clicked = false; 
				break;
			case "super_fertilizer":
				super_fertilizer_clicked = true;
				fertilizer_clicked = false;
				watering_can_clicked = false;
				seeds_clicked = false;
				defense_item_clicked = false; 
				distraction_item_clicked = false;
				plant_clicked = false; 
				break;
			case "defense":
				defense_item_clicked = true; 
				watering_can_clicked = false;
				seeds_clicked = false;
				distraction_item_clicked = false;
				plant_clicked = false; 
				fertilizer_clicked = false;
				super_fertilizer_clicked = false;
				break;
			case "distraction":
				distraction_item_clicked = true;
				defense_item_clicked = false; 
				watering_can_clicked = false;
				seeds_clicked = false;
				plant_clicked = false; 
				fertilizer_clicked = false;
				super_fertilizer_clicked = false;
				break;

			case "distraction_plant":
				distraction_item_clicked = false;
				defense_item_clicked = false; 
				watering_can_clicked = false;
				seeds_clicked = true;
				plant_clicked = false; 
				fertilizer_clicked = false;
				super_fertilizer_clicked = false;
				break;
			case "plant":
				distraction_item_clicked = false;
				defense_item_clicked = false; 
				watering_can_clicked = false;
				seeds_clicked = false;
				other_plant_type_clicked = false;
				plant_clicked = true; 
				fertilizer_clicked = false;
				super_fertilizer_clicked = false;
				break;
		}
	}


	/*
		Probabilities for how many plants are destroyed in line if multiple
		50% 1 plant
		35% 2 plants
		15% all plants

	*/
	public void OnElephantCollidedWithFarm(Vector2I elephant_position, Elephant elephant)
	{
		int plantCount = plants.Count;
		if (plantCount > 0 )
		{
			GD.Print(elephant_position);

			int elephant_walking_line = elephant_position.Y;
			List<Plant> plants_on_line = plants.FindAll(plant => plant.GetCoordinates().Y == elephant_position.Y);
			string distraction_plant_type = LevelManager.Instance.GetCurrentLevelDistractionPlantType();
			List<Plant> distraction_plants_on_line = new();
			if (distraction_plant_type != null)
			{
				distraction_plants_on_line = plants_on_line.FindAll(plant => plant.GetName().Equals(distraction_plant_type));
				
			if (distraction_plants_on_line.Count > 0)
			{
				GD.Print("Distraction plant found!");
				Dictionary<int, Vector2I> plant_phases = plant_growth_phases_by_name.GetValueOrDefault(distraction_plant_type);
				if(plant_phases != null)
				{
					int final_phase = plant_phases.Count;
					List<Plant> fully_grown_distraction_plants_within_range = distraction_plants_on_line.FindAll(plant => plant.GetGrowthPhase() == final_phase && (Math.Abs(plant.GetCoordinates().X - elephant_position.X) < 2));
					if (fully_grown_distraction_plants_within_range.Count > 0)
					{
							if ("chilI".Equals(distraction_plant_type))
							{
								elephant.SetElephantEatenChili(true);
							} else if ("sunflower".Equals(distraction_plant_type))
							{
								elephant.SetElephantSmelledSunflower(true);
							}
						elephant.PushAway();
						return;
					}
				}
			}
			} 

				
			if (plants_on_line.Count > 0 && !elephant.GetElephantEatenChili() || !elephant.GetElephantSmelledSunflower())
			{
				List<Puddle> puddles_on_line = puddles.FindAll(puddle => puddle.GetCoordinates().Y == elephant_walking_line && puddle.GetElephantHasTouchedPuddle() && Math.Abs(elephant_position.X - puddle.GetCoordinates().X) < 2);
				if(puddles_on_line != null && puddles_on_line.Count > 0)
				{

					if(puddles_on_line.Count == 1 && plants_on_line.Count == 1)
					{
						Puddle puddle = puddles_on_line[0];
						if (elephant.GetElephantCollidedWithMudPuddle() && GetCellAtlasCoords(puddle.GetCoordinates()) == mud_puddle_atlas_coordinates)
						{
							Plant plant_to_be_fertilized = plants_on_line[0];
							plant_to_be_fertilized.SetIsFertilizedByElephant(true);
						} else
						{
							
						Plant plant_to_be_watered= plants_on_line[0];
						if (WaterPlant(plant_to_be_watered.GetCoordinates()))
							{
								plant_to_be_watered.SetIsWateredByElephant(true);
								return;
							}
						}
					} else if(puddles_on_line.Count <= plants_on_line.Count)
					{
						for (int i=0; i < puddles_on_line.Count;i++)
						{
							Puddle puddle = puddles_on_line[i];
							if (elephant.GetElephantCollidedWithMudPuddle() && GetCellAtlasCoords(puddle.GetCoordinates()) == mud_puddle_atlas_coordinates)
						{
							Plant plant_to_be_fertilized = plants_on_line[i];
							var elephant_might_fertilize_plant = GD.Randf();
							if (elephant_might_fertilize_plant <= 0.80f)
							{
								bool fertilizingSucceeded = UpdatePlantToNextPhase(plant_to_be_fertilized.GetCoordinates());
							if (fertilizingSucceeded)
								{
									plant_to_be_fertilized.SetIsFertilizedByElephant(true);
									return;
								}
							}
						} else
							{
								Plant plant_to_be_watered= plants_on_line[i];
								if (WaterPlant(plant_to_be_watered.GetCoordinates()))
								{
									plant_to_be_watered.SetIsWateredByElephant(true);
									return;
								
							}
							}
						}
					}
				}	
				var plant_roll = GD.Randf();
				if (plants_on_line.Count == 1)
				{
					Plant plant_to_be_destroyed = plants_on_line[0];
					if("chili".Equals(plant_to_be_destroyed.GetName()) || "sunflower".Equals(plant_to_be_destroyed.GetName()))
						{
							return;
						}
					if(plant_to_be_destroyed.GetIsWateredByElephant() || plant_to_be_destroyed.GetIsFertilizedByElephant())
					{
						if (plant_roll <= 0.20f){
							RemovePlantAtCoordinates(plant_to_be_destroyed.GetCoordinates());
						}
					} else
					{
						if (plant_roll <= 0.40f){
							if("chili".Equals(plant_to_be_destroyed.GetName()) || "sunflower".Equals(plant_to_be_destroyed.GetName()))
							{
								return;
							}
							RemovePlantAtCoordinates(plant_to_be_destroyed.GetCoordinates());
						}
					}
						
					} else
				{
						DestroyRandomPlant(plants_on_line);
				}
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
			int itemTotalHealth = item.GetTotalHealth();
			if (item != null)
			{
				if(item.GetHealth() == 0 || item.GetHealth()-1 == 0)
				{
					DestroyItemAtCoordinates(itemType, item.GetCoordinates());

				} else
				{
					
				if(elephant.GetElephantAttackedDefenseItemCount() < 2)
				{
					item.TakeDamage(1);
				 	elephant.OnPushBack();
				} else
					{
						elephant.PushAwayAfterAttack();
					}
				}

			}

			
		} else if ("puddle".Equals(itemType) || elephant.GetElephantCollidedWithPuddle())
		{
			Puddle puddle = puddles[index];
			GD.Print("Collided with puddle");
			puddle.SetElephantHasTouchedPuddle(true);
			if(puddle.GetNumberOfElephantTouches() > 2)
			{
				SetCell(puddle.GetCoordinates(), dropped_items_id, new Vector2I(2,1));
			} else if (puddle.GetNumberOfElephantTouches() > 4){
				SetCell(puddle.GetCoordinates(), dropped_items_id, new Vector2I(3,1));
				puddle.SetElephantHasTouchedPuddle(false);
			} else
			{
				puddle.SetNumberOfElephantTouches(puddle.GetNumberOfElephantTouches()+1);
				
			}

		} else if ("dropped_plant".Equals(itemType))
        {
            DroppedPlant droppedPlant =  dropped_plants[index];
			if ("pineapple".Equals(droppedPlant.GetName())){
				elephant.PauseMovementAndPlayAnimation("eat pineapple");
			} else if ("mango".Equals(droppedPlant.GetName())){
				elephant.PauseMovementAndPlayAnimation("eat mango");
			} else if ("watermelon".Equals(droppedPlant.GetName()))
			{
				elephant.PauseMovementAndPlayAnimation("eat watermelon");
			} else if("chili".Equals(droppedPlant.GetName())){
				GD.Print("eating chili!");
				elephant.PauseMovementAndPlayAnimation("eat chili");
			} else if ("sunflower".Equals(droppedPlant.GetName()))
			{
				elephant.PauseMovementAndPlayAnimation("smell sunflower");
			}
				else
			{
				return;
			}
			droppedPlantCoordinates = droppedPlant.GetCoordinates();
		}
	}

    private void OnEatingFinished()
    {
       SetCell(droppedPlantCoordinates, 6, new Vector2I(0,0));
    }


    private void DestroyRandomPlant(List<Plant> plants_on_line)
    {
        	int random_index = GD.RandRange(0, plants_on_line.Count-1);
			if (random_index == -1 || random_index > plants_on_line.Count)
			{
				return;
			} else
			{
				var roll = GD.Randf();
        		Plant plant_to_be_destroyed = plants_on_line[random_index];
				if("chili".Equals(plant_to_be_destroyed.GetName()) || "sunflower".Equals(plant_to_be_destroyed.GetName()))
					{
						return;
					} else if(plant_to_be_destroyed.GetIsFertilizedByElephant() || plant_to_be_destroyed.GetIsWateredByElephant())
				{
					if(roll <= 0.15)
						{
						RemovePlantAtCoordinates(plant_to_be_destroyed.GetCoordinates());
						plants_on_line.Remove(plant_to_be_destroyed);
					}
				} else
			{
				if(roll <= 0.30)
					{
						RemovePlantAtCoordinates(plant_to_be_destroyed.GetCoordinates());
						plants_on_line.Remove(plant_to_be_destroyed);
					}
					
				}
				
			}


    }

	private void WaterRandomPlant(List<Plant> plants_on_line)
    {
        	int random_index = GD.RandRange(0, plants_on_line.Count-1);
			if (random_index == -1 || random_index > plants_on_line.Count)
			{
				return;
			} else
			{
        		Plant plant_to_be_watered= plants_on_line[random_index];
			if (WaterPlant(plant_to_be_watered.GetCoordinates()))
			{
				plant_to_be_watered.SetIsWateredByElephant(true);
			}
			}
    }

	private void FertilizeRandomPlant(List<Plant> plants_on_line)
    {
        	int random_index = GD.RandRange(0, plants_on_line.Count-1);
			if (random_index == -1 || random_index > plants_on_line.Count)
			{
			} else
			{
        		Plant plant= plants_on_line[random_index];
			if (UpdatePlantToNextPhase(plant.GetCoordinates()))
			{
				plant.SetIsFertilizedByElephant(true);
			}
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
	private void PlacePlant(Vector2I position, string plant_name)
	{
		int plantable_tiles = farm_tile_coordinates.Count;
		int id = (int) (GD.Randi() % plantable_tiles);
		if(plant_name == null || plant_name == "")
		{
			GD.Print("Plant name undefined!");
		} else
		{
			int seeds_in_inventory = 0;
			if(distraction_plant_type != null)
			{
				seeds_in_inventory = _inventory.GetItemQuantityInInvetory(selected_item_id, distraction_plant_type + "_seeds");
			} else
			{
				seeds_in_inventory = _inventory.GetNumberOfSeedsInInventory(selected_item_id);
				
			}
			if (seeds_in_inventory > 0)
			{
				Plant newPlant = new Plant(id, plant_name, "plant", position, true, default_plant_phase, false, false, false); 
				plants.Add(newPlant);
				SetCell(position, 0, new Vector2I(2,0));
				EmitSignal(SignalName.UpdatedItemCount, selected_item_id, plant_name + "_seeds", 1, "decrease");
				/* if ("chili".Equals(plant_name) || "sunflower".Equals(plant_name)){
				} else
				{
					EmitSignal(SignalName.UpdatedSeedCount, 1, "decrease");
				} */
				EmitSignal(SignalName.SeedPlaced, true);


			} else
			{
				GD.Print("Cannot plant, you don't have enough seeds!");
				EmitSignal(SignalName.SeedPlaced, false);
			}
		}
	}

	private bool WaterPlant(Vector2I position)
	{
		int index = FindPlantAtCoordinates(position);
		bool wateredPlantSuccessfully = false;
		if (index == -1)
		{
			GD.Print("Plant not found");
			int waterable_cell_id = GetCellSourceId(position);
			if (waterable_cell_id != -1 && water_level > 0 && waterable_cell_id == dropped_items_id && LevelManager.Instance.GetWateringCanPuddleUpgrade() && puddles.Count <= LevelManager.Instance.GetWateringCanTotalLevel())
			{
				if(puddles.FindIndex(puddle => puddle.GetCoordinates() == position) != -1)
				{
					return false;
				} else
				{
					
				SetCell(position, dropped_items_id, new Vector2I(1,1));
				Puddle puddle = new(puddles.Count + 1, "boost", "puddle", position, false, false, 0, LevelManager.Instance.GetCurrentDay());
				puddles.Add(puddle);
				water_level--;
				LevelManager.Instance.SetWateringCanLevel(water_level);
				EmitSignal(SignalName.UpdatedWateringcanText);
				}
			}
			
		} else
		{
			Plant foundPlant = plants[index];
			Dictionary<int, Vector2I> phasesOfSelectedPlant = plant_growth_phases_by_name.GetValueOrDefault(foundPlant.GetName());
			if (phasesOfSelectedPlant != null && water_level > 0 && foundPlant.GetGrowthPhase() < phasesOfSelectedPlant.Count)
			{
				Vector2I wateredtTile = phasesOfSelectedPlant.GetValueOrDefault(foundPlant.GetGrowthPhase());
				GD.Print(foundPlant.GetGrowthPhase());
				if(foundPlant.GetIsWatered() || foundPlant.GetIsWateredByElephant())
				{
					return false;
				} else
				{
					
					SetCell(foundPlant.GetCoordinates(), 0, wateredtTile, 1);
					foundPlant.SetIsWatered(true);
					water_level--;
					LevelManager.Instance.SetWateringCanLevel(water_level);
					EmitSignal(SignalName.UpdatedWateringcanText);
					wateredPlantSuccessfully = true;
				}
			}
			
		}
		return wateredPlantSuccessfully;
	}

	private void CollectWater()
	{
		water_level = LevelManager.Instance.GetWateringCanTotalLevel();
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

	public bool UpdatePlantToNextPhase(Vector2I coordinates)
	{
		bool isSuccess = false;
		int index = FindPlantAtCoordinates(coordinates);
		if (index == -1)
		{
			GD.Print("Plant not found");
			return isSuccess;
		} else
		{
			Plant foundPlant = plants[index];
			
			Dictionary<int, Vector2I> phasesOfSelectedPlant = plant_growth_phases_by_name.GetValueOrDefault(foundPlant.GetName());
			if(foundPlant.GetGrowthPhase() == phasesOfSelectedPlant.Count)
			{
				return isSuccess;
			}
			if (foundPlant.GetIsWatered())
			{
				int newPhase = foundPlant.GetGrowthPhase() + 1;
				if (phasesOfSelectedPlant != null)
				{
					foundPlant.SetGrowthPhase(newPhase);
					Vector2I correctTile = phasesOfSelectedPlant.GetValueOrDefault(newPhase);
					SetCell(foundPlant.GetCoordinates(), 0, correctTile);
					GD.Print("Plant " + foundPlant.GetType() + " is updated to phase " + foundPlant.GetGrowthPhase());
					foundPlant.SetIsWatered(false);
					isSuccess = true;
				}

			}
			return isSuccess;
		}
	}

	public void ResetWateredByElephant()
	{
		List<Plant> plants_watered_by_elephants = plants.FindAll(plant => plant.GetIsWateredByElephant());
		foreach (Plant plant in plants_watered_by_elephants)
		{
			plant.SetIsWateredByElephant(false);
		}
	}
	//This method can be used in case of implementing fertilizer so that the plant would skip some phase.
	private bool UpdatePlantToCustomPhase(Vector2I coordinates, int new_phase)
	{
		int index = FindPlantAtCoordinates(coordinates);
		bool success = false;
		if (index == -1)
		{
			GD.Print("Plant not found at this position!");
		} else
		{
			Plant foundPlant = plants[index];
			if (foundPlant.GetGrowthPhase() >= new_phase)
			{
				GD.Print("Plant is already in this phase!");
			} else if (new_phase < foundPlant.GetGrowthPhase())
			{
				GD.Print("Plant cannot grow backwards!");
			} else
			{
				GD.Print("Changing plant phase!");
				Dictionary<int, Vector2I> phasesOfSelectedPlant = plant_growth_phases_by_name.GetValueOrDefault(foundPlant.GetName());
				if (phasesOfSelectedPlant != null)
				{
					foundPlant.SetGrowthPhase(new_phase);
					Vector2I correctTile = phasesOfSelectedPlant.GetValueOrDefault(new_phase);
					if (foundPlant.GetIsWatered() && new_phase < phasesOfSelectedPlant.Count)
					{
						SetCell(foundPlant.GetCoordinates(), farm_source_id, correctTile, 1);
						success = true;
					} else
					{
						SetCell(foundPlant.GetCoordinates(), farm_source_id, correctTile, 0);
						success = true;
					}
				}


				
			}
		}
		return success;
	}

	private void InitializePlantTypesAndPhases()
	{
		plant_growth_phases_by_name = new Dictionary<string, Dictionary<int, Vector2I>>();
		Dictionary<int, Vector2I> pineAppleDic = new()

        {
            { 1, new Vector2I(2, 0) },
            { 2, new Vector2I(3, 0) },
            { 3, new Vector2I(4, 0) },
            { 4, new Vector2I(5, 0) }
        };
		plant_growth_phases_by_name.Add("pineapple", pineAppleDic);

		Dictionary<int, Vector2I> watermelonDic = new()

        {
            { 1, new Vector2I(2, 0) },
            { 2, new Vector2I(3, 0) },
            { 3, new Vector2I(4, 0) },
            { 4, new Vector2I(6, 0) }
        };

		plant_growth_phases_by_name.Add("watermelon", watermelonDic);

		Dictionary<int, Vector2I> mangoDic = new()
        {
            { 1, new Vector2I(2, 0) },
			{ 2, new Vector2I(3, 3) },
            { 3, new Vector2I(4, 3) },
            { 4, new Vector2I(5, 3) },
            { 5, new Vector2I(6, 3) }
        };
		plant_growth_phases_by_name.Add("mango", mangoDic);

		Dictionary<int, Vector2I> chiliDic = new()
		{
			{1, new Vector2I(2,0)},
			{2, new Vector2I(3,1)},
			{3, new Vector2I(4,1)}
		};
		plant_growth_phases_by_name.Add("chili", chiliDic);

		Dictionary<int, Vector2I> sunflowerDic = new()
		{
			{1, new Vector2I(2,0)},
			{2, new Vector2I(3,0)},
			{3, new Vector2I(6,1)}
		};
		plant_growth_phases_by_name.Add("sunflower", sunflowerDic);




	}

	private void InitializeDroppedPlants()
	{
		dropped_plants_dict = new()
        {
            { "pineapple", new Vector2I(0, 0) },
            { "mango", new Vector2I(1, 0) },
			{"watermelon", new Vector2I(2,0)},
			{"chili", new Vector2I(3,0)},
			{"sunflower", new Vector2I(0,1)},
        };
		
	}

	private void InitializeUpgradeItems()
	{
		upgrade_items_by_name = new Dictionary<string, Dictionary<string, Vector2I>>();
		Dictionary<string, Vector2I> distractionDic = new()

        {
            { "camp_fire", new Vector2I(0, 2) },
            { "noise_maker", new Vector2I(0, 1) },
            { "noise_maker_2", new Vector2I(1, 1) },
            { "noise_maker_3", new Vector2I(2, 1) },
			 { "beehive", new Vector2I(0, 0) },
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

	}

    public void PlaceElephantPoop(Vector2I coordinates)
    {
		DestroyItemAtCoordinates("dropped_plant", coordinates);
        SetCell(coordinates, dropped_poop_id, new Vector2I(0,0));
		elephant_poops.Add(new DroppedPlant(elephant_poops.Count +1, elephant_poop, elephant_poop, coordinates, new Vector2I(0,0), "none", true));
    }

    public void RemoveOldPuddles()
    {
		if(puddles != null && puddles.Count > 0)
		{
			for (int i=0; i < puddles.Count; i++)
			{
				Puddle puddle = puddles[i];
				int days_since_adding = Math.Abs(puddle.GetDayWhenPuddleWasAdded()-LevelManager.Instance.GetCurrentDay());
				if(days_since_adding > 1)
				{
					puddles.Remove(puddle);
					SetCell(puddle.GetCoordinates(),farm_source_id, default_tile_atlas_coords);
				}
			}
			}
		
	}

}
