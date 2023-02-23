/*********************************************************************************************
* Project: Alarm Im Darm
* File   : FartalotManager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Class to implement the story buddy feature. Giving text messages according to certain events.
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FartalotManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private List<string> enemyCountTexts;

    [SerializeField]
    private TextMeshProUGUI messageText;

    [SerializeField]
    private GameObject fartalotGO;

    [SerializeField]
    private MapManager mapManager;

    private TextWriterSingle textWriterSingle;

    #endregion

    #region Methods
    private void OnEnable()
    {
        GameManager.Instance.EnemyCountTextEvent += OnEnemyCountTextEvent;
        GameManager.Instance.FindPOITextEvent += OnFindPOITextEvent;
    }

    private void Start()
    {
        enemyCountTexts.Add("Na hier ist ja was los, da bleibt wohl keine Kloschüssel trocken heute.");
        enemyCountTexts.Add("Seien Sie froh, dass es nur hinten rauskommt!");
        enemyCountTexts.Add("1,2,3 hier kommt ein Zäpfchen!");
        enemyCountTexts.Add("Alles raus jetzt, was keine Miete zahlt!");
        enemyCountTexts.Add("Da schlägt wohl Montezumas Rache wieder zu!");
        enemyCountTexts.Add("Ein Furz, ist ein Furz, ist ein Furz, ist ein Furz.");
        enemyCountTexts.Add("Der Scheiß ist heiß!");
        enemyCountTexts.Add("Nehmen Sie's mit Humor Ihre Durchfurztheit");
        enemyCountTexts.Add("Ohne Scheiß kein Preis!");

        Image[] images = fartalotGO.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            Color tmp = image.color;
            tmp.a = 0.6f;
            image.color = tmp;
        }

        fartalotGO.SetActive(false);
    }

    private void Update()
    {
        if (textWriterSingle != null)
        {
            if (!textWriterSingle.textHasFinished)
            {
                textWriterSingle.Update();
            }
            else if (textWriterSingle.textHasFinished)
            {
                StartCoroutine(WaitForSeconds(2));
            }
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyCountTextEvent -= OnEnemyCountTextEvent;
            GameManager.Instance.FindPOITextEvent -= OnFindPOITextEvent;
            if (mapManager != null)
            {
                if (mapManager.PointsOfInterest.Count != 0)
                {
                    foreach (var pointOfInterest in mapManager.PointsOfInterest)
                    {
                        pointOfInterest.GetComponent<PointOfInterest>().POITextEvent -= OnPOITextEvent;
                    }

                }
            }
        }
    }

    //Methods for displaying the right text
    public void OnPOITextEvent(string pOIText)
    {
        fartalotGO.SetActive(true);
        messageText.text = pOIText;
        AddWriter(messageText, messageText.text, 0.04f, true);
    }

    private void OnEnemyCountTextEvent()
    {
        fartalotGO.SetActive(true);
        int rndIndex = UnityEngine.Random.Range(0, enemyCountTexts.Count);
        messageText.text = enemyCountTexts[rndIndex];   
        AddWriter(messageText, messageText.text, 0.04f, true);
    }

    private void OnFindPOITextEvent()
    {
        fartalotGO.SetActive(true);
        messageText.text = "Ihr Virus lebt immernoch? Entweder sind noch Immunzellen übrig, " +
            "oder es ist noch auf der Suche nach etwas Besonderem?!";
        AddWriter(messageText, messageText.text, 0.04f, true);
    }

    private IEnumerator WaitForSeconds(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        fartalotGO.SetActive(false);
        textWriterSingle = null;
    }

    public void AddWriter(TextMeshProUGUI _textField, string _textToWrite, float _timePerCharacter, bool _invisibleCharacters)
    {
        textWriterSingle = new TextWriterSingle(_textField, _textToWrite, _timePerCharacter, _invisibleCharacters);
    }
    #endregion

    //Nested Class um das TMPro Buchstabe f�r Buchstabe auszugeben
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
                //N�chsten Buchstaben anzeigen
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
