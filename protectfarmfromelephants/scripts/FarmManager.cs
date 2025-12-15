using Godot;
using System;

public partial class FarmManager : Node2D
{
	private int days_left = 5;

    private int plant_phase = 1; 
	private RichTextLabel _timer_text;
	private Timer _timer;

    private int farm_source_id;
	
    private Godot.Collections.Array<Vector2I> farm_tile_coordinates;
	[Export] TileMapLayer _farm_tiles;

	[Export] Player _player;
		// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        _timer_text = GetNode<RichTextLabel>("%TimerText");
        _timer_text.Text = "Days left: " + days_left;
		_timer = GetNode<Timer>("%Timer");
        StartTimer();
        farm_tile_coordinates = _farm_tiles.GetUsedCellsById(farm_source_id);

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
		 if (_timer.IsStopped() && days_left > 0 )
        {
            days_left--;
            _timer_text.Text = "Days left: " + days_left;
            //if(plantPhase == 4)
            {
                GD.Print("Your fruit is ready to picked up!");
            }

            /* if(plantPhase < 4)
            {
                plantPhase++;    
            foreach (Vector2I plantedTile in plantedTileCoordinates)
            {
                 _plant_tiles.SetCell(plantedTile,0, new Vector2I(plantPhase,0));
            }
            } */
           // _plant_tiles.SetCell(new Vector2I(1,0),0, new Vector2I(2,0));

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

    private void UpdatePlantStages()
    {
        
    }

}
