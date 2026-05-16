using Godot;

namespace ProtectFarm;
public class Plant : PlacedItem
{
	private int GrowthPhase = 0;

    private bool IsWatered;

    private bool IsWateredByElephant;

    private bool IsFertilizedByElephant;

	public Plant (int id, string name, string type, Vector2I coords, bool isPickable, int phase, bool watered, bool wateredByElephant, bool fertilizedByElephant) : base (id, name, type, coords, isPickable)
    {
		GrowthPhase = phase;
        IsWatered = watered;
        IsWateredByElephant = wateredByElephant;
        IsFertilizedByElephant = fertilizedByElephant;
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

public bool GetIsFertilizedByElephant()
    {
        return IsFertilizedByElephant;
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

public void SetIsFertilizedByElephant(bool fertilized)
    {
        IsFertilizedByElephant = fertilized;
    }

}
