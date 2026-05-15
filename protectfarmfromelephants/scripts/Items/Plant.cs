using Godot;

namespace ProtectFarm;
public class Plant : PlacedItem
{
	private int GrowthPhase = 0;

    private bool IsWatered;

    private bool IsWateredByElephant;

	public Plant (int id, string type, Vector2I coords, bool isPickable, int phase, bool watered, bool wateredByElephant) : base (id, type, coords, isPickable)
    {
		GrowthPhase = phase;
        IsWatered = watered;
        IsWateredByElephant = wateredByElephant;
    }

	public int GetGrowthPhase()
    {
        return GrowthPhase;
    }


public bool GetIsWatered()
    {
        return IsWatered;
    }

public bool GetIsWateredByElephant()
    {
        return IsWateredByElephant;
    }

public void SetGrowthPhase(int phase)
    {
        GrowthPhase = phase;
    }


public void SetIsWatered(bool watered)
    {
        IsWatered = watered;
    }

public void SetIsWateredByElephant(bool watered)
    {
        IsWateredByElephant = watered;
    }

}
