using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{

    public static LevelController Current;
    public bool isGameActive;
    int currentLevel;

    public GameObject startMenu, gameUI, gameOverMenu, finishMenu;
    public GameObject howToPlayMenu;

    public Text scoreText, finishScoreText, currentLevelText, nextLevelText;
    public Text startGameMunuMoneyText, finishGameMenuMoneyText, gameOverMenuMoneyText;

    public GameObject gameLine;
    float maxDistance;
    public Slider levelProgressBar;
    public int score;

    public AudioSource gameMusicAudioSurce;
    public AudioClip victoryClip, gameOverClip;

    public DailyReward dailyReward;

    void Start()
    {
        Current = this;

        UpdateMoneyTexts();
        gameMusicAudioSurce = Camera.main.GetComponent<AudioSource>();
        currentLevel = PlayerPrefs.GetInt("currentLevel");


        if (SceneManager.GetActiveScene().buildIndex != currentLevel)
        {
            SceneManager.LoadScene(currentLevel);
        }
        else
        {
            CharacterControllerScript.Current = GameObject.FindObjectOfType<CharacterControllerScript>();
            GameObject.FindObjectOfType<MarketController>().InitializeMarketController();
            dailyReward.InitializeDailyReward();
            currentLevelText.text = (currentLevel + 1).ToString();
            nextLevelText.text = (currentLevel + 2).ToString();
            UpdateMoneyTexts();
        }
    }

    private void Update()
    {
        if(isGameActive == true)
        {
            float distance = gameLine.transform.position.z - CharacterControllerScript.Current.transform.position.z;
            levelProgressBar.value = 1 - (distance / maxDistance);
        }
    }

    public void StartLevel()
    {
        if(currentLevel == 0)
        {
            howToPlayMenu.SetActive(true);
            startMenu.SetActive(false);
       //     Time.timeScale = 0;
        }
        else
        {
        maxDistance = gameLine.transform.position.z - CharacterControllerScript.Current.transform.position.z;

        CharacterControllerScript.Current.ChangeSpeed(CharacterControllerScript.Current.runningSpeed);

        startMenu.SetActive(false);
        gameUI.SetActive(true);

        CharacterControllerScript.Current.animator.SetBool("running", true);

        isGameActive = true;
        }
    }

    public void CloseTheHTPMenu()
    {
        howToPlayMenu.SetActive(false);
       // Time.timeScale = 1;
        maxDistance = gameLine.transform.position.z - CharacterControllerScript.Current.transform.position.z;

        CharacterControllerScript.Current.ChangeSpeed(CharacterControllerScript.Current.runningSpeed);

        startMenu.SetActive(false);
        gameUI.SetActive(true);

        CharacterControllerScript.Current.animator.SetBool("running", true);

        isGameActive = true;
    }


    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartNextLevel()
    {
        SceneManager.LoadScene(currentLevel + 1);
    }

    public void Gameover()
    {
        UpdateMoneyTexts();
        gameMusicAudioSurce.Stop();
        gameMusicAudioSurce.PlayOneShot(gameOverClip);
        gameUI.SetActive(false);
        gameOverMenu.SetActive(true);
        isGameActive = false;
    }

    public void FinishLevel()
    {
        GiveMoneyToPlayer(score);
        gameMusicAudioSurce.Stop();
        gameMusicAudioSurce.PlayOneShot(victoryClip);
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);
        gameUI.SetActive(false);
        finishMenu.SetActive(true);
        finishScoreText.text = score.ToString();
        isGameActive = false;
    }

    public void changeScore(int addScore)
    {
        score += addScore;
        scoreText.text = score.ToString();
    }

    public void UpdateMoneyTexts()
    {
        int money = PlayerPrefs.GetInt("money");
        startGameMunuMoneyText.text = money.ToString();
        gameOverMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();
    }

    public void GiveMoneyToPlayer(int newMoney)
    {
        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money + newMoney);
        PlayerPrefs.SetInt("money", money);
        UpdateMoneyTexts();
    }
}
