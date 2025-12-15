using Godot;
using System;
using System.Runtime.CompilerServices;

//Source for this was this YouTube tutorial: https://www.youtube.com/watch?v=4qEOdviP1yA
public partial class FarmTiles : TileMapLayer
{
	// Called when the node enters the scene tree for the first time.
	[Export] private Level1 level1;
	[Export] private Godot.Timer _timer;

	[Export] private TileMapLayer plants;

	[Export] private Player player;

	[Export] private Vector2I _fromCell = new(2,0);

	public override void _Ready()
    {
    }

	private int plantedPlants;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (_timer.IsStopped() && level1.GetDaysLeft() > 0 && plantedPlants > 0)
        {
            plants.SetCell(new Vector2I(0,0), 0, new Vector2I(1,0),0);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("mouse_right_click"))
        {
			
        	Vector2 mousePos = GetLocalMousePosition();
			Vector2I mouse_map_pos = LocalToMap(mousePos);
			Vector2I atlas_coords = GetCellAtlasCoords(mouse_map_pos);
			GD.Print(atlas_coords);
			TileData tile = GetCellTileData(mouse_map_pos);
            bool isPlantable = (bool)tile.GetCustomData("isPlantable");
            bool isAlreadyPlanted = (bool)tile.GetCustomData("isAlreadyPlanted");
			string plantType = (string)tile.GetCustomData("plantType");
			int plantGrowthPhase = (int) tile.GetCustomData("phase");
                
			if(atlas_coords == new Vector2I(0, 0) && isPlantable)
            {
				GD.Print("You can plant your seed here!");
                plants.SetCell(mouse_map_pos,0, new Vector2I(0,0),0);
				plantedPlants++;
				//tile.SetCustomData("isAlreadyPlanted", true);
				//tile.SetCustomData("isPlantable", false);
				tile.SetCustomData("phase", 1);
				GD.Print(plantedPlants);
            }
        }
		//GD.Print(mouse)
    }

}
