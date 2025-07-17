using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Button playerVsPlayerButton;
    public Button playerVsAIButton; 
    public Button quitButton;

    void Start()
    {
        // 绑定按钮事件
        playerVsPlayerButton.onClick.AddListener(() => StartGame(false));
        playerVsAIButton.onClick.AddListener(() => StartGame(true));
        quitButton.onClick.AddListener(QuitGame);
    }

    void StartGame(bool withAI)
    {
        // 保存游戏模式到PlayerPrefs
        PlayerPrefs.SetInt("PlayWithAI", withAI ? 1 : 0);
        PlayerPrefs.Save();
        
        // 加载游戏场景
        SceneManager.LoadScene("GameScene");
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}