using UnityEngine;

/*********************************************************************************************
* Project: Alarm Im Darm
* File   : GameAudioManager
* Date   : 15.11.2022
* Author : Levi
*
* This script is used to play the right backgroundMusic in the "Level"-Scene
* 
*********************************************************************************************/

public class GameAudioManager : MonoBehaviour
{
    [SerializeField]
    GameDifficultyScriptableObject diff;

    private void Start()
    {
        //plays the right audio, depending on the difficulty
        switch (diff.PlayerDifficulty)
        {
            case GameDifficultyScriptableObject.GameDifficulty.easy:
                AudioManager.instance.PlayMusic(EMusic.LevelEasy);
                break;
            case GameDifficultyScriptableObject.GameDifficulty.medium:
                AudioManager.instance.PlayMusic(EMusic.LevelMedium);
                break;
            case GameDifficultyScriptableObject.GameDifficulty.hard:
                AudioManager.instance.PlayMusic(EMusic.LevelHard);
                break;
        }
    }

    //gets called to make a buttonSound
    public void PressButton()
    {
        AudioManager.instance.PlaySound(ESounds.Button);
    }
}
