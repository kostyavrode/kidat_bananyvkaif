using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TMP_Text scoreBar;
    [SerializeField] private TMP_Text bestScoreBar;
    [SerializeField] private TMP_Text moneyBar;
    [SerializeField] private TMP_Text levelTextBar;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject winPanel;
    public UniWebView uniWebView;
    private void Awake()
    {
        instance = this;
    }
    public void StartGame()
    {
        GameManager.instance.StartGame();
    }
    public void ShowMoney(string money)
    {
        moneyBar.text = money;
    }
    public void PauseGame()
    {
        GameManager.instance.PauseGame();
    }
    public void UnPauseGame()
    {
        GameManager.instance.UnPauseGame();
    }
    public void SetLevelText(string temp)
    {
        levelTextBar.text = "Level "+temp;
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale=1;
    }
    public void ShowScore(string score)
    {
        scoreBar.text = score;
    }
    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        inGamePanel.SetActive(false);
    }
    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        inGamePanel.SetActive(false);
    }
    public void ShowBestScore(string bestScore)
    {
        bestScoreBar.text = bestScore;
    }
    public void ShowPrivacy(string url)
    {
        var webviewObject = new GameObject("UniWebview");
        uniWebView = webviewObject.AddComponent<UniWebView>();
        uniWebView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        uniWebView.SetShowToolbar(true, false, true, true);
        uniWebView.Load(url);
        uniWebView.Show();
    }
}
