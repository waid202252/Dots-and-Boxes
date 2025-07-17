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
    public int minimaxDepth = 2; // 降低深度避免卡顿
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

        // 1. 优先选择能完成方框的移动
        foreach (var move in moves)
        {
            if (WillCompleteBox(board, move.Item1, move.Item2, move.Item3))
            {
                Debug.Log($"AI Greedy Move (Complete Box): ({move.Item1}, {move.Item2}, {move.Item3})");
                return move;
            }
        }

        // 2. 选择不会给对手创造机会的移动
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

        // 3. 随机选择
        var randomMove = moves[Random.Range(0, moves.Count)];
        Debug.Log($"AI Greedy Move (Random fallback): ({randomMove.Item1}, {randomMove.Item2}, {randomMove.Item3})");
        return randomMove;
    }

    bool WillCompleteBox(GameBoard board, int row, int col, bool isHorizontal)
    {
        // 简化实现，避免创建临时对象
        if (isHorizontal)
        {
            // 检查上方和下方的方框
            int boxesCompleted = 0;
            
            // 上方方框
            if (row > 0)
            {
                int completedSides = CountCompletedSides(board, row - 1, col);
                if (completedSides == 3) boxesCompleted++;
            }
            
            // 下方方框
            if (row < board.gridSize - 1)
            {
                int completedSides = CountCompletedSides(board, row, col);
                if (completedSides == 3) boxesCompleted++;
            }
            
            return boxesCompleted > 0;
        }
        else
        {
            // 检查左方和右方的方框
            int boxesCompleted = 0;
            
            // 左方方框
            if (col > 0)
            {
                int completedSides = CountCompletedSides(board, row, col - 1);
                if (completedSides == 3) boxesCompleted++;
            }
            
            // 右方方框
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
        // 简化检查，避免创建临时GameBoard
        if (isHorizontal)
        {
            // 检查是否会创造3边方框
            // 上方方框
            if (row > 0)
            {
                int completedSides = CountCompletedSides(board, row - 1, col);
                if (completedSides == 2) return true;
            }
            
            // 下方方框
            if (row < board.gridSize - 1)
            {
                int completedSides = CountCompletedSides(board, row, col);
                if (completedSides == 2) return true;
            }
        }
        else
        {
            // 左方方框
            if (col > 0)
            {
                int completedSides = CountCompletedSides(board, row, col - 1);
                if (completedSides == 2) return true;
            }
            
            // 右方方框
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
        
        // 简化Minimax，避免创建大量临时对象
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
        
        // 能完成方框的移动得高分
        if (WillCompleteBox(board, row, col, isHorizontal))
        {
            score += 100;
        }
        
        // 会给对手创造机会的移动扣分
        if (WillCreateOpportunity(board, row, col, isHorizontal))
        {
            score -= 50;
        }
        
        // 添加一些随机性
        score += Random.Range(-10, 10);
        
        return score;
    }

    int CountCompletedSides(GameBoard board, int boxRow, int boxCol)
    {
        if (boxRow < 0 || boxRow >= board.gridSize - 1 || boxCol < 0 || boxCol >= board.gridSize - 1)
            return 0;

        int count = 0;
        
        // 检查四条边
        if (board.horizontalLines[boxRow, boxCol].isPlaced) count++; // 上边
        if (board.horizontalLines[boxRow + 1, boxCol].isPlaced) count++; // 下边
        if (board.verticalLines[boxRow, boxCol].isPlaced) count++; // 左边
        if (board.verticalLines[boxRow, boxCol + 1].isPlaced) count++; // 右边
        
        return count;
    }

    // 清理方法 - 移除所有TempBoard相关代码
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
        // 强制清理所有TempBoard对象
        GameObject[] tempBoards = GameObject.FindGameObjectsWithTag("TempBoard");
        foreach (GameObject tempBoard in tempBoards)
        {
            if (tempBoard != null)
                DestroyImmediate(tempBoard);
        }

        // 也清理所有名字包含"TempBoard"的对象
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