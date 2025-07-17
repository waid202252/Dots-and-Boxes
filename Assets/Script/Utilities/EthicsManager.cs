using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EthicsManager
{
    [System.Serializable]
    public class AccessibilitySettings
    {
        public bool colorBlindSupport = true;
        public bool highContrastMode = false;
        public float textSize = 1.0f;
        public bool soundEnabled = true;
    }

    private static AccessibilitySettings accessibilitySettings = new AccessibilitySettings();

    public static bool IsAIDifficultyFair(AIController.AIStrategy strategy, int playerSkillLevel)
    {
        switch (playerSkillLevel)
        {
            case 1: 
                return strategy == AIController.AIStrategy.Random || strategy == AIController.AIStrategy.Greedy;
            case 2: 
                return strategy == AIController.AIStrategy.Greedy;
            case 3: 
                return true; 
            default:
                return strategy == AIController.AIStrategy.Greedy; 
        }
    }

    public static Color GetAccessibleColor(Color originalColor, int playerId)
    {
        if (!accessibilitySettings.colorBlindSupport)
            return originalColor;

        switch (playerId)
        {
            case 1:
                return accessibilitySettings.highContrastMode ? Color.black : new Color(0f, 0.4f, 0.8f); 
            case 2:
                return accessibilitySettings.highContrastMode ? Color.white : new Color(0.8f, 0.4f, 0f); 
            default:
                return originalColor;
        }
    }

    public static void ClearPersonalData()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("GameHistory");
        PlayerPrefs.DeleteKey("SkillLevel");
        
        Debug.Log("[ETHICS] Personal data cleared while preserving anonymous statistics");
    }

    private static float sessionStartTime;
    private static bool sessionStarted = false;

    public static void StartGameSession()
    {
        sessionStartTime = Time.time;
        sessionStarted = true;
    }

    public static bool CheckGameTimeLimit()
    {
        if (!sessionStarted) return true;

        float sessionDuration = Time.time - sessionStartTime;
        const float MAX_SESSION_TIME = 3600f; 

        if (sessionDuration > MAX_SESSION_TIME)
        {
            Debug.Log("[ETHICS] Suggesting break after extended play session");
            return false;
        }

        return true;
    }

    public static bool ValidateGameFairness(GameBoard board, Player player1, Player player2)
    {
        int totalMoves = 0;
        int player1Moves = 0;
        int player2Moves = 0;

        for (int r = 0; r < board.gridSize; r++)
        {
            for (int c = 0; c < board.gridSize - 1; c++)
            {
                if (board.horizontalLines[r, c].isPlaced)
                {
                    totalMoves++;
                    if (board.horizontalLines[r, c].playerId == 1)
                        player1Moves++;
                    else
                        player2Moves++;
                }
            }
        }

        for (int r = 0; r < board.gridSize - 1; r++)
        {
            for (int c = 0; c < board.gridSize; c++)
            {
                if (board.verticalLines[r, c].isPlaced)
                {
                    totalMoves++;
                    if (board.verticalLines[r, c].playerId == 1)
                        player1Moves++;
                    else
                        player2Moves++;
                }
            }
        }

        int moveDifference = Mathf.Abs(player1Moves - player2Moves);
        bool isFair = moveDifference <= 3; 

        if (!isFair)
        {
            Debug.LogWarning($"[ETHICS] Potential fairness issue detected. Move difference: {moveDifference}");
        }

        return isFair;
    }

    public static AccessibilitySettings GetAccessibilitySettings()
    {
        return accessibilitySettings;
    }

    public static void SetAccessibilitySettings(AccessibilitySettings settings)
    {
        accessibilitySettings = settings;
        PlayerPrefs.SetInt("ColorBlindSupport", settings.colorBlindSupport ? 1 : 0);
        PlayerPrefs.SetInt("HighContrastMode", settings.highContrastMode ? 1 : 0);
        PlayerPrefs.SetFloat("TextSize", settings.textSize);
        PlayerPrefs.SetInt("SoundEnabled", settings.soundEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void LoadAccessibilitySettings()
    {
        accessibilitySettings.colorBlindSupport = PlayerPrefs.GetInt("ColorBlindSupport", 1) == 1;
        accessibilitySettings.highContrastMode = PlayerPrefs.GetInt("HighContrastMode", 0) == 1;
        accessibilitySettings.textSize = PlayerPrefs.GetFloat("TextSize", 1.0f);
        accessibilitySettings.soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
    }

    public static void GenerateEthicsReport()
    {
        Debug.Log("=== Ethics Report ===");
        Debug.Log($"Color Blind Support: {accessibilitySettings.colorBlindSupport}");
        Debug.Log($"High Contrast Mode: {accessibilitySettings.highContrastMode}");
        Debug.Log($"Session Duration: {(sessionStarted ? Time.time - sessionStartTime : 0)} seconds");
        Debug.Log("====================");
    }
}