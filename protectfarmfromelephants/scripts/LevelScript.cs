using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Godot;
namespace ProtectFarm;

public partial class LevelScript : Node2D
{
    [Export]
    private int level_num;
    
    private LevelData level;

    [Export] private TimeManager timer;

    private RichTextLabel _timer_text;

    private int total_enemies;

    private int spawned_enemies;
    private Node2D changeDay; 
    [Export] private AcceptDialog _change_day_dialog;
    [Export] private Player _player;

    [Export] SellPopup _sell_popup;

    [Export] private Elephant _elephant;
    [Export] private FarmManager _farmManager;

    [Export] private Godot.Timer _elephant_timer;

    [Signal] public delegate void SellPopUpOpenedEventHandler();

    [Export]
    public PackedScene ElephantScene { get; set; }

    public List<string> elephant_move_directions;

    private string elephant_move_direction;

    private PathFollow2D elephantSpawnLocation;

    private Godot.Vector2 elephantMoveDirection;

    //How far from the farm elephants spawn
    private int elephant_spawn_point_from_farm = 15;

    private List<Elephant> spawned_elephants;

    private Node2D elephants;

    public override void _Ready()
    {
        GetTree().Paused = false;
        GD.Randomize();
        level = LevelManager.Instance.GetLevelData(level_num);
        _farmManager = GetNode<FarmManager>("%Farm");
        timer = GetNode<TimeManager>("Timer");
        _elephant_timer = GetNode<Godot.Timer>("ElephantTimer");
        _change_day_dialog = GetNode<AcceptDialog>("%ChangeDayDialog");
        _change_day_dialog.CloseRequested += OnDialogCloseRequested;
        _change_day_dialog.Confirmed += OnDialogConfirmed;
        _sell_popup = GetNode<SellPopup>("SellPopup");
        if(GetTree() != null)
        {
            if (!timer.IsInsideTree())
            {
                AddChild(timer);
            }

            if (!_elephant_timer.IsInsideTree())
            {
                AddChild(_elephant_timer);
            }

                timer.Connect(TimeManager.SignalName.TimerFinished, new Callable(this, nameof(OnDayEnd)));
                timer.StartTimer(level.GetLevelTotalDays());
                _elephant_timer.Start();
        }
        total_enemies = GD.RandRange(level.GetLevelMininumEnemies(), level.GetLevelMaximumEnemies());
        elephants = new Node2D();
        if (!elephants.IsInsideTree())
        {
            AddChild(elephants);
        }
        }


    private void OnDayEnd()
    {
        timer.SetDaysLeft(timer.GetDaysLeft()-1);
        LevelManager.Instance.SetCurrentDay(LevelManager.Instance.GetCurrentDay()+1);
        int sold_quota = level.GetCurrentQuota();
        int expected_quota = level.GetExpectedQuota();
        
        if (timer.GetDaysLeft() > 0)
        {
            _change_day_dialog.Title = "Day " + timer.GetCurrentDay() + " has ended!";
            timer.SetCurrentDay(timer.GetCurrentDay()+1);
            if(sold_quota >= expected_quota)
            {
                 _change_day_dialog.DialogText = "You reached the quota and passed this level: " + sold_quota + "/" + expected_quota;
                 _change_day_dialog.OkButtonText = "Move to next level";
            } else
            {
                _change_day_dialog.DialogText = "Sold fruits: " + sold_quota + "/" + expected_quota;
                _change_day_dialog.OkButtonText = "Start new day";
                _player.SetPlayerIsAlive(false); 
                GetTree().Paused = true;
            }

        } else if (timer.GetDaysLeft() == 0)
        {
            GD.Print("Time's up!");
            if (sold_quota >= expected_quota)
        {
             GetTree().Paused = true;
            GD.Print("You survived and managed to fill the quota!");
            _change_day_dialog.Title = "Day " + timer.GetCurrentDay() + " has ended!";
            _change_day_dialog.DialogText = "You reached the quota and passed this week: " + sold_quota + "/" + expected_quota;
            _change_day_dialog.OkButtonText = "Move to next level";
            
        } else{
            GD.Print("You failed to fill the quota!");
            LevelManager.Instance.SetPlayerHasFailed(true);
            LevelManager.Instance.LoadScene(Scenes.CutScenes.between_levels_animations_scene);
        }
        } else {
                
            }
            _change_day_dialog.PopupCentered();
    
    }
    
