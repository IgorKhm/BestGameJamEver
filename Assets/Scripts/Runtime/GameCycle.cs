using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCycle : MonoBehaviour
{
    public static GameCycle Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Optional UI Text")]
    [SerializeField] private TextMeshProUGUI winTextUI;
    [SerializeField] private TextMeshProUGUI loseTextUI;

    [SerializeField] private string winText = "You entered the party!";
    [SerializeField] private string loseText = "Rejected!";

    [Header("Time")]
    [Tooltip("Normal speed is 1")]
    [SerializeField] private float gameplayTimeScale = 1f;

    public Action OnRoundWon;
    public Action OnRoundLost;

    private void Awake()
    {
        // Singleton guard (important because you reload the scene)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += (_, __) => ShowStart(); // after restart

        ShowStart();

        InputSystem.Instance.OnRestartPressed += () => startPanel.SetActive(true);
    }

    private void ShowOnly(GameObject panel)
    {
        if (startPanel != null) startPanel.SetActive(panel == startPanel);
        if (winPanel != null) winPanel.SetActive(panel == winPanel);
        if (losePanel != null) losePanel.SetActive(panel == losePanel);
    }

    public void ShowStart()
    {
        Time.timeScale = 0f;          // pause until Play
        ShowOnly(startPanel);
    }

    public void StartPressed()
    {
        // Hide all menu panels and run the game
        Time.timeScale = gameplayTimeScale;
        if (startPanel != null) startPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        FindObjectOfType<GameManager>()?.StartGame();
    }

    public void WinGame()
    {
        Time.timeScale = 0f;
        if (winTextUI != null) winTextUI.text = winText;
        ShowOnly(winPanel);
        OnRoundWon?.Invoke();
    }

    public void LoseGame()
    {
        Time.timeScale = 0f;
        if (loseTextUI != null) loseTextUI.text = loseText;
        ShowOnly(losePanel);
        OnRoundLost?.Invoke();
    }

    public void RestartGame()
    {
        Time.timeScale = gameplayTimeScale;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // ShowStart() will be called by sceneLoaded handler
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
