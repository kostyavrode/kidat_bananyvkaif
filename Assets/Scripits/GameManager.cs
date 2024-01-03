using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Action onGameStarted;
    public Transform gameCameraPos;
    
    private bool isGameStarted;
    private float currentTimeScale;
    private int score;
    private int money;
    private int level;

    private void Awake()
    {
        instance = this;
        currentTimeScale = Time.timeScale;
        if (PlayerPrefs.HasKey("Money"))
        {
            money = PlayerPrefs.GetInt("Money");
        }
        if (PlayerPrefs.HasKey("Level"))
        {
            level = PlayerPrefs.GetInt("Level");
        }
        else
        {
            PlayerPrefs.SetInt("Money", 0);
            PlayerPrefs.Save();
        }
        
    }
    private void Start()
    {
        UIManager.instance.ShowMoney(money.ToString());
    }
    private void Update()
    {
        if (isGameStarted)
        {
            //score += 1;
            UIManager.instance.ShowScore(score.ToString());
        }
    }
    public void StartGame()
    {
        UIManager.instance.SetLevelText(level.ToString());
        isGameStarted = true;
        onGameStarted?.Invoke();
        Time.timeScale = 1f;
        Camera.main.transform.DORotateQuaternion(gameCameraPos.rotation,2f);
        Camera.main.transform.DOMove(gameCameraPos.position, 2f);
        Debug.Log("Level" + level);
    }
    public void AddPrize(int prize)
    {
        Debug.Log("AddPrize" + prize);
        score += prize;
        money += prize;
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money")+prize);
        PlayerPrefs.Save();
    }
    public void PauseGame()
    {
        isGameStarted = false;
        Time.timeScale = 0f;
    }
    public void UnPauseGame()
    {
        isGameStarted = true;
        Time.timeScale = currentTimeScale;
    }
    public void EndGame()
    {
        isGameStarted = false;
        CheckBestScore();
        UIManager.instance.ShowLosePanel();
        if (score>15)
        {
            level += 1;
            PlayerPrefs.SetInt("Level", level);
            PlayerPrefs.Save();
        }
    }
    private void CheckBestScore()
    {
        if (PlayerPrefs.HasKey("BestScore"))
        {
            int tempBestScore = PlayerPrefs.GetInt("BestScore");
            if (tempBestScore > score)
            {
                UIManager.instance.ShowBestScore(tempBestScore.ToString());
            }
            else
            {
                UIManager.instance.ShowBestScore(score.ToString());
                PlayerPrefs.SetInt("BestScore", score);
                PlayerPrefs.Save();
            }
        }
        else
        {
            UIManager.instance.ShowBestScore(score.ToString());
            PlayerPrefs.SetInt("BestScore", score);
            PlayerPrefs.Save();
        }
    }
    public bool IsGameStarted()
    {
        return isGameStarted;
    }
}
