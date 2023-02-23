/*****************************************************************************
* Project: Alarm Im Darm
* File   : NPCKillNotifier.cs
* Date   : 06.10.2022
* Author : Martin Stasch (MS)
*
* This script is for NPC tracking. It gets the room manager reference where the NPC was spawned and informs the room manager in case the npc was destroyed.
*
* History:
*	20.10.2022	MS	Created
******************************************************************************/



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCKillNotifier : MonoBehaviour
{
    
    private RoomManager roomManager;

    public RoomManager RoomManager
    {
        set { roomManager = value; }
    }

    /// <summary>
    /// event to inform the room manager of the death of the NPC
    /// </summary>
    private void OnDestroy()
    {
        roomManager.NPCkilled(this.gameObject);
    }
}
