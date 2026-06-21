using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;  
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameManager gameManager;

    private float heightOffset = 667f;


    private void OnEnable()
    {
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(OnRestartClicked);
        gameManager.OnGameOver += ShowGameOver;
        gameManager.OnRestart += HideGameOver;
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(OnRestartClicked);
        gameManager.OnGameOver -= ShowGameOver;
        gameManager.OnRestart -= HideGameOver;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        scoreText.text = $"Your Score: {gameManager.score}";
        highScoreText.text = $"High Score: {gameManager.highScore}";

        Sequence sequence = DOTween.Sequence();
        RectTransform gameOverRectTransform = gameOverPanel.GetComponent<RectTransform>();
        sequence.Append(gameOverRectTransform.DOAnchorPosY(heightOffset, .5f).From());
        sequence.Append(gameOverRectTransform.DOShakeAnchorPos(1f));
    }

    public void HideGameOver()
    {
        gameOverPanel.SetActive(false);
    }

    public void OnRestartClicked()
    {
        gameManager.Restart();
    }
}


