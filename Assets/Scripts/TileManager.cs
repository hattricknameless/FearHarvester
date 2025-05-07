using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {
    
    public static TileManager tileManager;
    public Tilemap tilemap;
    public Dictionary<Vector2Int, GameObject> gridLayout;

    private void Awake() {
        tileManager = this;
    }

    private void Start() {
        gridLayout = new();

        Initialization();
    }

    public event Action OnTileManagerInitialized;

    private void Initialization() {
        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles) {
            Vector3 worldPosition = tile.gameObject.transform.position;
            Vector2Int gridPosition = (Vector2Int)tilemap.WorldToCell(worldPosition);
            if (!gridLayout.ContainsKey(gridPosition)) {
                gridLayout.Add(gridPosition, tile.gameObject);
                tile.tilePosition = gridPosition;
            }
        }
        Debug.Log("TileManager Initialization Complete");
        OnTileManagerInitialized?.Invoke();
    }

    public void isPlayerSeen(Vector2Int position) {
        Vector2Int[] offsets = {new(1, 1), new(1, 0), new(1, -1), new(0, 1), new(0, -1), new(-1, 1), new(-1, 0), new(-1, -1)};
        foreach (Vector2Int offset in offsets) {
            if (gridLayout.TryGetValue(position + offset, out GameObject tileObject)) {
                Tile tile = tileObject.GetComponent<Tile>();
                if (!tile.isBrush) {
                    tile.SetExclamationTile();
                }
                if (tile.isPlayerCheck() && !tile.isBrush) {
                    GameEnd(false);
                    Debug.Log($"Player seen at {position + offset}");
                }
            }
        }
    }

    public Action<bool> OnGameEnd;
    public void GameEnd(bool result) {
        OnGameEnd?.Invoke(result);
    }
}
