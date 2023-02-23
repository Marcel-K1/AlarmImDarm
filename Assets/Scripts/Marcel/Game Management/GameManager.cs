/*********************************************************************************************
* Project: Alarm Im Darm
* File   : GameManager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Game Manager as Singleton to communicate between scenes
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singelton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    #endregion

    #region Variables

    //Player Data
    [SerializeField]
    ScriptableInt playerHealth;
    [SerializeField]
    ScriptableFloat playerSpeed;
    [SerializeField]
    ScriptableInt playerStrength;
    [SerializeField]
    ScriptableString playerName;
    [SerializeField]
    ScriptableInt playerPoints;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private PlayerSpriteScriptableObject playerDefaultSprite;

    //Map Data
    [SerializeField]
    public EnemyListScriptableObject enemyList;
    [SerializeField]
    private MapManagerScriptableObject MapData;
    [SerializeField]
    public CollectedPointsOfInterestScriptableObject POIData;
    [SerializeField]
    private GameDifficultyScriptableObject gameDifficulty;
    [SerializeField]
    public SaveDataScriptableObject saveData;

    private bool failSafeVar = true;
    private bool firstMessage = true;
    private bool secondMessage = true;
    private bool thirdMessage = true;

    #endregion

    #region Properties

    [SerializeField]
    private bool paused = false;
    public bool Paused { get => paused; set => paused = value; }

    [SerializeField]
    private bool firstBoot = true;
    public bool FirstBoot { get => firstBoot; set => firstBoot = value; }

    [SerializeField]
    private bool won = false;
    public bool Won { get => won; set => won = value; }

    [SerializeField]
    private bool lost = false;
    public bool Lost { get => lost; set => lost = value; }

    [SerializeField]
    private bool gameOver = false;
    public bool GameOver { get => gameOver; set => gameOver = value; }

    [SerializeField]
    private bool reload = false;
    public bool Reload { get => reload; set => reload = value; }

    [SerializeField]
    private bool loadingPlayerData = false;
    public bool LoadingPlayerData { get => loadingPlayerData; set => loadingPlayerData = value; }
    
    [SerializeField]
    private bool currentPOICollected = false;
    public bool CurrentPOICollected { get => currentPOICollected; set => currentPOICollected = value; }
    
    [SerializeField]
    private float currentEnemiesLeftPercentage = 0;
    public float CurrentEnemiesLeftPercentage { get => currentEnemiesLeftPercentage; set => currentEnemiesLeftPercentage = value; }

    [SerializeField]
    private bool sceneChangeToFinalScene = false;
    public bool SceneChangeToFinalScene { get => sceneChangeToFinalScene; set => sceneChangeToFinalScene = value; }
    #endregion

    #region Events

    private static event System.Action reloadEvent = () => { };

    public event System.Action ReloadEvent { add { reloadEvent += value; } remove { reloadEvent -= value; } }

    private static event System.Action gameOverEvent = () => { };

    public event System.Action GameOverEvent { add { gameOverEvent += value; } remove { gameOverEvent -= value; } }

    private static event System.Action pauseEvent = () => { };

    public event System.Action PauseEvent { add { pauseEvent += value; } remove { pauseEvent -= value; } }

    private static event System.Action enemyCountTextEvent = () => { };

    public event System.Action EnemyCountTextEvent { add { enemyCountTextEvent += value; } remove { enemyCountTextEvent -= value; } }

    private static event System.Action findPOITextEvent = () => { };

    public event System.Action FindPOITextEvent { add { findPOITextEvent += value; } remove { findPOITextEvent -= value; } }

    #endregion

    #region Methods

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    private void Update()
    {
        //Quit Conditions
        if (Input.GetKeyDown(KeyCode.Escape) && (SceneManager.GetActiveScene().name == "TestLevel" || SceneManager.GetActiveScene().name == "TestWin"))
        {
            ReloadAndResetToMainMenu();
        }

        //Pause Conditions
        if (Input.GetKeyDown(KeyCode.P) && SceneManager.GetActiveScene().name == "TestLevel")
        {
            pauseEvent.Invoke();
            paused = true;
            Pause(paused);
        }

        //Fartalot Event Conditions
        if (SceneManager.GetActiveScene().name == "TestLevel")
        {
            enemyList.currentEnemiesLeft = enemyList.Minion.Count + enemyList.Boss.Count;
            CurrentEnemiesLeftPercentage = (((float)enemyList.currentEnemiesLeft / (float)enemyList.currentEnemiesCapacity) * 100);

            if (CurrentEnemiesLeftPercentage < 80 && CurrentEnemiesLeftPercentage > 60)
            {
                if (failSafeVar && firstMessage)
                {
                    enemyCountTextEvent.Invoke();
                    failSafeVar = false;
                    firstMessage = false;
                }
                else
                {
                    failSafeVar = true;

                }
            }
            else if (CurrentEnemiesLeftPercentage < 60 && CurrentEnemiesLeftPercentage > 30)
            {
                if (failSafeVar && secondMessage)
                {
                    enemyCountTextEvent.Invoke();
                    failSafeVar = false;
                    secondMessage = false;
                }
                else
                {
                    failSafeVar = true;

                }
            }
            else if (CurrentEnemiesLeftPercentage < 30 && CurrentEnemiesLeftPercentage > 0)
            {
                if (failSafeVar && thirdMessage)
                {
                    enemyCountTextEvent.Invoke();
                    failSafeVar = false;
                    thirdMessage = false;
                }
                else
                {
                    failSafeVar = true;
                }
            }

        }

        //Win- or Loose Conditions
        if (SceneManager.GetActiveScene().name == "TestLevel" && !SceneChangeToFinalScene)
        {

            if (CurrentEnemiesLeftPercentage == 0 && CurrentPOICollected)
            {
                won = true;
                StartCoroutine(LoadWinLooseScene(3));
                SceneChangeToFinalScene = true;
            }
            else if (gameOver)
            {
                lost = true;
                StartCoroutine(LoadWinLooseScene(3));
                SceneChangeToFinalScene = true;
            }
        }
    }


    //Game Loop Methods
    public void LoadMainMenuFirstTime()
    {
        SceneManager.LoadScene(1);
    }
    public void ReloadAndResetToMainMenu()
    {
        reloadEvent.Invoke();
        SceneManager.LoadScene(1);
        won = false;
        lost = false;
        gameOver = false;
        reload = false;
        currentPOICollected = false;
    }
    public IEnumerator LoadWinLooseScene(float timeToWait)
    {
        //Important for HUD:
        gameOverEvent.Invoke();

        yield return new WaitForSeconds(timeToWait);

        SceneManager.LoadScene(3);
    }
    public void Pause(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }


    //Singleton security method
    public void DestroyThyself()
    {
        Destroy(gameObject);
        instance = null;
    }


    //Used to trigger right dislpay text
    public void ShowAdvice()
    {
        if (enemyList.currentEnemiesLeft == 0 && won == false)
        {
            findPOITextEvent.Invoke();
            return;
        }

        if (enemyList.Boss.Count == 0 && won == false)
        {
            findPOITextEvent.Invoke();
            return;
        }

        if(currentPOICollected)
        {
            findPOITextEvent.Invoke();
            return;
        }
    }


    #region Load and Save Methods
    public void SaveLevelData()
    {
        saveData.mapCurrentBosses = enemyList.Boss;
        saveData.mapCurrentMinions = enemyList.Minion;
        saveData.mapEntititesCapacity = enemyList.currentEnemiesCapacity;
        saveData.mapCurrentEnemiesLeft = enemyList.currentEnemiesLeft;

        saveData.playerSpeed = playerSpeed.value;
        saveData.playerStrength = playerStrength.value;
        saveData.playerHealth = playerHealth.value;
        saveData.playerName = playerName.value;
        saveData.playerPoints = playerPoints.value;

        saveData.playerDifficulty = gameDifficulty.PlayerDifficulty;
    }
    public void SavePlayerData()
    {
        saveData.playerSpeed = playerSpeed.value;
        saveData.playerStrength = playerStrength.value;
        saveData.playerHealth = playerHealth.value;
        saveData.playerName = playerName.value;
        saveData.playerPoints = playerPoints.value;
    }
    public void LoadHUDData()
    {
        enemyList.Boss = saveData.mapCurrentBosses;
        enemyList.Minion = saveData.mapCurrentMinions;
        enemyList.currentEnemiesCapacity = saveData.mapEntititesCapacity;
        enemyList.currentEnemiesLeft = saveData.mapCurrentEnemiesLeft;
    }
    public void LoadGameDifficulty()
    {
        gameDifficulty.PlayerDifficulty = saveData.playerDifficulty;
    }
    public void ResetPlayerData()
    {
        playerSpeed.value = playerSpeed.initValue;
        playerStrength.value = playerStrength.initValue;
        playerHealth.value = playerHealth.initValue;
        playerName.value = playerName.initValue;
        playerPoints.value = playerPoints.initValue;

        saveData.playerSpeed = playerSpeed.value;
        saveData.playerStrength = playerStrength.value;
        saveData.playerHealth = playerHealth.value;
        saveData.playerName = playerName.value;
        saveData.playerPoints = playerPoints.value;

        Sprite playerSprite = playerPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
        playerSprite = playerDefaultSprite.PlayerDefaultSprite;
    }
    #endregion

    #endregion
}

