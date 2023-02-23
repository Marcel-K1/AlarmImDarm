/*********************************************************************************************
* Project: Alarm Im Darm
* File   : State
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Base class state, making it possible to compare current states and setting next states in the different state machines
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    #region Attributes

    protected Entity entity = null;

    #endregion

    #region Constructors

    public State(Entity entity)
    {
        this.entity = entity;
    }

    #endregion

    #region Methods

    public abstract void Execute();

    #endregion
}
