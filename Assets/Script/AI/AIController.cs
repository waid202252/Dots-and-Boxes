using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIController : MonoBehaviour
{
    public enum AIStrategy
    {
        Random,
        Greedy,
        Minimax
    }

    [Header("AI Settings")]
    public AIStrategy strategy = AIStrategy.Greedy;
    public int minimaxDepth = 2; 
    public float thinkingTime = 1.0f;

    public (int row, int col, bool isHorizontal) GetBestMove(GameBoard board)
    {
        List<(int row, int col, bool isHorizontal)> availableMoves = board.GetAvailableMoves();
        
        if (availableMoves.Count == 0)
        {
            Debug.LogWarning("AI: No available moves!");
            return (-1, -1, false);
        }

        Debug.Log($"AI: Calculating best move from {availableMoves.Count} options using {strategy} strategy");

        try
        {
            switch (strategy)
            {
                case AIStrategy.Random:
                    return GetRandomMove(availableMoves);
                case AIStrategy.Greedy:
                    return GetGreedyMove(board, availableMoves);
                case AIStrategy.Minimax:
                    return GetMinimaxMove(board, availableMoves);
                default:
                    return GetRandomMove(availableMoves);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AI Error: {e.Message}");
            return GetRandomMove(availableMoves);
        }
    }

    (int, int, bool) GetRandomMove(List<(int row, int col, bool isHorizontal)> moves)
    {
        var move = moves[Random.Range(0, moves.Count)];
        Debug.Log($"AI Random Move: ({move.Item1}, {move.Item2}, {move.Item3})");
        return move;
    }

    (int, int, bool) GetGreedyMove(GameBoard board, List<(int row, int col, bool isHorizontal)> moves)
    {
        Debug.Log("AI: Using Greedy strategy");

    
        foreach (var move in moves)
        {
            if (WillCompleteBox(board, move.Item1, move.Item2, move.Item3))
            {
                Debug.Log($"AI Greedy Move (Complete Box): ({move.Item1}, {move.Item2}, {move.Item3})");
                return move;
            }
        }

        List<(int, int, bool)> safeMoves = new List<(int, int, bool)>();
        foreach (var move in moves)
        {
            if (!WillCreateOpportunity(board, move.Item1, move.Item2, move.Item3))
            {
                safeMoves.Add(move);
            }
        }

        if (safeMoves.Count > 0)
        {
            var move = safeMoves[Random.Range(0, safeMoves.Count)];
            Debug.Log($"AI Greedy Move (Safe): ({move.Item1}, {move.Item2}, {move.Item3})");
            return move;
        }

        var randomMove = moves[Random.Range(0, moves.Count)];
        Debug.Log($"AI Greedy Move (Random fallback): ({randomMove.Item1}, {randomMove.Item2}, {randomMove.Item3})");
        return randomMove;
    }

    bool WillCompleteBox(GameBoard board, int row, int col, bool isHorizontal)
    {
        if (isHorizontal)
        {
            int boxesCompleted = 0;
            
            if (row > 0)
            {
                int completedSides = CountCompletedSides(board, row - 1, col);
                if (completedSides == 3) boxesCompleted++;
            }
            
            if (row < board.gridSize - 1)
            {
                int completedSides = CountCompletedSides(board, row, col);
                if (completedSides == 3) boxesCompleted++;
            }
            
            return boxesCompleted > 0;
        }
        else
        {
            int boxesCompleted = 0;
            
            if (col > 0)
            {
                int completedSides = CountCompletedSides(board, row, col - 1);
                if (completedSides == 3) boxesCompleted++;
            }
            
            if (col < board.gridSize - 1)
            {
                int completedSides = CountCompletedSides(board, row, col);
                if (completedSides == 3) boxesCompleted++;
            }
            
            return boxesCompleted > 0;
        }
    }

    bool WillCreateOpportunity(GameBoard board, int row, int col, bool isHorizontal)
    {
        if (isHorizontal)
        {
            if (row > 0)
            {
                int completedSides = CountCompletedSides(board, row - 1, col);
                if (completedSides == 2) return true;
            }
            
            if (row < board.gridSize - 1)
            {
                int completedSides = CountCompletedSides(board, row, col);
                if (completedSides == 2) return true;
            }
        }
        else
        {
            if (col > 0)
            {
                int completedSides = CountCompletedSides(board, row, col - 1);
                if (completedSides == 2) return true;
            }
            
            if (col < board.gridSize - 1)
            {
                int completedSides = CountCompletedSides(board, row, col);
                if (completedSides == 2) return true;
            }
        }
        
        return false;
    }

    (int, int, bool) GetMinimaxMove(GameBoard board, List<(int row, int col, bool isHorizontal)> moves)
    {
        Debug.Log("AI: Using Minimax strategy (simplified)");
        
        int bestScore = int.MinValue;
        (int, int, bool) bestMove = moves[0];

        foreach (var move in moves)
        {
            int score = EvaluateMove(board, move.Item1, move.Item2, move.Item3);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        Debug.Log($"AI Minimax Move: ({bestMove.Item1}, {bestMove.Item2}, {bestMove.Item3}) with score {bestScore}");
        return bestMove;
    }

    int EvaluateMove(GameBoard board, int row, int col, bool isHorizontal)
    {
        int score = 0;
        
        if (WillCompleteBox(board, row, col, isHorizontal))
        {
            score += 100;
        }
        
        if (WillCreateOpportunity(board, row, col, isHorizontal))
        {
            score -= 50;
        }
        
        score += Random.Range(-10, 10);
        
        return score;
    }

    int CountCompletedSides(GameBoard board, int boxRow, int boxCol)
    {
        if (boxRow < 0 || boxRow >= board.gridSize - 1 || boxCol < 0 || boxCol >= board.gridSize - 1)
            return 0;

        int count = 0;
        
        if (board.horizontalLines[boxRow, boxCol].isPlaced) count++; 
        if (board.horizontalLines[boxRow + 1, boxCol].isPlaced) count++; 
        if (board.verticalLines[boxRow, boxCol].isPlaced) count++; 
        if (board.verticalLines[boxRow, boxCol + 1].isPlaced) count++; 
        
        return count;
    }

    void OnDestroy()
    {
        CleanupTempBoards();
    }

    void OnDisable()
    {
        CleanupTempBoards();
    }

    void CleanupTempBoards()
    {
        GameObject[] tempBoards = GameObject.FindGameObjectsWithTag("TempBoard");
        foreach (GameObject tempBoard in tempBoards)
        {
            if (tempBoard != null)
                DestroyImmediate(tempBoard);
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TempBoard"))
            {
                DestroyImmediate(obj);
            }
        }
        
        Debug.Log("AI: Cleaned up temporary board objects");
    }
}