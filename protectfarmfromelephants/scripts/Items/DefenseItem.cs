//Code for all items that player can use to defend their farm from elephants.
//Needed assets: simple fence, electric fence and sunflower, chili peppers.

using Godot;
using System;
namespace ProtectFarm;
public class DefenseItem : PlacedItem
{

    private bool Is_breakable;
    private int HP;  

    public DefenseItem(int id, string type, string name, Vector2I coordinates, bool breakable, int hp) 
        : base(id, type, name, coordinates)
    {
        Is_breakable = breakable; 
        HP = hp;
    }

    public bool GetIsBreakable()
    {
        return Is_breakable;
    }

    public int GetHealth()
    {
        return HP;
    }

    public void SetIsBreakable(bool isBreakable)
    {
        Is_breakable = isBreakable;
    }

    public void SetHealth(int hp)
    {
        HP = hp;
    }

    public void TakeDamage(int damage)
    {
        if (HP > 0)
        {
            HP -= damage;
        }
    }

}