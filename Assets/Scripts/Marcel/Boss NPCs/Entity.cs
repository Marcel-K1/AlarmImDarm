/*********************************************************************************************
* Project: Alarm Im Darm
* File   : Entity
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Base class for all boss enemies, setting up properties for the deriving classes
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    #region Variables
    //Current State the entity is in
    [SerializeField]
    private string state;
    [SerializeField]
    private bool inRage = false;

    //Used for walking along a random direction as long as currentWalkTime<maxWalkTime
    [SerializeField]
    private float currentWalkTime, maxWalkTime;
    //Used for chasing along as long as currentChaseWalkTime<maxChaseTime
    [SerializeField]
    private float currentChaseTime, maxChaseTime;

    [SerializeField]
    private Vector3 movementDirection = Vector3.zero;

    //Used for tracking the player
    [SerializeField]
    private float attentionRadius = 8f;
    [SerializeField]
    private LayerMask mask = 6;

    //Used for changing appearance according to different states
    protected SpriteRenderer spriteRenderer = null;
    
    [SerializeField]
    public EnemyListScriptableObject enemyList;
    #endregion

    #region Properties
    public SpriteRenderer Renderer => spriteRenderer;
    public Vector3 MovementDirection { get => movementDirection; set => movementDirection = value; }
    public string State { get => state; set => state = value; }
    public float CurrentWalkTime { get => currentWalkTime; set => currentWalkTime = value; }
    public float MaxWalkTime { get => maxWalkTime; set => maxWalkTime = value; }
    public bool InRage { get => inRage; set => inRage = value; }
    public LayerMask Mask { get => mask; set => mask = value; }
    public float AttentionRadius { get => attentionRadius; set => attentionRadius = value; }
    public float CurrentChaseTime { get => currentChaseTime; set => currentChaseTime = value; }
    public float MaxChaseTime { get => maxChaseTime; set => maxChaseTime = value; }
    #endregion

    #region Methods
    public abstract void Move();
    public abstract void TakeDamage(int damage);
    #endregion
}
