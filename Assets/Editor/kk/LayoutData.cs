using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LayoutData", menuName = "ScriptableObjects/LayoutData", order = 1)]
public class LayoutData : ScriptableObject {
    public int rows = 4;
    public int cols = 4;
    public bool[] grid;

    public void InitGrid() {
        if (grid == null || grid.Length != rows * cols) {
            grid = new bool[rows * cols];
        }
    }
}