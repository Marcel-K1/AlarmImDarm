/*********************************************************************************************
* Project: Alarm Im Darm
* File   : BossAStateMachine.cs
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Managing the BossA States
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAStateMachine : StateMachine
{
    #region Constructors
    public BossAStateMachine(BossA agent) : base(agent, new SetDestinationState(agent)) { }
    #endregion

    #region Methods
    protected override void SwitchState()
    {
        BossA bossA = (BossA)entity;

        //Idle Loop
        if (currentState is SetDestinationState)
            {
                currentState = new WalkingState(entity);
            }
        else if (currentState is WalkingState && entity.CurrentWalkTime >= entity.MaxWalkTime)
            currentState = new SetDestinationState(entity);
        else if (currentState is WalkingState && bossA.WallTrigger)
        {

            bossA.MovementDirection *= -1f;
            bossA.MaxWalkTime = 0f;
            bossA.WallTrigger = false;
        }

        //Rage Loop
        else if (currentState is SetDestinationState && (bossA.Health <= 50) && !entity.InRage || currentState is WalkingState && (bossA.Health <= 50) && !entity.InRage)
            {
                currentState = new RageState(entity);
                bossA.FireRate = 1.5f;
                entity.InRage = true; 
            }
        else if (currentState is RageState)
            currentState = new SetDestinationState(entity);

        this.entity.State = currentState.ToString();
    }
    #endregion
}
