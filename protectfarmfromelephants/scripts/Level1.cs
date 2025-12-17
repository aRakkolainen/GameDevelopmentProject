using System.Threading;
using Godot;

public partial class Level1 : Node2D
{

    /* [Export] private FarmManager _timer; */
    private int total_days = 5;

    private string plant_type = "pineapple";

    private int quota = 20;

    private bool day_changed = false;

    private RichTextLabel _timer_text;

    [Export] private Player _player;

    [Export] private Elephant _elephant;
    [Export] private TileMapLayer _farmTiles;

    public override void _Ready()
    {
        /* _timer_text = GetNode<RichTextLabel>("%TimerText");
        _timer_text.Text = "Days left: " + days_left;
        StartTimer();
         */
    }

    public override void _Process(double delta)
    {
         
            
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