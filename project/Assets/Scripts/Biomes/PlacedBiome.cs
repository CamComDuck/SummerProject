using System.Collections.Generic;
using UnityEngine;

public class PlacedBiome : MonoBehaviour {
    
    private Vector2Int gridPosition;
    private GridBuilding.Directions dir;
    private BiomeSO biomeSO;

    public static PlacedBiome Create(Vector3 worldPosition, Vector2Int gridPosition, GridBuilding.Directions dir, BiomeSO newBiomeSO) {
        GridBuilding gridBuilding = FindFirstObjectByType<GridBuilding>();
        Transform placedObjectTransform = Instantiate(newBiomeSO.GetPrefab(), worldPosition, Quaternion.Euler(0, gridBuilding.GetRotationAngle(dir), 0));

        PlacedBiome placedObject = placedObjectTransform.GetComponent<PlacedBiome>();

        placedObject.gridPosition = gridPosition;
        placedObject.dir = dir;
        placedObject.biomeSO = newBiomeSO;
        return placedObject;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public Vector2Int GetGridPosition() {
        return gridPosition;
    }

    public BiomeSO GetBiomeSO() {
        return biomeSO;
    }

    public void SetGridPosition(Vector2Int newPosition) {
        this.gridPosition = newPosition;
    }
}