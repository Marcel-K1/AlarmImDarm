/*********************************************************************************************
* Project: Alarm Im Darm
* File   : WalkingState
* Date   : 19.10.2022
* Author : Marcel Klein
*
* State to describe Walking behaviour of Boss Enemy
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : State
{
    #region Constructors
    public WalkingState(Entity entity) : base(entity) { }
    #endregion

    #region Methods
    public override void Execute()
    {
        entity.CurrentWalkTime += Time.deltaTime;

        if (entity != null)
        {
            if (entity is BossA)
            {
                entity.Move();
            }
            else if (entity is BossB)
            {
                entity.Move();
            }
        }
    }
    #endregion
}
