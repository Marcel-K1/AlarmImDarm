/*********************************************************************************************
* Project: Alarm Im Darm
* File   : EditorManager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Class to implement the player character editing feature
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using AnotherFileBrowser;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;

public class EditorManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private TextMeshProUGUI playerNameText;
    [SerializeField]
    private TextMeshProUGUI playerPointsText;
    [SerializeField]
    private TextMeshProUGUI playerHealthText;
    [SerializeField]
    private TextMeshProUGUI playerStrengthText;
    [SerializeField]
    private TextMeshProUGUI playerSpeedText;
    [SerializeField]
    private GameObject errorPanelSize;
    [SerializeField]
    private GameObject errorPanelPoints;

    [SerializeField]
    ScriptableString playerName;
    [SerializeField]
    ScriptableInt points;
    [SerializeField]
    ScriptableInt health;
    [SerializeField]
    ScriptableFloat speed;
    [SerializeField]
    ScriptableInt strength;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject playerBulletPrefab;
    [SerializeField]
    private RawImage image;
    [SerializeField]
    ScriptableInt playerSpriteSize;
    [SerializeField]
    private Slider playerSizeSlider = null;
    [SerializeField]
    private PlayerSpriteScriptableObject playerDefaultSprite;

    private string path;
    private Texture2D tex;
    private byte[] imageBytes;
    #endregion

    #region Methods
    private void Start()
    {
        if (playerName.value == "")
        {
            playerNameText.text = playerName.initValue;
            playerName.value = playerName.initValue;
        }
        image.texture = playerPrefab.GetComponentInChildren<SpriteRenderer>().sprite.texture;
    }

    private void Update()
    {
        playerNameText.text = $"{playerName.value}";
        playerPointsText.text = $"Verfügbare Punkte: {points.value}";
        playerHealthText.text = $"{health.value}";
        playerStrengthText.text = $"{strength.value}";
        playerSpeedText.text = $"{speed.value}";
    }

    #region Button Methods

    public void SetName()
    {
        playerName.value = playerNameText.text;
    }

    public void SetHealth()
    {
        if (points.value >= 10)
        {
            health.value += 10;
            points.value -= 10;
        }
        else
        {
            StartCoroutine(ShowErrorPanelPoints());
        }

    }

    public void SetStrength()
    {
        if (points.value >= 10)
        {
            strength.value += 1;
            points.value -= 10;
        }
        else
        {
            StartCoroutine(ShowErrorPanelPoints());
        }
    }

    public void SetSpeed()
    {
        if (points.value >= 10)
        {
            speed.value += 1;
            points.value -= 10;
        }
        else
        {
            StartCoroutine(ShowErrorPanelPoints());
        }
    }

    public void SetSize()
    {
        playerSpriteSize.value = (int)playerSizeSlider.value;
        image.rectTransform.sizeDelta = new Vector2 (playerSpriteSize.value, playerSpriteSize.value);
    }

    public void ResetImage()
    {
        image.texture = playerDefaultSprite.PlayerDefaultSprite.texture;
    }

    public void SavePlayerData()
    {
        GameManager.Instance.SavePlayerData();
    }

    public void ResetPlayerDataAndPOIData()
    {
        GameManager.Instance.ResetPlayerData();
        playerNameText.text = playerName.value;
    }
    #endregion

    #region Image Editor Methods
    public void OpenExplorer()
    {
        if (points.value >= 30)
        {
            var bp = new BrowserProperties();
            bp.filter = "Image files(*.png) | *.png";
            bp.filterIndex = 0;
    
            new FileBrowser().OpenFileBrowser(bp, filepath =>
            {
                path = filepath;
                StartCoroutine(LoadImage(filepath));
            });
        }
        else
        {
            StartCoroutine(ShowErrorPanelPoints());
        }
      
    }

    IEnumerator LoadImage(string filepath)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filepath))
        {
            yield return uwr.SendWebRequest();
            UnityWebRequest.Result requestResult = uwr.result;

            if (requestResult == UnityWebRequest.Result.ConnectionError || requestResult == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                if (uwrTexture.width == playerSpriteSize.value && uwrTexture.height == playerSpriteSize.value)
                {
                    GetImage();
                }
                else
                {
                    StartCoroutine(ShowErrorPanelSize());
                    yield return null;
                }
            }

        }
    }

    private void GetImage()
    {
        if (path != null)
        {
            UpdateImage();
            SaveImage();
        }
    }

    private void UpdateImage()
    {
        imageBytes = System.IO.File.ReadAllBytes(path);

        tex = new Texture2D(playerSpriteSize.value, playerSpriteSize.value);
        tex.LoadImage(imageBytes);
        tex.Apply();
        image.texture = tex;
    }

    public void SaveImage()
    {
        if (points.value >= 30 && imageBytes != null)
        {
            Sprite playerSprite = playerPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
            Sprite playerBulletSprite = playerBulletPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
            playerSprite.texture.LoadImage(imageBytes);
            playerSprite.texture.Apply();            
            playerBulletSprite.texture.LoadImage(imageBytes);
            playerBulletSprite.texture.Apply();
            points.value -= 30;
        }
    }

    IEnumerator ShowErrorPanelSize()
    {
        errorPanelSize.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        errorPanelSize.gameObject.SetActive(false);
    }

    IEnumerator ShowErrorPanelPoints()
    {
        errorPanelPoints.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        errorPanelPoints.gameObject.SetActive(false);
    }
    #endregion


    #endregion
}
