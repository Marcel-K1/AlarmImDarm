/*****************************************************************************
* Project: Alarm Im Darm
* File   : SaveDataScriptableObject.cs
* Date   : 06.10.2022
* Author : Martin Stasch (MS)
*
* Datascript used for load and save function
*
* History:
*	20.10.2022	MS	Created
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataScriptableObject", menuName = "ScriptableObject/SaveDataSO")]
public class SaveDataScriptableObject : ScriptableObject
{
    //Map Data
    public bool loadGame = false;
    public int[] map;
    public bool[] spawnEntities;
    public Vector2Int playerPosAtLoad;

    //HUD & Game Manager Data
    public int mapEntititesCapacity;
    public List<GameObject> mapCurrentMinions;
    public List<GameObject> mapCurrentBosses;
    public int mapCurrentEnemiesLeft;
    public List<string> CollectedPOI;

    //Editor & Player Data
    public float playerSpeed;
    public int playerStrength;
    public int playerHealth;
    public string playerName;
    public int playerPoints;
    public GameDifficultyScriptableObject.GameDifficulty playerDifficulty;

}
