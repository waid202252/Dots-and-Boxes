using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Security.Cryptography;

public static class SecurityManager
{
    public static bool ValidateMove(int row, int col, bool isHorizontal, GameBoard board)
    {
        if (board == null) return false;
        
        if (isHorizontal)
        {
            if (row < 0 || row >= board.gridSize || col < 0 || col >= board.gridSize - 1)
            {
                LogSecurityEvent($"Invalid horizontal line coordinates: ({row}, {col})");
                return false;
            }
        }
        else
        {
            if (row < 0 || row >= board.gridSize - 1 || col < 0 || col >= board.gridSize)
            {
                LogSecurityEvent($"Invalid vertical line coordinates: ({row}, {col})");
                return false;
            }
        }

        if (!board.CanPlaceLine(row, col, isHorizontal))
        {
            LogSecurityEvent($"Attempted to place line on occupied position: ({row}, {col}, {isHorizontal})");
            return false;
        }

        return true;
    }

    public static string GenerateGameStateHash(GameBoard board)
    {
        if (board == null) return "";

        StringBuilder stateString = new StringBuilder();
        
        for (int r = 0; r < board.gridSize; r++)
        {
            for (int c = 0; c < board.gridSize - 1; c++)
            {
                var line = board.horizontalLines[r, c];
                stateString.Append($"H{r},{c},{line.isPlaced},{line.playerId}|");
            }
        }

        for (int r = 0; r < board.gridSize - 1; r++)
        {
            for (int c = 0; c < board.gridSize; c++)
            {
                var line = board.verticalLines[r, c];
                stateString.Append($"V{r},{c},{line.isPlaced},{line.playerId}|");
            }
        }

        for (int r = 0; r < board.gridSize - 1; r++)
        {
            for (int c = 0; c < board.gridSize - 1; c++)
            {
                var box = board.boxes[r, c];
                stateString.Append($"B{r},{c},{box.isCompleted},{box.ownerId}|");
            }
        }

        return ComputeHash(stateString.ToString());
    }

    public static bool ValidateGameState(GameBoard board, int expectedPlayer1Score, int expectedPlayer2Score)
    {
        int actualPlayer1Score = 0;
        int actualPlayer2Score = 0;

        for (int r = 0; r < board.gridSize - 1; r++)
        {
            for (int c = 0; c < board.gridSize - 1; c++)
            {
                if (board.boxes[r, c].isCompleted)
                {
                    if (board.boxes[r, c].ownerId == 1)
                        actualPlayer1Score++;
                    else if (board.boxes[r, c].ownerId == 2)
                        actualPlayer2Score++;
                }
            }
        }

        bool isValid = (actualPlayer1Score == expectedPlayer1Score && 
                       actualPlayer2Score == expectedPlayer2Score);

        if (!isValid)
        {
            LogSecurityEvent($"Score mismatch detected! Expected: P1={expectedPlayer1Score}, P2={expectedPlayer2Score}. Actual: P1={actualPlayer1Score}, P2={actualPlayer2Score}");
        }

        return isValid;
    }

    public static bool ValidateAIMove(int row, int col, bool isHorizontal, GameBoard board, AIController.AIStrategy strategy)
    {
        if (!ValidateMove(row, col, isHorizontal, board))
            return false;

        LogAIMove(row, col, isHorizontal, strategy);

        return true;
    }

    private static float lastInputTime = 0f;
    private static int inputCount = 0;
    private const float INPUT_WINDOW = 1.0f; 
    private const int MAX_INPUTS_PER_WINDOW = 10; 

    public static bool CheckInputRate()
    {
        float currentTime = Time.time;
        
        if (currentTime - lastInputTime > INPUT_WINDOW)
        {
            lastInputTime = currentTime;
            inputCount = 1;
            return true;
        }
        
        inputCount++;
        
        if (inputCount > MAX_INPUTS_PER_WINDOW)
        {
            LogSecurityEvent($"Input rate limit exceeded: {inputCount} inputs in {INPUT_WINDOW} seconds");
            return false;
        }
        
        return true;
    }

    private static string ComputeHash(string input)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString().Substring(0, 16); 
        }
    }

    private static void LogSecurityEvent(string message)
    {
        Debug.LogWarning($"[SECURITY] {System.DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        
        PlayerPrefs.SetString($"SecurityLog_{System.DateTime.Now.Ticks}", message);
    }

    private static void LogAIMove(int row, int col, bool isHorizontal, AIController.AIStrategy strategy)
    {
        string moveLog = $"AI Move: ({row},{col},{isHorizontal}) using {strategy} strategy";
        Debug.Log($"[AI_AUDIT] {System.DateTime.Now:yyyy-MM-dd HH:mm:ss} - {moveLog}");
        
        PlayerPrefs.SetString($"AIMoveLog_{System.DateTime.Now.Ticks}", moveLog);
    }

    public static void GenerateSecurityReport()
    {
        Debug.Log("=== Security Report ===");
        
        int securityEventCount = 0;
        int aiMoveCount = 0;
        
        
        Debug.Log($"Security Events: {securityEventCount}");
        Debug.Log($"AI Moves Logged: {aiMoveCount}");
        Debug.Log("========================");
    }
}
