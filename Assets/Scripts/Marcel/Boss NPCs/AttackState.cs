/*********************************************************************************************
* Project: Alarm Im Darm
* File   : AttackState
* Date   : 19.10.2022
* Author : Marcel Klein
*
* State to describe Attack behaviour of Boss Enemy
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    #region Constructors
    public AttackState(Entity entity) : base(entity) { }
    #endregion

    #region Methods
    public override void Execute()
    {
        entity.CurrentChaseTime += Time.deltaTime;

        if (entity != null)
        {
            entity.Move();
        }
    }
    #endregion
}
