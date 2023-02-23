using UnityEngine;
using UnityEngine.UI;

/*********************************************************************************************
* Project: Alarm Im Darm
* File   : MenuAudioManager
* Date   : 15.11.2022
* Author : Levi
*
* This script is used to play the right backgroundMusic in the "Menu"-Scene
* 
*********************************************************************************************/

public class MenuAudioManager : MonoBehaviour
{
    [SerializeField]
    Slider sfxSlider;

    float sfxVolume;

    private void Start()
    {
        if (!GameManager.Instance.FirstBoot)
        {
            AudioManager.instance.PlayMusic(EMusic.MainMenu);
        }
    }

    //gets called to make a buttonSound
    public void PressButton()
    {
        AudioManager.instance.PlaySound(ESounds.Button);
    }

    //gets called to change the soundVolume
    public void ChangeSFXVolume(float volume)
    {
        AudioManager.instance.ChangeSFXVolume(volume);
    }

    //gets called to mute the sounds
    public void MuteSFX(bool isMuted)
    {
        if (isMuted)
        {
            sfxSlider.interactable = false;
            sfxVolume = AudioManager.instance.sfxVolume;
            AudioManager.instance.ChangeSFXVolume(0);
        }
        else
        {
            sfxSlider.interactable = true;
            sfxSlider.IsInteractable();
            ChangeSFXVolume(sfxVolume);
        }
    }
}
