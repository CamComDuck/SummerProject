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
    [SerializeField] private Material validBuildMaterial;
    [SerializeField] private Material invalidBuildMaterial;

    private Directions placingDirection = Directions.Down;
    private BiomeSelectionPanel biomeSelectionPanel;
    private ToolSelectionPanel toolSelectionPanel;
    private GridXZ<BiomeTile> worldBiomesGrid;

    private Transform biomeGhost;
    private BiomeSO biomeGhostSO;
    private MeshRenderer biomeGhostMeshRenderer;

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
        biomeSelectionPanel = FindFirstObjectByType<BiomeSelectionPanel>();
        toolSelectionPanel = FindFirstObjectByType<ToolSelectionPanel>();

        PlaceBiomeOnGrid(worldBiomesGrid.GetCenterGridPosition(), biomeSOs[0]);
    }

    private void Update() {
        if (toolSelectionPanel.IsSelectedToolAction(ToolSO.Action.Select)) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit)) {
            Vector2Int gridPosition = worldBiomesGrid.GetGridPosition(raycastHit.point);

            if (toolSelectionPanel.IsSelectedToolAction(ToolSO.Action.Place)) { // PLACE tool
                if (Input.GetMouseButton(0)) {
                    bool isValid = IsValidBuildPosition(gridPosition);
                    if (isValid) {
                        PlaceBiomeOnGrid(gridPosition, biomeSelectionPanel.GetSelectedBiome());

                        toolSelectionPanel.SetDestroyEnabled(true);
                        DestroyBiomeGhost();
                    }   
                } 

                Vector3 worldPosition = worldBiomesGrid.GetWorldPosition(gridPosition);

                if (biomeGhostSO != biomeSelectionPanel.GetSelectedBiome()) {
                    DestroyBiomeGhost();
                }

                if (biomeGhost == null) {
                    biomeGhostSO = biomeSelectionPanel.GetSelectedBiome();
                    biomeGhost = Instantiate(biomeGhostSO.GetPrefab(), worldPosition, Quaternion.Euler(0, GetRotationAngle(placingDirection), 0));
                    biomeGhostMeshRenderer = biomeGhost.GetComponentInChildren<MeshRenderer>();
                } 

                biomeGhost.transform.position = worldPosition;
                
                if (IsValidBuildPosition(gridPosition)) {
                    List<Material> materials = new() { biomeGhostMeshRenderer.material, validBuildMaterial};
                    biomeGhostMeshRenderer.SetMaterials(materials);
                
                } else {
                    List<Material> materials = new() { biomeGhostMeshRenderer.material, invalidBuildMaterial};
                    biomeGhostMeshRenderer.SetMaterials(materials);
                }              

            } else if (toolSelectionPanel.IsSelectedToolAction(ToolSO.Action.Destroy)) { // DESTROY tool
                DestroyBiomeGhost();

                if (Input.GetMouseButton(0)) {
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
    }

    private void GameInput_OnRotatePlacedPerformed(object sender, System.EventArgs e) {
        ChangeObjectRotation();
    }

    private void DestroyBiomeGhost() {
        if (biomeGhost != null) {
            Destroy(biomeGhost.gameObject);
            biomeGhost = null;
            biomeGhostSO = null;
            biomeGhostMeshRenderer = null;
        }
    }

    public bool IsValidBuildPosition(Vector2Int placingGridPosition) {
        if (placingGridPosition.x >= 0 && placingGridPosition.y >= 0) {
            if (worldBiomesGrid.GetGridObject(placingGridPosition) == null) {
                // Debug.LogWarning("Biome would be partly outside grid!");
                return false;
            } else if (worldBiomesGrid.GetGridObject(placingGridPosition).HasBiomeObject()) {
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
            
            return true;
            
        } else {
            Debug.LogWarning("Outside of grid!");
            return false;
        }
    }

    private void PlaceBiomeOnGrid(Vector2Int gridPosition, BiomeSO biome) {
        Vector3 worldPosition = worldBiomesGrid.GetWorldPosition(gridPosition);
        PlacedBiome placedBiome = PlacedBiome.Create(worldPosition, gridPosition, placingDirection, biome);
        worldBiomesGrid.GetGridObject(gridPosition).SetBiomeObject(placedBiome);
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
            Debug.LogWarning("Outside of grid!");
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