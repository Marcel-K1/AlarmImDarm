/*********************************************************************************************
* Project: Alarm Im Darm
* File   : BossBStateMachine.cs
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

public class BossBStateMachine : StateMachine
{
    #region Constructors
    public BossBStateMachine(BossB agent) : base(agent, new SetDestinationState(agent)) { }
    #endregion

    #region Methods
    protected override void SwitchState()
    {
        BossB bossB = (BossB)entity;

        //Idle Loop
        if (currentState is SetDestinationState)
            currentState = new WalkingState(entity);
        else if (currentState is WalkingState && entity.CurrentWalkTime >= entity.MaxWalkTime)
        {
            currentState = new SetDestinationState(entity);
            bossB.CollisionHappened = false;
        }
        else if (currentState is WalkingState && bossB.WallTrigger)
        {
            bossB.MovementDirection *= -1;
            bossB.MaxWalkTime = 0f;
            bossB.WallTrigger = false;
        }
            
        //Attack Loop
        else if (currentState is SetDestinationState && bossB.PlayerCollider != null && bossB.CollisionHappened == false || currentState is WalkingState && bossB.PlayerCollider != null && bossB.CollisionHappened == false)
            currentState = new AttackState(entity);
        else if (currentState is AttackState && bossB.CurrentChaseTime >= bossB.MaxChaseTime || currentState is AttackState && bossB.CollisionHappened)
        {
            currentState = new SetDestinationState(entity);
        }

        //Rage Loop
        else if (currentState is SetDestinationState && (bossB.Health <= 50) && !entity.InRage || 
            currentState is WalkingState && (bossB.Health <= 50) && !entity.InRage ||
            currentState is AttackState &&(bossB.Health <= 50) && !entity.InRage)
        {
            currentState = new RageState(entity);
            bossB.ChaseSpeed = 14f;
            entity.InRage = true;
        }


        this.entity.State = currentState.ToString();

    }
    #endregion
}
