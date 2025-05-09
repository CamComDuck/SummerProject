using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.AI.Navigation;
using UnityEngine.AI;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using TMPro;

public class GridBuilding : MonoBehaviour {

    public static GridBuilding Instance { get; private set;}
    public static GridXZ<BiomeTile> WorldBiomes { get; private set;}

    public List<BiomeSO> biomeSOs = new List<BiomeSO>();

    private Directions placingDirection = Directions.Down;
    private BiomeSelectionPanel biomeSelectionPanel;
    private ToolSelectionPanel toolSelectionPanel;

    public enum Directions {
        Down,
        Left,
        Up,
        Right
    }

    private void Awake() {
        if (Instance != null) Debug.LogError("There is more than one GridBuildingSystem instance!");
        Instance = this;

        const int gridSize = 50;
        const int tileSize = 10;
        Vector3 origin = new Vector3((gridSize * -1 * 0.5f) * tileSize, 0, (gridSize * -1 * 0.5f) * tileSize);

        WorldBiomes = new GridXZ<BiomeTile>(new Vector2Int(gridSize, gridSize), tileSize, origin, (GridXZ<BiomeTile> g, Vector2Int v) => new BiomeTile(g, v));
    }

    private void Start() {
        GameInput.OnCameraRotatePlacedPerformed += GameInput_OnRotatePlacedPerformed;
        GameInput.OnPlacePerformed += GameInput_OnPlacePerformed;
        biomeSelectionPanel = FindFirstObjectByType<BiomeSelectionPanel>();
        toolSelectionPanel = FindFirstObjectByType<ToolSelectionPanel>();

        Vector3 worldPosition = WorldBiomes.GetWorldPosition(WorldBiomes.GetCenterGridPosition());
        PlacedBiome placedBiome = PlacedBiome.Create(worldPosition, WorldBiomes.GetCenterGridPosition(), placingDirection, biomeSOs[0]);
        WorldBiomes.GetGridObject(WorldBiomes.GetCenterGridPosition()).SetBiomeObject(placedBiome);
    }

    private void GameInput_OnRotatePlacedPerformed(object sender, System.EventArgs e) {
        ChangeObjectRotation();
    }

    private void GameInput_OnPlacePerformed(object sender, System.EventArgs e) {
        if (toolSelectionPanel.GetSelectedTool() != ToolSelectionPanel.Tools.Place) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit)) {
            Vector2Int gridPosition = WorldBiomes.GetGridPosition(raycastHit.point);
            PlaceBiomeOnGrid(WorldBiomes, gridPosition, biomeSelectionPanel.GetSelectedBiome());
        }
    }

    public bool PlaceBiomeOnGrid(GridXZ<BiomeTile> biomeGrid, Vector2Int placingGridPosition, BiomeSO biomeSO) { // Returns if object was placed sucessfully
        if (placingGridPosition.x >= 0 && placingGridPosition.y >= 0) {
            if (biomeGrid.GetGridObject(placingGridPosition) == null) {
                // Debug.LogWarning("Biome would be partly outside grid!");
                return false;
            } else if (biomeGrid.GetGridObject(placingGridPosition).HasBiomeObject()) {
                // Debug.LogWarning("Biome already on this grid tile!");
                return false;
            }

            List<BiomeTile> neighborBiomes = WorldBiomes.GetNeighborGridObjects(placingGridPosition);
            bool foundNeighbor = false;
            foreach (BiomeTile biomeTile in neighborBiomes) {
                if (biomeTile != null) {
                    if (biomeTile.HasBiomeObject()) {
                        foundNeighbor = true;
                        break;
                    }
                }
            }
            
            if (!foundNeighbor) {
                // Debug.LogWarning("There are no biome tile neighbors");
                return false;
            }

            Vector3 worldPosition = biomeGrid.GetWorldPosition(placingGridPosition);
            PlacedBiome placedBiome = PlacedBiome.Create(worldPosition, placingGridPosition, placingDirection, biomeSO);
            biomeGrid.GetGridObject(placingGridPosition).SetBiomeObject(placedBiome);
            
            return true;
            
        } else {
            Debug.LogWarning("Clicked outside of grid!");
            return false;
        }
    }

    public void DestroyObjectOnGrid(GridXZ<BiomeTile> grid, BiomeTile biome) {
        PlacedBiome placedBiome = biome.GetBiomeObject();
        if (placedBiome != null) {
            placedBiome?.DestroySelf();
            grid.GetGridObject(placedBiome.GetGridPosition()).ClearBiomeObject();
        }
    }

    public void DestroyObjectOnGrid(GridXZ<BiomeTile> grid, PlacedBiome placedBiome) {
        if (placedBiome != null) {
            placedBiome?.DestroySelf();
            grid.GetGridObject(placedBiome.GetGridPosition()).ClearBiomeObject();
        }
    }

    private void ChangeObjectRotation() {
        placingDirection = GetNextDir(placingDirection);
    }

    public void UpdateNavMesh() {
        NavMeshSurface navMeshSurface = this.GetComponent<NavMeshSurface>();
        if (navMeshSurface == null) navMeshSurface = this.AddComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }

    private int GetNavMeshAgentID(string name) {
        for (int i = 0; i < NavMesh.GetSettingsCount(); i++) {
            NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(index: i);
            if (name == NavMesh.GetSettingsNameFromID(agentTypeID: settings.agentTypeID)) {
                return settings.agentTypeID;
            }
        }
        return 0;
    }

    public static Directions GetNextDir(Directions dir) {
        switch (dir) {
            default:
            case Directions.Down:  return Directions.Left;
            case Directions.Left:  return Directions.Up;
            case Directions.Up:    return Directions.Right;
            case Directions.Right: return Directions.Down;
        }
    }

    public int GetRotationAngle(Directions dir) {
        switch (dir) {
            default:
            case Directions.Down:  return 0;
            case Directions.Left:  return 90;
            case Directions.Up:    return 180;
            case Directions.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Directions dir, Vector2Int gridSize) {
        switch (dir) {
            default:
            case Directions.Down:  return new Vector2Int(0, 0);
            case Directions.Left:  return new Vector2Int(0, gridSize.x);
            case Directions.Up:    return new Vector2Int(gridSize.x, gridSize.y);
            case Directions.Right: return new Vector2Int(gridSize.y, 0);
        }
    }

    public List<Vector2Int> GetObjectGridPositions(Vector2Int offset, Directions dir, Vector2Int objectGridSize) {
        List<Vector2Int> objectGridPositions = new List<Vector2Int>();
        switch (dir) {
            default:
            case Directions.Down:
            case Directions.Up:
                for (int x = 0; x < objectGridSize.x; x++) {
                    for (int y = 0; y < objectGridSize.y; y++) {
                        objectGridPositions.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Directions.Left:
            case Directions.Right:
                for (int x = 0; x < objectGridSize.y; x++) {
                    for (int y = 0; y < objectGridSize.x; y++) {
                        objectGridPositions.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return objectGridPositions;
    }
}