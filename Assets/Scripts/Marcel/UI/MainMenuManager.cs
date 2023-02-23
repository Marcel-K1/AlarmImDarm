/*********************************************************************************************
* Project: Alarm Im Darm
* File   : MainMenuManager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Manages the canvas for the Main Menu
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private GameObject[] panels;
    [SerializeField]
    private GameObject buttonPanel;
    [SerializeField]
    private TextMeshProUGUI storyText;
    [SerializeField]
    private TextMeshProUGUI tutorialText;
    [SerializeField]
    private TextMeshProUGUI playerInfoText;
    [SerializeField]
    private TextWriterSingle textWriterSingle;

    [SerializeField]
    private ScriptableString playerName;
    [SerializeField]
    private ScriptableInt health;
    [SerializeField]
    private ScriptableInt strength;
    [SerializeField]
    private ScriptableFloat speed;
    [SerializeField]
    private ScriptableInt points;
    [SerializeField]
    private ScriptableFloat shootingCooldown;

    [SerializeField]
    private GameDifficultyScriptableObject gameDifficulty;
    [SerializeField]
    private MapManagerScriptableObject mapData;
    [SerializeField] 
    private SaveDataScriptableObject saveData;
    [SerializeField]
    private EnemyListScriptableObject enemyList;
    [SerializeField]
    private SaveDataScriptableObject mapSaveDataSO;

    #endregion

    #region Properties

    private int playerDifficulty = 0;
    public int PlayerDifficulty { get => playerDifficulty; set => playerDifficulty = value; }

    #endregion

    #region Methods
    void OnEnable()
    {
        if (GameManager.Instance.FirstBoot)
        {
            buttonPanel.SetActive(false);
            SetActivePanel(0);
        }
        else
        {
            buttonPanel.SetActive(true);
            SetActivePanel(1);
        }

        ResetStats();
    }

    private void Start()
    {
        AddWriter(storyText, storyText.text, 0.025f, true);
        SaveSystem.SetPaths();
    }

    private void Update()
    {
        if (!textWriterSingle.textHasFinished && textWriterSingle != null)
        {
            textWriterSingle.Update();
        }

        gameDifficulty.UpdateGameDifficulty(playerDifficulty);
    }

    #region Button Methods
    public void SetActivePanel(int index)
    {
        for (var i = 0; i < panels.Length; i++)
        {
            var active = i == index;
            var g = panels[i];
            if (g.activeSelf != active) g.SetActive(active);
            if (index == 1)
            {
                AddWriter(tutorialText, tutorialText.text, 0.025f, true);
            }
        }
    }
    //Used for First Boot, meaning showing the tutorial text
    public void GoToMenu()
    {
        SetActivePanel(1);
        buttonPanel.SetActive(true);
        GameManager.Instance.FirstBoot = false;
    }
    public void LoadNewGame()
    {
        GameManager.Instance.LoadingPlayerData = false;
        saveData.loadGame = false;
        enemyList.Boss.Clear();
        enemyList.Minion.Clear();
        enemyList.currentEnemiesCapacity = 0;
        GameManager.Instance.CurrentEnemiesLeftPercentage = 0;
        GameManager.Instance.Won = false;
        GameManager.Instance.Lost = false;
        GameManager.Instance.GameOver = false;
        GameManager.Instance.Reload = false;
        SetMapDataToDifficulty();
        SceneManager.LoadScene(2);
    }
    public void LoadSavedLevel()
    {
        SetMapDataToDifficulty();
        SaveSystem.Load(mapSaveDataSO, points);
        GameManager.Instance.LoadingPlayerData = true;
        SceneManager.LoadScene(2);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        GameManager.Instance.DestroyThyself();
#else
                Application.Quit();
#endif
    }
    private void SetMapDataToDifficulty()
    {
        switch (gameDifficulty.PlayerDifficulty)
        {
            case GameDifficultyScriptableObject.GameDifficulty.easy:
                mapData.MapLevelMultiplier = 2.7f;
                mapData.MapLevelEnemyNumberMultiplier = 0.5f;
                break;
            case GameDifficultyScriptableObject.GameDifficulty.medium:
                mapData.MapLevelMultiplier = 3.5f;
                mapData.MapLevelEnemyNumberMultiplier = 1f;
                break;
            case GameDifficultyScriptableObject.GameDifficulty.hard:
                mapData.MapLevelMultiplier = 4.5f;
                mapData.MapLevelEnemyNumberMultiplier = 2f;
                break;
            default:
                break;
        }

    }
    void ResetStats()
    {
        playerName.OnAfterDeserialize();
        health.OnAfterDeserialize();
        strength.OnAfterDeserialize();
        speed.OnAfterDeserialize();
        points.OnAfterDeserialize();
        shootingCooldown.OnAfterDeserialize();
    }
    public void SetDifficulty(int _difficulty)
    {
        PlayerDifficulty = _difficulty;
    }
    #endregion

    //Used for adding instance of nested class
    public void AddWriter(TextMeshProUGUI _textField, string _textToWrite, float _timePerCharacter, bool _invisibleCharacters)
    {
        textWriterSingle = new TextWriterSingle(_textField, _textToWrite, _timePerCharacter, _invisibleCharacters);
    }

    //Used for administer Game Difficulty
    public void LoadGameDifficultyData()
    {
        GameManager.Instance.LoadGameDifficulty();
    }

    #endregion

    //Nested Class um das TMPro Buchstabe für Buchstabe auszugeben
    public class TextWriterSingle
    {
        //Felder
        private TextMeshProUGUI textField;
        private string textToWrite;
        private float timePerCharacter;
        private float timer;
        private int characterIndex;
        private bool invisibleCharacters;

        public bool textHasFinished = false;
    
        //Konstruktor
        public TextWriterSingle(TextMeshProUGUI _textField, string _textToWrite, float _timePerCharacter, bool _invisibleCharacters)
        {
            this.textField = _textField;
            this.textToWrite = _textToWrite;
            this.timePerCharacter = _timePerCharacter;
            this.invisibleCharacters = _invisibleCharacters;
            characterIndex = 0;
    
        }
    
        // Gibt jeden Buchstaben einzeln aus
        public void Update()
        {
            timer -= Time.deltaTime;
            while (timer <= 0f)
            {
                //Nächsten Buchstaben anzeigen
                timer += timePerCharacter;
                characterIndex++;
                string text = textToWrite.Substring(0, characterIndex);
                if (invisibleCharacters)
                {
                    textField.text += "<color=#00000000>" + textToWrite.Substring(0, characterIndex) + "</color>";
                }
                textField.text = text;
                if (characterIndex >= textToWrite.Length)
                {
                    //Ganzer String wurde angezeigt
                    textField = null;
                    textHasFinished = true;
                    return;
                }

            }
        }
    }
}


