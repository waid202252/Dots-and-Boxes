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

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }


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


        if (gameController.gameState.isGameOver || 
            gameController.gameState.currentPhase != GamePhase.WaitingForInput)
            return;

        if (gameController.GetCurrentPlayer().isAI)
            return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        
        if (gameBoardView.GetLineFromWorldPosition(mouseWorldPos, out int row, out int col, out bool isHorizontal))
        {
            if (SecurityManager.ValidateMove(row, col, isHorizontal, gameController.gameBoard))
            {
                gameController.ProcessMove(row, col, isHorizontal);
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = gameCamera.transform.position.z * -1; 
        return gameCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    public void ProcessExternalMove(int row, int col, bool isHorizontal)
    {
        if (SecurityManager.ValidateMove(row, col, isHorizontal, gameController.gameBoard))
        {
            gameController.ProcessMove(row, col, isHorizontal);
        }
    }
}