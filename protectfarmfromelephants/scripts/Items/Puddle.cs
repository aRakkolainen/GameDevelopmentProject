using Godot;
using ProtectFarm;

public class Puddle : PlacedItem
{
    private Vector2I coordinates;

    private bool elephant_has_touched;

    private int num_of_elephant_touches;

    private int day_when_puddle_was_added;

    public Puddle(int id, string type, string name, Vector2I coor, bool isPickable, bool touched, int touches, int current_day) : base(id, type, name, coor, isPickable)
    {
        coordinates = coor;
        elephant_has_touched = touched;
        num_of_elephant_touches = touches;
        day_when_puddle_was_added = current_day;
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

    public int GetDayWhenPuddleWasAdded()
    {
        return day_when_puddle_was_added;
    }

    public void SetDayWhenPuddleWasAdded(int day)
    {
        day_when_puddle_was_added = day;
    }
}