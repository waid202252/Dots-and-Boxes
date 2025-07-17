using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public int playerId;
    public string playerName;
    public int score;
    public bool isAI;
    public Color playerColor;

    public Player(int id, string name, bool ai = false)
    {
        playerId = id;
        playerName = name;
        score = 0;
        isAI = ai;
        playerColor = id == 1 ? Color.blue : Color.red;
    }

    public void AddScore(int points = 1)
    {
        score += points;
    }
}