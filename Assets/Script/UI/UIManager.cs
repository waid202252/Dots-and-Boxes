using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    
    [Header("Bottom Panel Buttons")]
    public Button bottomRestartButton;
    public Button bottomBackToMenuButton;
    
    [Header("Game Over Panel Buttons")]
    public Button gameOverRestartButton;
    public Button gameOverBackToMenuButton;

    private GameController gameController;

    public void InitializeUI(GameController controller)
    {
        gameController = controller;
        
        // 绑定底部面板按钮事件
        if (bottomRestartButton != null)
            bottomRestartButton.onClick.AddListener(() => gameController.RestartGame());
        
        if (bottomBackToMenuButton != null)
            bottomBackToMenuButton.onClick.AddListener(() => gameController.BackToMenu());

        // 绑定游戏结束面板按钮事件
        if (gameOverRestartButton != null)
            gameOverRestartButton.onClick.AddListener(() => gameController.RestartGame());
        
        if (gameOverBackToMenuButton != null)
            gameOverBackToMenuButton.onClick.AddListener(() => gameController.BackToMenu());

        // 隐藏游戏结束面板
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 初始化UI显示
        UpdateGameUI();
    }

    public void UpdateCurrentPlayerDisplay()
{
    if (currentPlayerText != null && gameController != null)
    {
        Player currentPlayer = gameController.GetCurrentPlayer();
        GamePhase currentPhase = gameController.gameState.currentPhase;
        
        Debug.Log($"UpdateCurrentPlayerDisplay: Player {currentPlayer.playerId}, Phase: {currentPhase}, isAI: {currentPlayer.isAI}");
        
        if (currentPlayer.isAI && currentPhase == GamePhase.AITurn)
        {
            currentPlayerText.text = "AI is thinking...";
            currentPlayerText.color = Color.yellow;
        }
        else if (currentPlayer.isAI)
        {
            currentPlayerText.text = $"AI's Turn";
            currentPlayerText.color = currentPlayer.playerColor;
        }
        else
        {
            currentPlayerText.text = $"{currentPlayer.playerName}'s Turn";
            currentPlayerText.color = currentPlayer.playerColor;
        }
        
        Debug.Log($"UI updated to: {currentPlayerText.text}");
    }
    else
    {
        Debug.LogError("UpdateCurrentPlayerDisplay: Missing references!");
    }
}

    public void UpdateGameUI()
    {
        if (gameController == null) return;

        // 更新分数显示
        if (player1ScoreText != null)
            player1ScoreText.text = $"Player 1: {gameController.player1.score}";
        
        if (player2ScoreText != null)
        {
            string player2Name = gameController.player2.isAI ? "AI" : "Player 2";
            player2ScoreText.text = $"{player2Name}: {gameController.player2.score}";
        }

        // 更新当前玩家显示
        UpdateCurrentPlayerDisplay();
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverText != null && gameController != null)
        {
            string message = "";
            switch (gameController.gameState.winner)
            {
                case 0:
                    message = "It's a Draw!";
                    break;
                case 1:
                    message = "Player 1 Wins!";
                    break;
                case 2:
                    string winner = gameController.player2.isAI ? "AI" : "Player 2";
                    message = $"{winner} Wins!";
                    break;
            }
            
            message += $"\n\nFinal Scores:\n";
            message += $"Player 1: {gameController.player1.score}\n";
            string player2Name = gameController.player2.isAI ? "AI" : "Player 2";
            message += $"{player2Name}: {gameController.player2.score}";
            
            gameOverText.text = message;
        }
    }

    // 公共方法供按钮直接调用（备用方案）
    public void OnRestartButtonClicked()
    {
        if (gameController != null)
        {
            Debug.Log("Restart button clicked"); // 调试信息
            gameController.RestartGame();
        }
        else
        {
            Debug.LogError("GameController is null!");
        }
    }

    public void OnBackToMenuButtonClicked()
    {
        if (gameController != null)
        {
            Debug.Log("Back to menu button clicked"); // 调试信息
            gameController.BackToMenu();
        }
        else
        {
            Debug.LogError("GameController is null!");
        }
    }

    public void ForceUIRefresh()
    {
        Debug.Log("Force UI Refresh called");
        
        if (gameController != null)
        {
            UpdateGameUI();
            UpdateCurrentPlayerDisplay();
            
            // 强制重绘UI
            if (currentPlayerText != null)
            {
                currentPlayerText.SetText(currentPlayerText.text);
            }
        }
    }
}   