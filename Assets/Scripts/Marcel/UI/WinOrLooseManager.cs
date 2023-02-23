/*********************************************************************************************
* Project: Alarm Im Darm
* File   : WinOrLooseManager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Manages the win and loose scene
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrLooseManager : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private TextMeshProUGUI wonText;
    [SerializeField]
    private TextMeshProUGUI lostText;
    [SerializeField]
    private TextWriterSingle textWriterSingle;

    #endregion

    #region Methods
    private void Start()
    {
        GameManager.Instance.SceneChangeToFinalScene = false;

        if (GameManager.Instance.Won)
        {
            wonText.gameObject.SetActive(true);
            AddWriter(wonText, wonText.text, 0.025f, true);
        }
        else if (GameManager.Instance.Lost)
        {
            lostText.gameObject.SetActive(true);
            AddWriter(lostText, lostText.text, 0.025f, true);
        }
    }

    private void Update()
    {
        if (!textWriterSingle.textHasFinished && textWriterSingle != null)
        {
            textWriterSingle.Update();
        }
    }

    public void AddWriter(TextMeshProUGUI _textField, string _textToWrite, float _timePerCharacter, bool _invisibleCharacters)
    {
        textWriterSingle = new TextWriterSingle(_textField, _textToWrite, _timePerCharacter, _invisibleCharacters);
    }

    private void OnDisable()
    {
        wonText.gameObject.SetActive(false);
        lostText.gameObject.SetActive(false);
    }

    public void GoToMenu()
    {
        GameManager.Instance.ReloadAndResetToMainMenu();    
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
