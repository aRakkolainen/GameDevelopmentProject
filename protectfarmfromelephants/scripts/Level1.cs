using System;
using System.Numerics;
using System.Threading;
using Godot;

public partial class Level1 : Node2D
{

    /* [Export] private FarmManager _timer; */
    private int total_days = 5;

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
        /* _timer_text = GetNode<RichTextLabel>("%TimerText");
        _timer_text.Text = "Days left: " + days_left;
        StartTimer();
         */
    }

    private void OnDayEnd()
    {
        _change_day_dialog.Title = "Day " + timer.GetCurrentDay() + " has ended!";
        _change_day_dialog.DialogText = "Sold fruits: " + sold_quota + "/" + quota;
        _change_day_dialog.PopupCentered();
        _player.SetPlayerIsAlive(false);
    }
    
    private void OnDialogConfirmed()
    {
        EndDay();
    }

    private void OnDialogCloseRequested()
    {
        EndDay();
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
            GD.Print("Trying to update tile:" + farm_tile_coordinates[i]);
            _farmManager.UpdatePlantToNextPhase(farm_tile_coordinates[i]);
                
        }
        _player.SetPlayerIsAlive(true);
        --total_days;
        timer.StartTimer(total_days);
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