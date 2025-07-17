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

    // 公平性确保
    public static bool IsAIDifficultyFair(AIController.AIStrategy strategy, int playerSkillLevel)
    {
        // 根据玩家技能水平调整AI难度
        switch (playerSkillLevel)
        {
            case 1: // 初学者
                return strategy == AIController.AIStrategy.Random || strategy == AIController.AIStrategy.Greedy;
            case 2: // 中级
                return strategy == AIController.AIStrategy.Greedy;
            case 3: // 高级
                return true; // 允许所有策略
            default:
                return strategy == AIController.AIStrategy.Greedy; // 默认中等难度
        }
    }

    // 可访问性支持
    public static Color GetAccessibleColor(Color originalColor, int playerId)
    {
        if (!accessibilitySettings.colorBlindSupport)
            return originalColor;

        // 色盲友好的颜色方案
        switch (playerId)
        {
            case 1:
                return accessibilitySettings.highContrastMode ? Color.black : new Color(0f, 0.4f, 0.8f); // 蓝色
            case 2:
                return accessibilitySettings.highContrastMode ? Color.white : new Color(0.8f, 0.4f, 0f); // 橙色
            default:
                return originalColor;
        }
    }

    // 数据隐私保护
    public static void ClearPersonalData()
    {
        // 清除所有个人游戏数据
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("GameHistory");
        PlayerPrefs.DeleteKey("SkillLevel");
        
        // 保留匿名统计数据
        Debug.Log("[ETHICS] Personal data cleared while preserving anonymous statistics");
    }

    // 游戏时间管理（防止过度游戏）
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
        const float MAX_SESSION_TIME = 3600f; // 1小时

        if (sessionDuration > MAX_SESSION_TIME)
        {
            Debug.Log("[ETHICS] Suggesting break after extended play session");
            return false;
        }

        return true;
    }

    // 公平竞争确保
    public static bool ValidateGameFairness(GameBoard board, Player player1, Player player2)
    {
        // 检查游戏是否公平进行
        int totalMoves = 0;
        int player1Moves = 0;
        int player2Moves = 0;

        // 计算每个玩家的移动次数
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

        // 移动次数不应该相差太大（除非有连续得分）
        int moveDifference = Mathf.Abs(player1Moves - player2Moves);
        bool isFair = moveDifference <= 3; // 允许少量差异

        if (!isFair)
        {
            Debug.LogWarning($"[ETHICS] Potential fairness issue detected. Move difference: {moveDifference}");
        }

        return isFair;
    }

    // 获取和设置可访问性设置
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

    // 生成伦理报告
    public static void GenerateEthicsReport()
    {
        Debug.Log("=== Ethics Report ===");
        Debug.Log($"Color Blind Support: {accessibilitySettings.colorBlindSupport}");
        Debug.Log($"High Contrast Mode: {accessibilitySettings.highContrastMode}");
        Debug.Log($"Session Duration: {(sessionStarted ? Time.time - sessionStartTime : 0)} seconds");
        Debug.Log("====================");
    }
}