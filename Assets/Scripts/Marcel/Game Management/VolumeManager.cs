/*********************************************************************************************
* Project: Alarm Im Darm
* File   : Volume Manager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Setting up the Volume Management System
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private AudioMixer m_musicMixer;

    [SerializeField]
    private Slider m_musicVolumeSlider;
    private float m_musicVolume = Mathf.Log10(0.5f) * 20;

    [SerializeField]
    private Slider m_sfxVolumeSlider;
    //private float m_sfxVolume = 0.4f;

    #endregion

    #region Methods
    public void Start()
    {
        SetMusicVolumeSlider();
        SetSFXVolumeSlider();
    }

    //Button methods and saving the values in player prefs
    public void ChangeMusicVolume(float _value)
    {
        PlayerPrefs.SetFloat("PlayerMusicVolume", Mathf.Log10(_value) * 20);
        PlayerPrefs.Save();
        m_musicMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("PlayerMusicVolume", Mathf.Log10(0.5f) * 20));
    }
    public void SetMusicVolumeSlider()
    {
        //if (m_musicVolumeSlider != null)
        //{
            m_musicVolumeSlider.value = 0.5f;
        //}
        //else
        //{
        //    ChangeMusicVolume(0f);
        //}

    }
    public void MuteMusic(bool _mute)
    {
        if (_mute)
        {
            if (m_musicMixer.GetFloat("MusicVolume", out float volume))
            {
                m_musicVolume = volume;
            }
            m_musicVolumeSlider.interactable = false;
            m_musicMixer.SetFloat("MusicVolume", Mathf.Log10(-80f) * 20);
        }
        else
        {
            m_musicVolumeSlider.interactable = true;
            m_musicMixer.SetFloat("MusicVolume", m_musicVolume);
        }
    }

    public void ChangeSFXVolume(float _value)
    {
        PlayerPrefs.SetFloat("PlayerEffectsVolume", _value);
        PlayerPrefs.Save();
        AudioManager.instance.ChangeSFXVolume(PlayerPrefs.GetFloat("PlayerEffectsVolume", 0.4f));
        AudioManager.instance.PlaySound(ESounds.Button);
    }
    public void SetSFXVolumeSlider()
    {
        m_sfxVolumeSlider.value = PlayerPrefs.GetFloat("PlayerEffectsVolume",0.4f);
    }
    public void MuteEffects(bool _mute)
    {
        if (_mute)
        {
            m_sfxVolumeSlider.interactable = false;
            AudioManager.instance.sfxVolume = 0;
        }
        else
        {
            m_sfxVolumeSlider.interactable = true;
            AudioManager.instance.ChangeSFXVolume(PlayerPrefs.GetFloat("PlayerEffectsVolume", 0.4f));
        }
    }
    #endregion
}
