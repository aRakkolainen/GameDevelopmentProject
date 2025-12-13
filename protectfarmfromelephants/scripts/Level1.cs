using System.Threading;
using Godot;

public partial class Level1 : Node2D
{

    /* [Export] private FarmManager _timer; */
    private int days_left = 5;

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

    public int GetDaysLeft()
    {
        return days_left;
    }

    public bool GetDayChanged()
    {
        return day_changed;
    }

}