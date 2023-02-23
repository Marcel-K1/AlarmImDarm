/*********************************************************************************************
* Project: Alarm Im Darm
* File   : HUDManager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Managing the players HUD display
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    #region Variables
    [Tooltip("UI Text to display Player's Points")]
    [SerializeField]
    private TextMeshProUGUI playerPointsText;
	[Tooltip("UI Text to display Player's Health")]
	[SerializeField]
	private TextMeshProUGUI playerStrengthText;	
	[Tooltip("UI Text to display Player's Stamina")]
	[SerializeField]
	private TextMeshProUGUI playerStaminaText;
	[Tooltip("UI Text to display Player's POI")]
	[SerializeField]
	private TextMeshProUGUI pointsOfInterestText;


	[Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;
	[SerializeField]
	private Gradient playerHealthGradient;
	[SerializeField]
	private Image playerFill;

	[Tooltip("UI Text to display earned points at the end")]
	[SerializeField]
	private TextMeshProUGUI pointsEarnedText;

	[Tooltip("UI Slider to display Shit Load")]
	[SerializeField]
	private Slider levelShitLoadSlider;
	[SerializeField]
	private Gradient levelShitLoadGradient;
	[SerializeField]
	private Image shitFill;

	[Tooltip("Player Stats References")]
	[SerializeField]
	private ScriptableInt health;
	[SerializeField]
	private ScriptableInt points;
	[SerializeField]
	private ScriptableFloat speed;
	[SerializeField]
	private ScriptableInt strength;
	[SerializeField]
	private ScriptableString playerName;

	[Tooltip("Other Helpful References")]
	[SerializeField]
	private EnemyListScriptableObject enemyList;
    [SerializeField]
    private SaveDataScriptableObject saveData;
    [SerializeField]
	private MapManager mapManager;

	#endregion

	#region Methods

	private void OnEnable()
    {
        GameManager.Instance.GameOverEvent += OnGameOver;
    }

    private void Start()
    {
        if (saveData.loadGame)
        {
			GameManager.Instance.LoadHUDData();
		}
    }

    private void Update()
    {
		SetHealth(health.value);
		SetPoints(points.value);
		SetStrength(strength.value);
		SetSpeed(speed.value);
		SetShit();
		SetPointsOfInterest();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOverEvent -= OnGameOver;
        }
        pointsEarnedText.gameObject.SetActive(false);
    }

	//Method to display points earned text
    private void OnGameOver()
    {
        if (GameManager.Instance.Won || GameManager.Instance.Lost)
        {
            if (playerName.value != "")
            {
				pointsEarnedText.text = $"{playerName.value} hat {points.value} Punkte verdient!";
			}
			else
            {
				pointsEarnedText.text = $"{playerName.initValue} hat {points.value} Punkte verdient!";
			}
			pointsEarnedText.gameObject.SetActive(true);
		}
	}

    #region UI Methods

    private void SetPointsOfInterest()
    {
		//pointsOfInterestText.text = $"Fremdkörper: {saveData.CollectedPOI}/{mapManager.POIPrefab.Length}";
		pointsOfInterestText.text = $"Fremdkörper: {GameManager.Instance.POIData.CollectedPOI.Count}/{mapManager.POIPrefab.Length}";
	}

    private void SetShit()
    {
		levelShitLoadSlider.maxValue = enemyList.currentEnemiesCapacity;
		levelShitLoadSlider.value = (enemyList.currentEnemiesCapacity - enemyList.Minion.Count) - enemyList.Boss.Count;
		shitFill.color = levelShitLoadGradient.Evaluate(levelShitLoadSlider.normalizedValue);
	}

    public void SetMaxHealth(int health)
	{	
		playerHealthSlider.maxValue = health;
		playerHealthSlider.value = health;

		playerFill.color = playerHealthGradient.Evaluate(1f);
			
	}

	public void SetHealth(int health)
	{ 
		playerHealthSlider.value = health;
		playerFill.color = playerHealthGradient.Evaluate(playerHealthSlider.normalizedValue);
	}

	public void SetPoints(float points)
	{
		playerPointsText.text = $"Punkte: {points}";
	}

	public void SetSpeed(float speed)
	{
		playerStaminaText.text = $"Geschwindigkeit: {speed}";
	}

	public void SetStrength(float strength)
    {
		playerStrengthText.text = $"Stärke: {strength}";
    }

	#endregion

	#endregion
}