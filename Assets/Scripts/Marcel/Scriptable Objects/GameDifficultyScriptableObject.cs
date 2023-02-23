using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficultyScriptableObject", menuName = "ScriptableObject/GameDifficultySO")]
public class GameDifficultyScriptableObject : ScriptableObject
{
    public enum GameDifficulty
    {
        easy,
        medium,
        hard
    }

    [SerializeField]
    private GameDifficulty playerDifficulty = 0;
    public GameDifficulty PlayerDifficulty { get => playerDifficulty; set => playerDifficulty = value; }

    public void UpdateGameDifficulty(int _difficulty)
    {
        playerDifficulty = (GameDifficulty)_difficulty;
    }
}
