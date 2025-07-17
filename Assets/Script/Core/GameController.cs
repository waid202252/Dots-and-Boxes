using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Game Components")]
    public GameBoard gameBoard;
    public GameBoardView gameBoardView;
    public UIManager uiManager;
    public InputController inputController;
    public AIController aiController;

    [Header("Game Data")]
    public GameState gameState;
    public Player player1;
    public Player player2;
    public bool playWithAI;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        playWithAI = PlayerPrefs.GetInt("PlayWithAI", 0) == 1;

        player1 = new Player(1, "Player 1", false);
        player2 = new Player(2, playWithAI ? "AI" : "Player 2", playWithAI);

        gameState = new GameState();

        if (gameBoard == null)
            gameBoard = FindObjectOfType<GameBoard>();
        
        gameBoard.InitializeBoard();

        if (gameBoardView != null)
            gameBoardView.InitializeBoardView(gameBoard);

        if (uiManager != null)
            uiManager.InitializeUI(this);

        StartPlayerTurn();
    }

    public void ProcessMove(int row, int col, bool isHorizontal)
{
    Debug.Log($"ProcessMove called: ({row}, {col}, {isHorizontal}) by Player {gameState.currentPlayerId}");
    Debug.Log($"Current game state: {gameState.currentPhase}, isGameOver: {gameState.isGameOver}");

    if (gameState.isGameOver)
    {
        Debug.LogWarning("Game is already over, ignoring move");
        return;
    }

    if (gameState.currentPhase != GamePhase.WaitingForInput && gameState.currentPhase != GamePhase.AITurn)
    {
        Debug.LogWarning($"Invalid game phase for move: {gameState.currentPhase}");
        return;
    }

    if (!SecurityManager.ValidateMove(row, col, isHorizontal, gameBoard))
    {
        Debug.LogError("Invalid move detected!");
        if (GetCurrentPlayer().isAI)
        {
            gameState.SwitchPlayer();
            StartPlayerTurn();
        }
        return;
    }

    List<Box> completedBoxes = gameBoard.PlaceLine(row, col, isHorizontal, gameState.currentPlayerId);
    Debug.Log($"Move processed, completed {completedBoxes.Count} boxes");

    if (completedBoxes.Count > 0)
    {
        Player currentPlayer = GetCurrentPlayer();
        currentPlayer.AddScore(completedBoxes.Count);
        Debug.Log($"Player {currentPlayer.playerId} scored {completedBoxes.Count} points, total: {currentPlayer.score}");

        if (gameBoardView != null)
        {
            gameBoardView.UpdateLineView(row, col, isHorizontal, currentPlayer);
            foreach (Box box in completedBoxes)
            {
                gameBoardView.UpdateBoxView(box.row, box.col, currentPlayer);
            }
        }

        if (uiManager != null)
            uiManager.UpdateGameUI();

        if (!gameBoard.IsGameOver())
        {
            Debug.Log("Player continues due to completed boxes");
            StartPlayerTurn();
        }
    }
    else
    {
        Debug.Log("No boxes completed, switching players");
        
        if (gameBoardView != null)
            gameBoardView.UpdateLineView(row, col, isHorizontal, GetCurrentPlayer());

        gameState.SwitchPlayer();
        Debug.Log($"Switched to Player {gameState.currentPlayerId}");
        
        if (!gameBoard.IsGameOver())
        {
            StartPlayerTurn();
        }
    }

    if (gameBoard.IsGameOver())
    {
        Debug.Log("Game Over detected");
        EndGame();
    }
}

    void StartPlayerTurn()
{
    Debug.Log($"StartPlayerTurn called for Player {gameState.currentPlayerId}");
    
    gameState.currentPhase = GamePhase.WaitingForInput;
    
    if (uiManager != null)
        uiManager.UpdateCurrentPlayerDisplay();

    Player currentPlayer = GetCurrentPlayer();
    Debug.Log($"Current player: {currentPlayer.playerName}, isAI: {currentPlayer.isAI}");

    if (currentPlayer.isAI)
    {
        Debug.Log("Starting AI turn");
        gameState.currentPhase = GamePhase.AITurn;
        StartCoroutine(ProcessAITurn());
    }
    else
    {
        Debug.Log("Waiting for human player input");
    }

    if (uiManager != null)
    {
        uiManager.ForceUIRefresh();
    }
}
    IEnumerator ProcessAITurn()
{
    Debug.Log("AI Turn started - Coroutine");
    
    yield return new WaitForSeconds(aiController != null ? aiController.thinkingTime : 1.0f);

    if (aiController != null && gameBoard != null)
    {
        try
        {
            var move = aiController.GetBestMove(gameBoard);
            
            if (move.row >= 0 && move.col >= 0) 
            {
                Debug.Log($"AI making move: ({move.row}, {move.col}, {move.isHorizontal})");
                
                ProcessMove(move.row, move.col, move.isHorizontal);
                
                Debug.Log("AI Turn completed - Coroutine ending");
            }
            else
            {
                Debug.LogError("AI returned invalid move!");
                gameState.SwitchPlayer();
                StartPlayerTurn();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AI Turn Error: {e.Message}\n{e.StackTrace}");
            gameState.SwitchPlayer();
            StartPlayerTurn();
        }
    }
    else
    {
        Debug.LogError("AI Controller or GameBoard is null!");
        gameState.SwitchPlayer();
        StartPlayerTurn();
    }
}

    void EndGame()
    {
        gameState.isGameOver = true;
        gameState.currentPhase = GamePhase.GameOver;

        if (player1.score > player2.score)
            gameState.winner = 1;
        else if (player2.score > player1.score)
            gameState.winner = 2;
        else
            gameState.winner = 0; 

        if (uiManager != null)
            uiManager.ShowGameOver();
    }

    public Player GetCurrentPlayer()
    {
        return gameState.currentPlayerId == 1 ? player1 : player2;
    }

    public Player GetPlayer(int playerId)
    {
        return playerId == 1 ? player1 : player2;
    }

    public void RestartGame()
    {
        InitializeGame();
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}