    private void OnDialogConfirmed()
    {
        // LevelData level1 = LevelManager.Instance.GetLevelData("level_1");
        int sold_quota = level.GetCurrentQuota();
        int expected_quota = level.GetExpectedQuota();
        if (sold_quota >= expected_quota)
        {
            LoadInBetweenLevelsAnimation();
        }

        if(timer.GetDaysLeft() == 0)
        {
            if (sold_quota < expected_quota)
            {
                _player.Die();
                LevelManager.Instance.LoadScene(Scenes.CutScenes.between_levels_animations_scene);
                LevelManager.Instance.SetPlayerHasFailed(true);
                ResetLevel();
            } else
            {
                LoadInBetweenLevelsAnimation();
            }
        } else
        {
            EndDay();
        }
    }

    private void LoadInBetweenLevelsAnimation()
    {
        GD.Print("Move to next level after animation");
        LevelManager.Instance.LoadScene(Scenes.CutScenes.between_levels_animations_scene);
    }


    private void OnDialogCloseRequested()
    {
        int sold_quota = level.GetCurrentQuota();
        int expected_quota = level.GetExpectedQuota();
        if(timer.GetDaysLeft() == 0)
        {
            if (sold_quota < expected_quota)
            {
                _player.Die();
                ResetLevel();
                //To-do instiate death scene
            } else
            {
                LoadInBetweenLevelsAnimation();
            }
        } else
        {
            EndDay();
        }
        _change_day_dialog.Hide();

    }


    public override void _Process(double delta)
    {

        
         if (timer.GetDayChanged())
        {
            GD.Print("Day has changed!");
            
        }
            
    }


    private void EndDay()
    {
        Godot.Collections.Array<Vector2I> farm_tile_coordinates = _farmManager.GetFarmTileCoordinates();
        for (int i=0; i < farm_tile_coordinates.Count; i++)
        {
            GD.Print("Trying to update tile: " + farm_tile_coordinates[i]);
            _farmManager.UpdatePlantToNextPhase(farm_tile_coordinates[i]);
            _farmManager.ResetWateredByElephant();
        }
        _farmManager.RemoveOldPuddles();
        
        timer.StartTimer(timer.GetDaysLeft());
        _player.SetPlayerIsAlive(true);
        if(GetTree() != null)
        {
            GetTree().Paused = false;
        }
        //GetTree().CallGroup("elephants", Node.MethodName.QueueFree);
    }

    private void ResetLevel()
    {
        timer.SetCurrentDay(1);
        timer.StartTimer(level.GetLevelTotalDays());
        Godot.Collections.Array<Vector2I> farm_tile_coordinates = _farmManager.GetFarmTileCoordinates();
        for (int i=0; i < farm_tile_coordinates.Count; i++)
        {
            _farmManager.RemovePlantAtCoordinates(farm_tile_coordinates[i]);  
        }
        LevelManager.Instance.ResetLevel();
    }

    public LevelData GetLevelData()
    {
        return level;
    }


    //Implemented based on Godot tutorial and Copilot discussion about what Nodes should be used for simple object that spawns on its own and moves to specific direction
    private void OnElephantTimerTimeout()
	{
        bool spawnRight = false;
		Elephant elephant = ElephantScene.Instantiate<Elephant>();
        spawned_elephants = new List<Elephant>();
        elephant_move_directions = new List<string>
        {
            "Left",
            "Right"
        };
        elephant_move_direction = elephant_move_directions[GD.RandRange(0,1)];
        if (elephant_move_direction.Equals("Left"))
        {
            elephantMoveDirection = Godot.Vector2.Left;
            spawnRight = true;
        } else
        {
            elephantMoveDirection = Godot.Vector2.Right;
        }
        Vector2I spawnLocation = GetElephantSpawnPoint(elephant_move_direction, spawnRight);

        Godot.Vector2 localSpawnPosition =_farmManager.MapToLocal(spawnLocation) + _farmManager.TileSet.TileSize / 2;
        Godot.Vector2 worldSpawnPosition = _farmManager.ToGlobal(localSpawnPosition);
        elephant.GlobalPosition = worldSpawnPosition;
        GD.Print("Elephant should spawn at location:" + spawnLocation);
        //if(spawned_enemies <= total_enemies)
        //{
		    AddChild(elephant);
            elephant.CollidedWithFarm += _farmManager.OnElephantCollidedWithFarm;
            elephant.CollidedWithItem += _farmManager.OnElephantCollidedWithItem;
            //GD.Print("Listeners:", GetSignalConnectionList("CollidedWithFarm").Count);
            

            elephant.Initialize();
            spawned_enemies++;
            if(elephant is Elephant elephantScript)
            {
                elephantScript.MoveDirection = elephantMoveDirection;
                elephantScript.farm = _farmManager;
                spawned_elephants.Add(elephant);
            }
             _elephant_timer.Start();
        //} else
        //{
        //    _elephant_timer.Stop();
        //}

    }

