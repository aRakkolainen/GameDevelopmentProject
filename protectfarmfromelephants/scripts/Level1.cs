using System.Threading;
using Godot;

public partial class Level1 : Node2D
{

    [Export] private TimeManager _timer;
    private int days_left = 5;

    private bool day_changed = false;

    private RichTextLabel _timer_text;

    [Export] private Player _player;

    [Export] private Elephant _elephant;
    [Export] private TileMapLayer _farmTiles;

    public override void _Ready()
    {
        _timer_text = GetNode<RichTextLabel>("%TimerText");
        _timer_text.Text = "Days left: " + days_left;
        StartTimer();
        
    }

    public override void _Process(double delta)
    {
           if (_timer.IsStopped() && days_left > 0 )
        {
            days_left--;
            _timer_text.Text = "Days left: " + days_left;
            _farmTiles.SetCell(new Vector2I(2,0),0, new Vector2I(3,0),0);
            GD.Print(_timer_text.Text);
            StartTimer();

        } else if (days_left == 0 && _player.GetPlayerIsAlive())
        {
            _timer.Stop();
            _player.Die();
        }
            
    }

    private void StartTimer()
    {
        _timer.Start();
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