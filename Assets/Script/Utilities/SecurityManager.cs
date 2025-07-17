using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Security.Cryptography;

public static class SecurityManager
{
    // 输入验证
    public static bool ValidateMove(int row, int col, bool isHorizontal, GameBoard board)
    {
        // 基本范围检查
        if (board == null) return false;
        
        // 检查坐标是否在有效范围内
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

        // 检查该位置是否已有连线
        if (!board.CanPlaceLine(row, col, isHorizontal))
        {
            LogSecurityEvent($"Attempted to place line on occupied position: ({row}, {col}, {isHorizontal})");
            return false;
        }

        return true;
    }

    // 游戏状态完整性验证
    public static string GenerateGameStateHash(GameBoard board)
    {
        if (board == null) return "";

        StringBuilder stateString = new StringBuilder();
        
        // 添加所有水平线状态
        for (int r = 0; r < board.gridSize; r++)
        {
            for (int c = 0; c < board.gridSize - 1; c++)
            {
                var line = board.horizontalLines[r, c];
                stateString.Append($"H{r},{c},{line.isPlaced},{line.playerId}|");
            }
        }

        // 添加所有垂直线状态
        for (int r = 0; r < board.gridSize - 1; r++)
        {
            for (int c = 0; c < board.gridSize; c++)
            {
                var line = board.verticalLines[r, c];
                stateString.Append($"V{r},{c},{line.isPlaced},{line.playerId}|");
            }
        }

        // 添加所有方框状态
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

    // 防作弊检查
    public static bool ValidateGameState(GameBoard board, int expectedPlayer1Score, int expectedPlayer2Score)
    {
        int actualPlayer1Score = 0;
        int actualPlayer2Score = 0;

        // 计算实际分数
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

    // AI行为验证
    public static bool ValidateAIMove(int row, int col, bool isHorizontal, GameBoard board, AIController.AIStrategy strategy)
    {
        // 验证AI移动是否合理
        if (!ValidateMove(row, col, isHorizontal, board))
            return false;

        // 记录AI移动用于审计
        LogAIMove(row, col, isHorizontal, strategy);

        return true;
    }

    // 输入频率限制
    private static float lastInputTime = 0f;
    private static int inputCount = 0;
    private const float INPUT_WINDOW = 1.0f; // 1秒窗口
    private const int MAX_INPUTS_PER_WINDOW = 10; // 每秒最多10次输入

    public static bool CheckInputRate()
    {
        float currentTime = Time.time;
        
        if (currentTime - lastInputTime > INPUT_WINDOW)
        {
            // 重置计数器
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

    // 辅助方法
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
            return builder.ToString().Substring(0, 16); // 取前16位
        }
    }

    private static void LogSecurityEvent(string message)
    {
        Debug.LogWarning($"[SECURITY] {System.DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        
        // 在实际项目中，这里应该发送到服务器或写入安全日志文件
        PlayerPrefs.SetString($"SecurityLog_{System.DateTime.Now.Ticks}", message);
    }

    private static void LogAIMove(int row, int col, bool isHorizontal, AIController.AIStrategy strategy)
    {
        string moveLog = $"AI Move: ({row},{col},{isHorizontal}) using {strategy} strategy";
        Debug.Log($"[AI_AUDIT] {System.DateTime.Now:yyyy-MM-dd HH:mm:ss} - {moveLog}");
        
        // 记录AI移动历史用于分析
        PlayerPrefs.SetString($"AIMoveLog_{System.DateTime.Now.Ticks}", moveLog);
    }

    // 获取安全统计信息
    public static void GenerateSecurityReport()
    {
        Debug.Log("=== Security Report ===");
        
        // 统计安全事件
        int securityEventCount = 0;
        int aiMoveCount = 0;
        
        // 遍历PlayerPrefs中的日志（简化实现）
        // 在实际项目中应该有专门的日志系统
        
        Debug.Log($"Security Events: {securityEventCount}");
        Debug.Log($"AI Moves Logged: {aiMoveCount}");
        Debug.Log("========================");
    }
}
