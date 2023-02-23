/*********************************************************************************************
* Project: Alarm Im Darm
* File   : PauseMenuManager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Manages the canvas for the Pause Menu
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private GameObject[] panels;
    [SerializeField]
    private GameObject buttonPanel;

    [SerializeField]
    private SaveDataScriptableObject mapSaveData;
    [SerializeField]
    private ScriptableInt playerPoints;
    #endregion

    #region Events
    private static event System.Action safeEvent = () => { };

    public event System.Action SafeEvent { add { safeEvent += value; } remove { safeEvent -= value; } }
    #endregion

    #region Methods

    private void OnEnable()
    {
        GameManager.Instance.PauseEvent += OnPaused;
    }

    public void OnPaused()
    {
        buttonPanel.SetActive(true);
        SetActivePanel(0);
    }

    public void SetActivePanel(int index)
    {
        for (var i = 0; i < panels.Length; i++)
        {
            var active = i == index;
            var g = panels[i];
            if (g.activeSelf != active) g.SetActive(active);
        }
    }

    public void CloseActivePanel(int index)
    {
        for (var i = 0; i < panels.Length; i++)
        {
            var active = i == index;
            var g = panels[i];
            if (g.activeSelf == active) g.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseEvent -= OnPaused;
        }

    }

    public void BackToLevel()
    {
        GameManager.Instance.Paused = false;
        GameManager.Instance.Pause(GameManager.Instance.Paused);
        buttonPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.Paused = false;
        GameManager.Instance.Pause(GameManager.Instance.Paused);
        GameManager.Instance.ReloadAndResetToMainMenu();
        buttonPanel.SetActive(false);
    }

    public void Safe()
    {
        safeEvent.Invoke();
        SaveSystem.Save(mapSaveData, playerPoints);
    }

    #endregion
}
