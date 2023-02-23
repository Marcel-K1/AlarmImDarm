/*********************************************************************************************
* Project: Alarm Im Darm
* File   : RageState
* Date   : 19.10.2022
* Author : Marcel Klein
*
* State to describe Rage behaviour of Boss Enemy
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RageState : State
{
    #region Constructors
    public RageState(Entity entity) : base(entity) { }
    #endregion

    #region Methods
    public override void Execute()
    {
        if (entity != null)
        {
            if (entity is BossA)
            {
                entity.AttentionRadius = 16f;
                entity.gameObject.transform.localScale = new Vector3 (3,3,1);
                entity.Renderer.color = Color.red;
            }
            else if (entity is BossB)
            {
                entity.AttentionRadius = 16f;
                entity.gameObject.transform.localScale = new Vector3(3, 3, 1);
                entity.Renderer.color = Color.red;
                entity.Move();
            }
        }
    }
    #endregion
}
