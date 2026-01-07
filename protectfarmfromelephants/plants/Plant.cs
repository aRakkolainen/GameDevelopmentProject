using Godot;

namespace ProtectFarm;
public class Plant
{
	private int ID;
	private string Plant_type="";
	private int Growth_phase = 0;

	private Vector2I Coordinates;

	public Plant (int id, string name, int phase, Vector2I coordinates)
    {
        ID = id;
		Plant_type = name;
		Growth_phase = phase;
		Coordinates = coordinates;
    }

	public int GetID()
    {
        return ID;
    }

	public string GetPlantType()
    {
        return Plant_type;
    }

	public int GetGrowthPhase()
    {
        return Growth_phase;
    }

public Vector2I GetCoordinates()
    {
        return Coordinates;
    }

public void SetID(int id)
    {
        ID = id;
    }

public void SetPlantType(string type)
    {
        Plant_type = type;
    }
public void SetGrowthPhase(int phase)
    {
        Growth_phase = phase;
    }

public void SetCoordinates(Vector2I coordinates)
    {
        Coordinates = coordinates;
    }

}
