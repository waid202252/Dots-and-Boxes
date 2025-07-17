using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    WaitingForInput,
    PlayerTurn,
    AITurn,
    GameOver
}

[System.Serializable]
public class GameState
{
    public GamePhase currentPhase;
    public int currentPlayerId;
    public bool isGameOver;
    public int winner; 
    
    public GameState()
    {
        currentPhase = GamePhase.WaitingForInput;
        currentPlayerId = 1;
        isGameOver = false;
        winner = 0;
    }

    public void SwitchPlayer()
    {
        currentPlayerId = currentPlayerId == 1 ? 2 : 1;
    }
}