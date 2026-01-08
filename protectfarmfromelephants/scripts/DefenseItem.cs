//Code for all items that player can use to defend their farm from elephants.
//Needed assets: simple fence, electric fence and sunflower, chili peppers.

using Godot;
using System;

public class DefenseItem 
{
    private int ID;
    private string name;

    private bool is_breakable;
    private int hp; 

    private Vector2I coordinates;
    public DefenseItem(int id, string item_name, bool breakable, int health_amount,  Vector2I coords)
    {
        ID = id; 
        name = item_name;
        is_breakable = breakable;
        hp = health_amount;
        coordinates = coords;

    }


    public int GetID()
    {
        return ID;
    }

    public string GetItemName()
    {
        return name;
    }

    public bool GetIsBreakable()
    {
        return is_breakable;
    }

    public int GetHealth()
    {
        return hp;
    }

    public void SetIsBreakable(bool isBreakable)
    {
        is_breakable = isBreakable;
    }


    public void SetHealth(int num)
    {
       hp = num;
    }

    public Vector2I GetCoordinates()
    {
        return coordinates;
    }

    public void SetCoordinates(Vector2I coordinates)
    {
        coordinates = coordinates;
    }

}