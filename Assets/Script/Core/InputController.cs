using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("Components")]
    public Camera gameCamera;
    public GameController gameController;
    public GameBoardView gameBoardView;

    [Header("Input Settings")]
    public LayerMask clickableLayer = -1;

    void Start()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // 处理鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        // 处理键盘输入
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameController?.RestartGame();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameController?.BackToMenu();
        }
    }

    void HandleMouseClick()
    {
        if (gameController == null || gameBoardView == null) return;

        // 检查游戏状态
        if (gameController.gameState.isGameOver || 
            gameController.gameState.currentPhase != GamePhase.WaitingForInput)
            return;

        // 如果当前是AI回合，忽略输入
        if (gameController.GetCurrentPlayer().isAI)
            return;

        // 获取鼠标世界坐标
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        
        // 检查是否点击了有效的线条位置
        if (gameBoardView.GetLineFromWorldPosition(mouseWorldPos, out int row, out int col, out bool isHorizontal))
        {
            // 验证移动是否合法
            if (SecurityManager.ValidateMove(row, col, isHorizontal, gameController.gameBoard))
            {
                // 处理移动
                gameController.ProcessMove(row, col, isHorizontal);
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = gameCamera.transform.position.z * -1; // 相机到平面的距离
        return gameCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    // 公共方法供AI或其他系统调用
    public void ProcessExternalMove(int row, int col, bool isHorizontal)
    {
        if (SecurityManager.ValidateMove(row, col, isHorizontal, gameController.gameBoard))
        {
            gameController.ProcessMove(row, col, isHorizontal);
        }
    }
}