using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCycle : MonoBehaviour
{
    [Tooltip("Normal speed is 1")]
    [SerializeField] private float GameSpeed = 1;
    [SerializeField] private float menuDurationBeforeRestartInSec = 10;
    private enum GameStatus { win, lose }
   
    [SerializeField] private TextMeshProUGUI conclusionText;
    [SerializeField] private string winText = "You entered the cult";
    [SerializeField] private string loseText = "You got rejected, you suck!";

    [SerializeField] private Image conclusionImage;
    private Sprite winSprite, loseSprite;
    
    public Action OnRoundWon;
    public Action OnRoundLost;
    
    private void Awake()
    {
        Time.timeScale = GameSpeed;
    }

    public void WinGame()
    {
        StartCoroutine(ConcludeGame(GameStatus.win));
    }

    public void LoseGame()
    {
        StartCoroutine(ConcludeGame(GameStatus.lose));
    }

    private IEnumerator ConcludeGame(GameStatus gameStatus)
    {
        switch (gameStatus)
        {
            case GameStatus.win:
                conclusionText.text = winText;
                conclusionImage.sprite = winSprite;
                OnRoundWon.Invoke();
                break;
            case GameStatus.lose:
                conclusionText.text = loseText;
                conclusionImage.sprite = loseSprite;
                OnRoundLost.Invoke();
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(menuDurationBeforeRestartInSec);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
