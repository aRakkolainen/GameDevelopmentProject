using Godot;
using ProtectFarm;

public class Puddle : PlacedItem
{
    private Vector2I coordinates;

    private bool elephant_has_touched;

    private int num_of_elephant_touches;

    public Puddle(int id, string type, string name, Vector2I coor, bool touched, int touches) : base(id, type, name, coor)
    {
        coordinates = coor;
        elephant_has_touched = touched;
        num_of_elephant_touches = touches;
    }

    public bool GetElephantHasTouchedPuddle()
    {
        return elephant_has_touched;
    }

    public void SetElephantHasTouchedPuddle(bool touched)
    {
        elephant_has_touched = touched;
    }

    public int GetNumberOfElephantTouches()
    {
        return num_of_elephant_touches;
    }

    public void SetNumberOfElephantTouches(int num)
    {
        num_of_elephant_touches = num;
    }
}