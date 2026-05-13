using Godot;
namespace ProtectFarm;
using ProtectFarm;
public abstract class PlacedItem
{
	protected int ID;
	protected string Type;

    protected string Name;

	protected Vector2I Coordinates;

	protected PlacedItem (int id, string type, string name, Vector2I coordinates)
    {
        ID = id;
		Type = type;
        Name = name;
		Coordinates = coordinates;
    }

    protected PlacedItem (int id, string name, Vector2I coordinates)
    {
        ID = id;
        Name = name;
		Coordinates = coordinates;
    }

    public int GetID()
    {
        return ID;
    }

    public string GetType()
    {
        return Type;
    }

    public string GetName()
    {
        return Name;
    }

    public Vector2I GetCoordinates()
    {
        return Coordinates;
    }

    public void SetID(int id)
    {
        ID = id;
    }

    public void SetType(string type)
    {
        Type = type;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetCoordinates(Vector2I coords)
    {
        Coordinates = coords;
    }



}