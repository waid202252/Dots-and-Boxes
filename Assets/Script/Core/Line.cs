using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Line
{
    public int row;
    public int col;
    public bool isHorizontal;
    public bool isPlaced;
    public int playerId; 

    public Line(int r, int c, bool horizontal)
    {
        row = r;
        col = c;
        isHorizontal = horizontal;
        isPlaced = false;
        playerId = 0;
    }
}