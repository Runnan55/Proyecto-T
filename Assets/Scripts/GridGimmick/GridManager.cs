using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 5;
    public GameObject tilePrefab;
    public float tileSpacing = 1.5f;
    
    [Header("Wave Configurations")]
    [SerializeField] public List<WaveData> waveLayouts = new List<WaveData>(); // Campo público
    
    private TileController[] tiles;

    private void Start() => GenerateGrid();

    public void GenerateGrid()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        
        tiles = new TileController[gridSize * gridSize];
        
        Vector3 startPosition = transform.position;

        for (int fila = 0; fila < gridSize; fila++)
        {
            for (int columna = 0; columna < gridSize; columna++)
            {
                Vector3 pos = startPosition + new Vector3(
                    columna * tileSpacing,
                    0,
                    (gridSize - 1 - fila) * tileSpacing
                );
                
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tile.name = $"Tile_{columna}_{fila}";
                tiles[fila * gridSize + columna] = tile.GetComponent<TileController>();
            }
        }
        
        InitializeWaveData();
    }

    private void InitializeWaveData()
    {
        foreach (WaveData wave in waveLayouts)
        {
            if(wave.tilesToRaise == null || wave.tilesToRaise.Length != gridSize * gridSize)
            {
                wave.tilesToRaise = new bool[gridSize * gridSize];
                for(int i = 0; i < wave.tilesToRaise.Length; i++) {
                    wave.tilesToRaise[i] = false;
                }
            }
        }
    }

    public void ActivateWave(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= waveLayouts.Count) return;
        
        WaveData wave = waveLayouts[waveIndex];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].ToggleTile(wave.tilesToRaise[i]);
        }
    }
}