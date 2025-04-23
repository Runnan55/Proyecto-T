using UnityEngine;

[System.Serializable]
public class WaveData
{
    public string waveName = "New Wave";
    public bool[] tilesToRaise;

    public WaveData(int gridSize)
    {
        tilesToRaise = new bool[gridSize * gridSize];
        for(int i = 0; i < tilesToRaise.Length; i++) {
            tilesToRaise[i] = false;
        }
    }
}