/*********************************************************************************************
* Project: Alarm Im Darm
* File   : StateMachine
* Date   : 19.10.2022
* Author : Marcel Klein
*
*  Base class for setting up the behaviour of all boss enemies in the simulation
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine
{
    #region Variables

    protected Entity entity = null;
    protected State currentState = null;

    #endregion

    #region Constructors

    protected StateMachine(Entity agent, State startState)
    {
        this.entity = agent;
        currentState = startState;
        this.entity.State = currentState.ToString();
    }

    #endregion

    #region Methods

    //Executing current state, than switching it according to states process in deriving classes for invader and defender
    public void Evaluate()
    {
        currentState?.Execute();
        SwitchState();
    }
    protected abstract void SwitchState();

    #endregion
}
