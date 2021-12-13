using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI[] moneyText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI newHighScoreText;
    public TextMeshProUGUI sockText;
    public Image shop;
    public GameObject titleScreen;
    public GameObject endScreen;
    public GameObject socks;
    public PlayerController playerController;
    public SpawnManager spawnManager;
    private int score;
    private int coins;
    private float seconds;
    public static bool isRunning = false;

    void Start()
    {
        moneyText[0].text = "$: " + Getint("WreckingCoins");
        highScoreText.text = "HighScore: " + Getint("HighScore");
        newHighScoreText.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning && !playerController.onCheckpoint)
        {
            seconds += Time.deltaTime;
            if (seconds > 0.5f)
            {
                UpdateScore(1);
                seconds = 0;
            }
        }
        
    }

    public void StartGame()
    {
        scoreText.gameObject.SetActive(true);
        socks.gameObject.SetActive(true);
        sockText.text = "x " + Getint("SockCount");
        score = 0;
        scoreText.text = "Score: " + score;
        isRunning = true;
        titleScreen.SetActive(false);
        spawnManager.SpawnBoostingPadsAndRandomCrates();
        spawnManager.SpawnCrateLine();
        spawnManager.SpawnCheckpoint();
        spawnManager.SpawnEnvironment();
    }
    public void EndGame()
    {
        isRunning = false;
        socks.gameObject.SetActive(false);
        coins = Getint("WreckingCoins") + score / 100;
        moneyText[1].text = "$: " + coins;
        SetInt("WreckingCoins", coins);
        endScreen.SetActive(true);
        if(Getint("HighScore") < score)
        {
            newHighScoreText.text += score;
            newHighScoreText.gameObject.SetActive(true);
            SetInt("HighScore", score);
        }

    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateScore(int addToScore)
    {
        score += addToScore;
        scoreText.text = "Score: " + score;
    }

    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }

    public int Getint(string KeyName)
    {
        return PlayerPrefs.GetInt(KeyName);
    }

    public void OpenShop()
    {
        moneyText[2].text = "Money: $" + Getint("WreckingCoins");
        shop.gameObject.SetActive(true);
    }
    public void BuySock()
    {
        int curr = Getint("WreckingCoins");
        if(curr >= 25)
        {
            curr -= 25;
            SetInt("WreckingCoins", curr);
            moneyText[2].text = "Money: $" + Getint("WreckingCoins");
            moneyText[1].text = "$: " + Getint("WreckingCoins");
            moneyText[0].text = "$: " + Getint("WreckingCoins");
            int sockCount = Getint("SockCount");
            sockCount++;
            SetInt("SockCount", sockCount);
        }
       
    }

    public void ReduceSockCount()
    {
        int sockCount = Getint("SockCount");
        sockCount--;
        SetInt("SockCount", sockCount);
        sockText.text = "x " + sockCount;

    }

    public void BackToMain()
    {
        shop.gameObject.SetActive(false);
    }
}
