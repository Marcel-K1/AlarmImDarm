using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyListScriptableObject", menuName = "ScriptableObject/EnemyListSO")]
public class EnemyListScriptableObject : ScriptableObject
{
    [SerializeField]
    private List<GameObject> minion;
    public List<GameObject> Minion { get => minion; set => minion = value; }

    [SerializeField]
    public List<GameObject> boss;
    public List<GameObject> Boss { get => boss; set => boss = value; }

    public int currentEnemiesCapacity;
    public int currentEnemiesLeft;
}
