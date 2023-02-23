/*****************************************************************************
* Project: Alarm Im Darm
* File   : MapManagerScriptableObject.cs
* Date   : 06.10.2022
* Author : Martin Stasch (MS)
*
* data script for map generation
*
* History:
*	06.10.2022	MS	Created
******************************************************************************/


using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(fileName = "MapManagerScriptableObject", menuName = "ScriptableObject/MapManagerSO")]
public class MapManagerScriptableObject : ScriptableObject
{

    public int MapLevel = 1;
    public float MapLevelMultiplier = 2.6f;

    public int EnemyBaseNumberPerRoom = 2;
    public float MapLevelEnemyNumberMultiplier = 1.5f;

}
