/*********************************************************************************************
* Project: Alarm Im Darm
* File   : SetDestinationState.cs
* Date   : 19.10.2022
* Author : Marcel Klein
*
* State for setting up the walking destination for Boss Enemies in Idle
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDestinationState : State
{
    #region Constructors
    public SetDestinationState(Entity entity) : base(entity) { }
    #endregion

    #region Methods
    public override void Execute()
    {
        if (entity != null)
        {
            //Used for walking along a random direction
            Vector2 randomDirection = Random.insideUnitCircle*4;
            entity.MovementDirection = new Vector3(randomDirection.x, randomDirection.y, 0).normalized;
            entity.CurrentWalkTime = 0f;
            entity.CurrentChaseTime = 0f;
            entity.MaxChaseTime = Random.Range(2f, 5f);
            entity.MaxWalkTime = Random.Range(2f, 5f);
        }
    }
    #endregion
}
