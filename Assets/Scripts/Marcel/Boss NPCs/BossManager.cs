/*********************************************************************************************
* Project: Alarm Im Darm
* File   : BossManager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Managing the Boss Enemies
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    #region Singleton
    private static BossManager instance;
    public static BossManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("BossManager").AddComponent<BossManager>();
                Debug.LogWarning("BossManager instance was missing");
            }
            return instance;
        }
    }
    #endregion

    #region Variables

    [SerializeField]
    private List<Entity> bossEnemies;
    [SerializeField]
    private List<Entity> bossTypes;
    [SerializeField] 
    private BossA enemyBossAPrefab;
    [SerializeField] 
    private BossB enemyBossBPrefab;
    [SerializeField]
    public List<StateMachine> stateMachines;

    private bool itemUnregistered = false;

    #endregion

    #region Properties
    public List<StateMachine> StateMachines { get => stateMachines; set => stateMachines = value; }
    public List<Entity> BossEnemies { get => bossEnemies; set => bossEnemies = value; }
    public List<Entity> BossTypes { get => bossTypes; set => bossTypes = value; }
    #endregion

    #region Methods
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        bossEnemies = new List<Entity>();
        stateMachines = new List<StateMachine>();
        bossTypes = new List<Entity>();

        bossTypes.Add(enemyBossAPrefab);
        bossTypes.Add(enemyBossBPrefab);

        foreach (Entity entity in bossEnemies)
        {

            if (entity is BossA)
            {
                entity.name = $"BossA";
            }
            else if (entity is BossB)
            {
                entity.name = $"BossB";
            }
            else
                return;

            CreateStateMachine(entity);
        }
    }

    void Update()
    {
        for (int i = 0; i < StateMachines.Count; i++)
        {
            StateMachines[i]?.Evaluate();
        }

        if (itemUnregistered)
        {

            for (int i = StateMachines.Count - 1; i >= 0; i--)
            {
                if (StateMachines[i] == null)
                {
                    StateMachines.RemoveAt(i);
                    BossEnemies.RemoveAt(i);
                }
            }
            itemUnregistered = false;
        }

    }

    public void CreateStateMachine(Entity entity)
    {
        StateMachine stateMachine;

        if (entity is BossA)
            stateMachine = new BossAStateMachine((BossA)entity);
        else if (entity is BossB)
            stateMachine = new BossBStateMachine((BossB)entity);
        else
            return;

        stateMachines.Add(stateMachine);
    }
    public void RegisterEntity(Entity entity)
    {
        if (BossEnemies.Contains(entity)) return;

        BossEnemies.Add(entity);
        CreateStateMachine(entity);
    }
    public void UnregisterEntity(Entity entity)
    {
        if (!BossEnemies.Contains(entity)) return;

        int index = BossEnemies.IndexOf(entity);

        BossEnemies[index] = null;
        StateMachines[index] = null;
        itemUnregistered = true;
    }
    #endregion
}
