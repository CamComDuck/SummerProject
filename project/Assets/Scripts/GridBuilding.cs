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

    [SerializeField] private List<BiomeSO> biomeSOs = new List<BiomeSO>();

    private Directions placingDirection = Directions.Down;
    private BiomeSelectionPanel biomeSelectionPanel;
    private ToolSelectionPanel toolSelectionPanel;
    private GridXZ<BiomeTile> worldBiomesGrid;

    public enum Directions {
        Down,
        Left,
        Up,
        Right
    }

    private void Awake() {
        const int gridSize = 50;
        const int tileSize = 10;
        Vector3 origin = new Vector3((gridSize * -1 * 0.5f) * tileSize, 0, (gridSize * -1 * 0.5f) * tileSize);

        worldBiomesGrid = new GridXZ<BiomeTile>(new Vector2Int(gridSize, gridSize), tileSize, origin, (GridXZ<BiomeTile> g, Vector2Int v) => new BiomeTile(g, v));
    }

    private void Start() {
        GameInput.OnCameraRotatePlacedPerformed += GameInput_OnRotatePlacedPerformed;
        GameInput.OnClickPerformed += GameInput_OnClickPerformed;
        biomeSelectionPanel = FindFirstObjectByType<BiomeSelectionPanel>();
        toolSelectionPanel = FindFirstObjectByType<ToolSelectionPanel>();

        Vector3 worldPosition = worldBiomesGrid.GetWorldPosition(worldBiomesGrid.GetCenterGridPosition());
        PlacedBiome placedBiome = PlacedBiome.Create(worldPosition, worldBiomesGrid.GetCenterGridPosition(), placingDirection, biomeSOs[0]);
        worldBiomesGrid.GetGridObject(worldBiomesGrid.GetCenterGridPosition()).SetBiomeObject(placedBiome);
    }

    private void GameInput_OnRotatePlacedPerformed(object sender, System.EventArgs e) {
        ChangeObjectRotation();
    }

    private void GameInput_OnClickPerformed(object sender, System.EventArgs e) {
        if (toolSelectionPanel.IsSelectedToolAction(ToolSO.Action.Select)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit)) {
            Vector2Int gridPosition = worldBiomesGrid.GetGridPosition(raycastHit.point);

            if (toolSelectionPanel.IsSelectedToolAction(ToolSO.Action.Place)) {
                bool isPlaced = PlaceBiomeOnGrid(worldBiomesGrid, gridPosition, biomeSelectionPanel.GetSelectedBiome());
                if (isPlaced)   toolSelectionPanel.SetDestroyEnabled(true);

            } else if (toolSelectionPanel.IsSelectedToolAction(ToolSO.Action.Destroy)) {
                DestroyObjectOnGrid(worldBiomesGrid, gridPosition);

                int count = 0;
                foreach (BiomeTile biomeTile in worldBiomesGrid.GetAllGridObjects()) {
                    if (biomeTile.HasBiomeObject()) count += 1;
                }

                if (count <= 1) { // Can't destroy the only tile
                    toolSelectionPanel.SetDestroyEnabled(false);
                } 
            }
            
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

            List<BiomeTile> neighborBiomes = worldBiomesGrid.GetNeighborGridObjects(placingGridPosition);
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

    public bool DestroyObjectOnGrid(GridXZ<BiomeTile> grid, BiomeTile biome) {
        PlacedBiome placedBiome = biome.GetBiomeObject();
        return DestroyObjectOnGrid(grid, placedBiome);
    }

    public bool DestroyObjectOnGrid(GridXZ<BiomeTile> grid, PlacedBiome placedBiome) {
        if (placedBiome != null) {
            placedBiome.DestroySelf();
            grid.GetGridObject(placedBiome.GetGridPosition()).ClearBiomeObject();
            return true;
        } else {
            return false;
        }
    }

    public bool DestroyObjectOnGrid(GridXZ<BiomeTile> grid, Vector2Int gridPosition) {
        if (gridPosition.x >= 0 && gridPosition.y >= 0) {
            if (grid.HasGridObject(gridPosition)) {
                BiomeTile placedBiome = grid.GetGridObject(gridPosition);
                return DestroyObjectOnGrid(grid, placedBiome);
            } else {
                return false;
            }
        } else {
            Debug.LogWarning("Clicked outside of grid!");
            return false;
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

    public GridXZ<BiomeTile> GetWorldBiomesGrid() {
        return worldBiomesGrid;
    }
}