using Godot;
using System;
namespace ProtectFarm;
public class DroppedPlant : PlacedItem {

    private Vector2I AtlasCoords;

    private string EffectOnElephant;

    private int EffectDuration;

    private int NumberOfElephantTouches;
    public DroppedPlant (int id, string name, string type, Vector2I coords, Vector2I atlas_coords, string effect, int effect_duration, int number, bool isPickable) : base(id, name, type, coords, isPickable)
    {
        AtlasCoords = atlas_coords;
        EffectOnElephant = effect;
        EffectDuration = effect_duration;
        NumberOfElephantTouches = number;
    }

    public Vector2I GetDroppedPlantAtlasCoords()
    {
        return AtlasCoords;
    }

    public void SetDroppedPlantAtlasCoords(Vector2I coords)
    {
        AtlasCoords = coords;
    }

    public string GetDroppedPlantEffectOnElephant()
    {
        return EffectOnElephant;
    }

    public void SetDroppedPlantEffectOnElephant(string effect)
    {
        EffectOnElephant = effect;
    }

     public int GetDroppedPlantEffectOnElephantDuration()
    {
        return EffectDuration;
    }

    public void SetDroppedPlantEffectOnElephantDuration(int effect_duration)
    {
        EffectDuration = effect_duration;
    }

    public int GetDroppedPlantNumberOfElephantTouches()
    {
        return NumberOfElephantTouches;
    }

    public void SetDroppedPlantNumberOfElephantTouches(int num)
    {
        NumberOfElephantTouches = num;
    }


}