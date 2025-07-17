using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Box
{
    public int row;
    public int col;
    public bool isCompleted;
    public int ownerId; // 0=未完成, 1=Player1, 2=Player2

    public Box(int r, int c)
    {
        row = r;
        col = c;
        isCompleted = false;
        ownerId = 0;
    }
}