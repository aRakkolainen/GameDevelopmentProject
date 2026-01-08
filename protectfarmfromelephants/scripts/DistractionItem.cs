//Code for objects that are used by player to distract elephants to use different route than the straight line.
// Assets will be: noise maker machine, fruits to throw, beehive

//Code for all items that player can use to defend their farm from elephants.
//Needed assets: simple fence, electric fence and sunflower, chili peppers.

using Godot;
using System;

public class DistractionItem 
{
    private int ID;
    private string name;

    private bool is_breakable;
    private int effect_duration;
    private int effect_range;

    private bool is_hostile;

    private int damage; 


    private Vector2I coordinates;

    public DistractionItem(int id, string item_name, bool breakable, int duration, int range, bool isHostile, int damage_amount, Vector2I coords)
    {
        ID = id; 
        name = item_name;
        is_breakable = breakable;
        effect_duration = duration;
        effect_range = range;
        is_hostile = isHostile;
        damage = damage_amount; 
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


    public void SetIsBreakable(bool isBreakable)
    {
        is_breakable = isBreakable;
    }

    public int GetEffectDuration()
    {
        return effect_duration;
    }

    public void SetEffectDuration(int seconds)
    {
       effect_duration = seconds;
    }

     public int GetEffectRange()
    {
        return effect_range;
    }

    public void SetEffectRange(int tiles)
    {
       effect_range = tiles;
    }


    public bool GetIsHostile()
    {
        return is_hostile;
    }

    public void SetIsHostile(bool isHostile)
    {
        is_breakable = isHostile;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int num)
    {
       damage = num;
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