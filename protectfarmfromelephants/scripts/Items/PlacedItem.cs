using Godot;
namespace ProtectFarm;

using System.Xml;
using ProtectFarm;
public abstract class PlacedItem
{
	protected int ID;
	protected string Type;

    protected string Name;

	protected Vector2I Coordinates;

    protected bool IsPickable;

    protected string TextureUid;

	protected PlacedItem (int id, string type, string name, Vector2I coordinates, bool pickable, string uid)
    {
        ID = id;
		Type = type;
        Name = name;
		Coordinates = coordinates;
        IsPickable = pickable;
        TextureUid = uid;
    }

    protected PlacedItem (int id, string name, Vector2I coordinates, bool pickable, string uid)
    {
        ID = id;
        Name = name;
		Coordinates = coordinates;
        IsPickable = pickable;
        TextureUid = uid;
    }

    protected PlacedItem (int id, string type, string name, Vector2I coordinates, bool pickable)
    {
        ID = id;
        Name = name;
        Type = type;
		Coordinates = coordinates;
        IsPickable = pickable;
    }

    protected PlacedItem (int id, string type, Vector2I coordinates, bool pickable)
    {
        ID = id;
        Type = type;
		Coordinates = coordinates;
        IsPickable = pickable;
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

    public bool GetIsPickable()
    {
        return IsPickable;
    }

    public string GetTextureUid()
    {
        return TextureUid;
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

    public void SetIsPickable(bool pickable)
    {
         IsPickable = pickable;
    }

    public void SetTextureUid(string uid)
    {
        TextureUid = uid;
    }



}