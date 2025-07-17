using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Box
{
    public int row;
    public int col;
    public bool isCompleted;
    public int ownerId; 

    public Box(int r, int c)
    {
        row = r;
        col = c;
        isCompleted = false;
        ownerId = 0;
    }
}