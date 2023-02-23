using System.Collections;
using UnityEngine;
using NaughtyAttributes;

/*********************************************************************************************
* Project: Alarm Im Darm
* File   : AudioManager
* Date   : 15.11.2022
* Author : Levi
*
* This is the Manager for playing the backgroundMusic and soundeffects
* 
*********************************************************************************************/

public enum EMusic
{
    MainMenu,
    LevelEasy,
    LevelMedium,
    LevelHard,
    Boss,
    Win,
    Lose,
}

public enum ESounds
{
    Button,
    NPCDeath,
    NPCShoot,
    PlayerDamage,
    PlayerShoot,
    Collectable,
    PointofInterest,
}

public class AudioManager : Singleton<AudioManager>
{
    #region Declarations

    [SerializeField]
    AudioSource ass;

    [SerializeField, Header("Music"), HorizontalLine]
    AudioClip mainMenu;

    [SerializeField]
    AudioClip levelEasy, levelMedium, levelHard, boss, win, lose;

    [SerializeField, Header("SFX"), HorizontalLine]
    AudioClip[] farts;

    [SerializeField]
    AudioClip[] npcDeath;

    [SerializeField]
    AudioClip npcShoot, playerDamage, playerShoot, collectable, pointOfInterest;

    [SerializeField]
    public float sfxVolume = 0.4f;

    #endregion
    #region Start

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        PlayMusic(EMusic.MainMenu);
        StartCoroutine(CheckMusic(.5f));
    }

    #endregion
    #region Playing Audio

    //changes the backgroundmusic
    public void PlayMusic(EMusic music)
    {
        ass.Stop();
        ass.clip = ConvertMusicEnumToAudioClip(music);
        ass.Play();
    }

    //plays a soundEffect
    public void PlaySound(ESounds sound)
    {
        ass.PlayOneShot(ConvertSoundEnumToAudioClip(sound), sfxVolume);
    }

    //repeats the backgroundmusic if it stopped playing
    IEnumerator CheckMusic(float time)
    {
        yield return new WaitForSeconds(time);
        if (!ass.isPlaying)
        {
            ass.Play();
        }
        StartCoroutine(CheckMusic(time));
    }

    #endregion
    #region Converter

    //converts the enum EMusic into the corresponding soundclip
    AudioClip ConvertMusicEnumToAudioClip(EMusic music)
    {
        AudioClip clip = null;

        switch (music)
        {
            case EMusic.MainMenu:
                clip = mainMenu;
                break;
            case EMusic.LevelEasy:
                clip = levelEasy;
                break;
            case EMusic.LevelMedium:
                clip = levelMedium;
                break;
            case EMusic.LevelHard:
                clip = levelHard;
                break;
            case EMusic.Boss:
                clip = boss;
                break;
            case EMusic.Win:
                clip = win;
                break;
            case EMusic.Lose:
                clip = lose;
                break;
            default:
                Debug.LogError("Music doesn't exist");
                break;
        }

        return clip;
    }

    //converts the enum ESounds into the corresponding soundclip
    AudioClip ConvertSoundEnumToAudioClip(ESounds sound)
    {
        AudioClip clip = null;

        switch (sound)
        {
            case ESounds.Button:
                clip = farts[Random.Range(0, farts.Length)];
                break;
            case ESounds.NPCDeath:
                clip = npcDeath[Random.Range(0, npcDeath.Length)];
                break;
            case ESounds.NPCShoot:
                clip = npcShoot;
                break;
            case ESounds.PlayerDamage:
                clip = playerDamage;
                break;
            case ESounds.PlayerShoot:
                clip = playerShoot;
                break;
            case ESounds.Collectable:
                clip = collectable;
                break;
            case ESounds.PointofInterest:
                clip = pointOfInterest;
                break;
            default:
                Debug.LogError("Sound doesn't exist");
                break;
        }

        return clip;
    }

    #endregion

    //gets called by the sfxSlider to change the volume of sounds
    public void ChangeSFXVolume(float volume)
    {
        sfxVolume = volume;
    }
}
