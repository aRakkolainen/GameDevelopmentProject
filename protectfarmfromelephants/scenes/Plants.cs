using Godot;
using System;

public partial class Plants : TileMapLayer
{

	[Export] private Godot.Timer _timer;

	[Export] private Level1 level1;
	private int plantedPlants;

	private int plantSourceId = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		/* if (_timer.IsStopped() && level1.GetDaysLeft() > 0 && plantedPlants > 0)
        {
            SetCell(new Vector2I(0,0), 0, new Vector2I(1,0),0);
        } */
	}
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("mouse_right_click"))
        {
			
        	Vector2 mousePos = GetLocalMousePosition();
			Vector2I mouse_map_pos = LocalToMap(mousePos);
			int sourceId = GetCellSourceId(mouse_map_pos);
			if (sourceId > -1)
			{
				/* if (source is TileSetScenesCollectionSource sceneSource)
				{
					int altId = GetCellAlternativeTile(mouse_map_pos);
					PackedScene scene = sceneSource.GetSceneTileScene(altId);
					int id = sceneSource.CreateSceneTile(scene);
					SetCell(mouse_map_pos,plantSourceId, new Vector2I(0,0), id);
				} */
			} 

			TileSetSource source = TileSet.GetSource(sourceId);
			GD.Print(source);
			//TileData tile = GetCellTileData(mouse_map_pos);
            //bool isPlantable = (bool)tile.GetCustomData("isPlantable");
            //bool isAlreadyPlanted = (bool)tile.GetCustomData("isAlreadyPlanted");
			//string plantType = (string)tile.GetCustomData("plantType");
			//int plantGrowthPhase = (int) tile.GetCustomData("phase");
                
				GD.Print("You can plant your seed here!");
                SetCell(mouse_map_pos,plantSourceId, new Vector2I(0,0), 1);
				plantedPlants++;
				//tile.SetCustomData("isAlreadyPlanted", true);
				//tile.SetCustomData("isPlantable", false);
				//tile.SetCustomData("phase", 1);
				GD.Print(plantedPlants);
            }
    }

}