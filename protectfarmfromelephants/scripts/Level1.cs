using System;
using System.Numerics;
using System.Threading;
using Godot;

public partial class Level1 : Node2D
{

    /* [Export] private FarmManager _timer; */
    private int total_days = 5;

    private int current_day = 0;
    private string plant_type = "pineapple";

    private int sold_quota = 0;
    private int quota = 20;

    private bool day_changed = false;

    [Export] private TimeManager timer;

    private RichTextLabel _timer_text;

    private Node2D changeDay; 
    [Export] private AcceptDialog _change_day_dialog;
    [Export] private Player _player;

    [Export] private Elephant _elephant;
    [Export] private FarmManager _farmManager;

    public override void _Ready()
    {
        _farmManager ??= GetNode<FarmManager>("%Farm");
        timer ??= GetNode<TimeManager>("%Timer");
        _change_day_dialog ??= GetNode<AcceptDialog>("%ChangeDayDialog");
        _change_day_dialog.CloseRequested += OnDialogCloseRequested;
        _change_day_dialog.Confirmed += OnDialogConfirmed;
        timer.Connect(TimeManager.SignalName.TimerFinished, new Callable(this, nameof(OnDayEnd)));
        timer.StartTimer(total_days);
    }

    


    private void OnDayEnd()
    {
        timer.SetDaysLeft(timer.GetDaysLeft()-1);
        if (timer.GetDaysLeft() > 0)
        {
            _change_day_dialog.Title = "Day " + timer.GetCurrentDay() + " has ended!";
            timer.SetCurrentDay(timer.GetCurrentDay()+1);
            _change_day_dialog.DialogText = "Sold fruits: " + sold_quota + "/" + quota;
            _change_day_dialog.OkButtonText = "Start new day";
            _player.SetPlayerIsAlive(false);  
        } else if (timer.GetDaysLeft() == 0)
        {
            GD.Print("Time's up!");
            if (sold_quota >= quota)
        {
            GD.Print("You survived and managed to fill the quota!");
            _change_day_dialog.Title = "Day " + timer.GetCurrentDay() + " has ended!";
            _change_day_dialog.DialogText = "You reached the quota and passed this week: " + sold_quota + "/" + quota;
            _change_day_dialog.OkButtonText = "Move to next level";
            
        } else{
            GD.Print("You failed to fill the quota!");
            _change_day_dialog.Title = "Day " + timer.GetCurrentDay() + " has ended!";
            _change_day_dialog.DialogText = "Sold fruits: " + sold_quota + "/" + quota;
            _change_day_dialog.OkButtonText = "Restart";
            ResetLevel();
            
        }
        } else {
                
            }
            //timer.SetCurrentDay(1);
            _change_day_dialog.PopupCentered();
            //timer.SetCurrentDay(1);
    }
    
    private void OnDialogConfirmed()
    {
        if(timer.GetDaysLeft() == 0)
        {
            if (sold_quota < quota)
            {
                _player.Die();
                ResetLevel();
                //To-do instiate death scene
            } else
            {
                GD.Print("Move to next level");
                //To-do: Instiate the next level scene
            }
        } else
        {
            EndDay();
        }
    }

    private void OnDialogCloseRequested()
    {
        if(timer.GetDaysLeft() == 0)
        {
            if (sold_quota < quota)
            {
                _player.Die();
                ResetLevel();
                //To-do instiate death scene
            } else
            {
                GD.Print("Move to next level");
                //To-do: Instiate the next level scene
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
                
        }
        
        timer.StartTimer(timer.GetDaysLeft());
        _player.SetPlayerIsAlive(true);
    }

    private void ResetLevel()
    {
         timer.SetCurrentDay(1);
            timer.StartTimer(total_days);
            Godot.Collections.Array<Vector2I> farm_tile_coordinates = _farmManager.GetFarmTileCoordinates();
            for (int i=0; i < farm_tile_coordinates.Count; i++)
        {
            _farmManager.RemovePlantAtCoordinates(farm_tile_coordinates[i]);  
        }
        _player.ClearInventory();
    }

    public int GetTotalDays()
    {
        return total_days;
    }

    public bool GetDayChanged()
    {
        return day_changed;
    }

    public void SetDayChanged(bool changed)
    {
        day_changed = changed;
    }

    public string GetPlantType()
    {
        return plant_type;
    }

    public int GetPlantQuota()
    {
        return quota;
    }

}