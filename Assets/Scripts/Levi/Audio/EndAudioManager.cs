using UnityEngine;

/*********************************************************************************************
* Project: Alarm Im Darm
* File   : EndAudioManager
* Date   : 15.11.2022
* Author : Levi
*
* This script is used to play the right backgroundMusic in the "Win"-Scene
* 
*********************************************************************************************/

public class EndAudioManager : MonoBehaviour
{
    private void Start()
    {
        //plays the right audio, depending on the endresult
        if (GameManager.Instance.Won)
        {
            AudioManager.instance.PlayMusic(EMusic.Win);
        }
        else
        {
            AudioManager.instance.PlayMusic(EMusic.Lose);
        }
    }

    //gets called to make a buttonSound
    public void PressButton()
    {
        AudioManager.instance.PlaySound(ESounds.Button);
    }
}
