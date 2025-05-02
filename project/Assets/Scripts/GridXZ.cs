using UnityEngine;
using CodeMonkey.Utils;
using Unity.Collections;
using System;
using System.Collections.Generic;

public class GridXZ<TGridObject> {

    private bool isDebugMode = false;

    private Vector2Int gridSize;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    private TextMesh[,] debugTextArray;

    public GridXZ(Vector2Int gridSize, float cellSize, Vector3 originPosition, Func<GridXZ<TGridObject>, Vector2Int, TGridObject> createGridObject) {
        this.gridSize = gridSize;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[gridSize.x, gridSize.y];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                gridArray[x, z] = createGridObject(this, new Vector2Int(x, z));
            }
        }

        if (isDebugMode) { // Enable Gizmos in Game tab to see Debug.Drawline
            debugTextArray = new TextMesh[gridSize.x, gridSize.y];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int z = 0; z < gridArray.GetLength(1); z++) {
                    debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(new Vector2Int(x, z)) + new Vector3(cellSize * .5f, 0, cellSize * .5f), 20, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(new Vector2Int(x, z)), GetWorldPosition(new Vector2Int(x, z+1)), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(new Vector2Int(x, z)), GetWorldPosition(new Vector2Int(x+1, z)), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(new Vector2Int(0, gridSize.y)), GetWorldPosition(new Vector2Int(gridSize.x, gridSize.y)), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(new Vector2Int(gridSize.x, 0)), GetWorldPosition(new Vector2Int(gridSize.x, gridSize.y)), Color.white, 100f);
        }
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector2Int GetGridSize() {
        return gridSize;
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x * cellSize, 0, gridPosition.y * cellSize) + originPosition;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        int y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        if (x >= 0 && y >= 0 && x < gridSize.x && y < gridSize.y) {
            return new Vector2Int(x, y);
        } else {
            return new Vector2Int(-1, -1);
        }
    }

    public void SetGridObject(Vector2Int gridPosition, TGridObject value) {
        if (gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < gridSize.x && gridPosition.y < gridSize.y) {
            gridArray[gridPosition.x, gridPosition.y] = value;

            if (isDebugMode) {
                debugTextArray[gridPosition.x, gridPosition.y].text = gridArray[gridPosition.x, gridPosition.y].ToString();
            }
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        Vector2Int gridPosition = GetGridPosition(worldPosition);
        SetGridObject(gridPosition, value);
    }

    public TGridObject GetGridObject(Vector2Int gridPosition) {
        if (gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < gridSize.x && gridPosition.y < gridSize.y) {
            return gridArray[gridPosition.x, gridPosition.y];
        } else {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        return GetGridObject(GetGridPosition(worldPosition));
    }

    public bool HasGridObject(Vector2Int gridPosition) {
        return GetGridObject(gridPosition) != null;
    }

    public List<TGridObject> GetAllGridObjects() {
        List<TGridObject> allGridObjects = new List<TGridObject>();
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                if (HasGridObject(new Vector2Int(x, y))) {
                    allGridObjects.Add(GetGridObject(new Vector2Int(x, y)));
                }
            }
        }
        return allGridObjects;
    }

    public Vector2Int GetCenterGridPosition() {
        int x = (int)Math.Floor(this.gridSize.x / 2.0);
        int y = (int)Math.Floor(this.gridSize.y / 2.0);
        return new Vector2Int(x, y);
    }

    public void PrintGrid() {
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                Debug.Log("("+x+","+z+"): "+GetGridObject(new Vector2Int(x, z)));
            }
        }
    }
}