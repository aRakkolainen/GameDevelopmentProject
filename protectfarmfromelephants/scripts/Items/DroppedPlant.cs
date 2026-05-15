using Godot;
using System;
namespace ProtectFarm;
public class DroppedPlant : PlacedItem {

    private Vector2I AtlasCoords;

    private string EffectOnElephant;
    public DroppedPlant (int id, string name, string type, Vector2I coords, Vector2I atlas_coords, string effect, bool isPickable) : base(id, name, type, coords, isPickable)
    {
        AtlasCoords = atlas_coords;
        EffectOnElephant = effect;
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


}