    private Vector2I GetElephantSpawnPoint(string move_direction, bool spawnRight)
    {
        Godot.Collections.Array<Vector2I> farm_tiles = _farmManager.GetFarmTileCoordinates();
       farm_tiles.Sort();
       Godot.Collections.Array<Vector2I> farm_tiles_left_side = new Godot.Collections.Array<Vector2I>();
       Godot.Collections.Array<Vector2I> farm_tiles_right_side = new Godot.Collections.Array<Vector2I>();
       Vector2I max_value = farm_tiles.Max();
       
       Vector2I min_value = farm_tiles.Min();
       foreach ( Vector2I tile in farm_tiles)
        {
            if (tile.X == max_value.X)
            {
                farm_tiles_right_side.Add(tile);
            }
            else if (tile.X == min_value.X)
            {
                farm_tiles_left_side.Add(tile);
            }

        }
            GD.Print("Farm starts at tiles: " + farm_tiles_left_side);
            GD.Print("Farm ends at tiles: " + farm_tiles_right_side);
            Godot.Collections.Array<Vector2I> elephant_spawn_tiles_left = GetNewSpawnPoints(farm_tiles_left_side, spawnRight);
            Godot.Collections.Array<Vector2I> elephant_spawn_tiles_right = GetNewSpawnPoints(farm_tiles_right_side, spawnRight);
            //Spawnpoint needed from opposite side than the moving direction.
            if (move_direction.Equals("Left"))
            {
                return elephant_spawn_tiles_right[GD.RandRange(0, elephant_spawn_tiles_right.Count-1)];
            } else
            {
                return elephant_spawn_tiles_left[GD.RandRange(0, elephant_spawn_tiles_left.Count-1)];
            }
    }

    private Godot.Collections.Array<Vector2I> GetNewSpawnPoints(Godot.Collections.Array<Vector2I> farm_tiles, bool spawnRight)
    {
        Godot.Collections.Array<Vector2I> elephant_spawn_tiles = new Godot.Collections.Array<Vector2I>();
        for (int i = 0; i < farm_tiles.Count; i++)
        {
            Vector2I farm_tile = farm_tiles[i];
            if (farm_tile.X >= 0 )
            {
                elephant_spawn_tiles.Add(new Vector2I(farm_tile.X + elephant_spawn_point_from_farm, farm_tile.Y));
            } else if(spawnRight && farm_tile.X < 0) {
                elephant_spawn_tiles.Add(new Vector2I(farm_tile.X + elephant_spawn_point_from_farm, farm_tile.Y));
            }
            else
            {
                elephant_spawn_tiles.Add(new Vector2I(farm_tile.X - elephant_spawn_point_from_farm, farm_tile.Y));
            }
        }
        GD.Print(elephant_spawn_tiles);
        return elephant_spawn_tiles;
    }

    public void OnPauseTimer()
    {
       /*  _elephant_timer.Stop();
        _player.Pause();
        timer.PauseTimer(); */
        GetTree().Paused = true;
    }

    public void OnContinueTimer()
    {
       /*  _elephant_timer.Start();
        timer.StartTimer(timer.GetDaysLeft());
        _player.Continue(); */
        GetTree().Paused = false;

    }

}