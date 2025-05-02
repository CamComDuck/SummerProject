using UnityEngine;

public class BiomeTile {
        
    private GridXZ<BiomeTile> grid;
    private Vector2Int gridPosition;
    private PlacedBiome biome = null;

    public BiomeTile(GridXZ<BiomeTile> grid, Vector2Int gridPosition) {
        this.grid = grid;
        this.gridPosition = gridPosition;
    }

    public void SetBiomeObject(PlacedBiome biome) {
        this.biome = biome;
        grid.SetGridObject(gridPosition, this);
    }

    public PlacedBiome GetBiomeObject() {
        return this.biome;
    }

    public void ClearBiomeObject() {
        SetBiomeObject(null);
    }

    public bool HasBiomeObject() {
        return biome != null;
    }

    public override string ToString() {
        return "(" + gridPosition.x + ", " + gridPosition.y + ")\n" + this.biome?.GetBiomeSO().GetName();
        // return "(" + gridPosition.x + ", " + gridPosition.y + "): " + this.biome?.GetBiomeSO().GetName();
        // return this.placedObject?.GetFurnitureObject().GetFurnitureObjectSO().GetObjectName();
    }

}