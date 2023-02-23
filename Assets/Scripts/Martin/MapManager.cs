/*****************************************************************************
* Project: Alarm Im Darm
* File   : MapManager.cs
* Date   : 06.10.2022
* Author : Martin Stasch (MS)
*
* Main map generation method that controls the creation of the dungeon at start of the level
*
* History:
*	06.10.2022	MS	Created
******************************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int MAP_WIDTH { get; } = 9;
    public int MAP_HEIGTH { get; } = 8;
    public float ROOM_SIZE_X { get; } = 36f;
    public float ROOM_SIZE_Y  { get; } = 20f;

    public Vector3 Scale 
    {
        get { return this.gameObject.transform.localScale; }
    }

    private DungenRoomCreator dungenRoomCreator;

    [SerializeField]
    public List<GameObject> PointsOfInterest;

    public bool GameLoaded { get; private set; }
    
    
    public Vector2Int PlayerPosition 
    {
        get { return saveData.playerPosAtLoad; }
        set { saveData.playerPosAtLoad = value; }
    }
    
    
    [SerializeField] private GameObject[] enemyMinionPrefab;
    [SerializeField] private GameObject[] enemyBossPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] itemPrefab;
    [SerializeField] private GameObject[] pOIPrefab;
    [SerializeField] private GameObject fartalotCanvas;
    public GameObject FartalotCanvas 
    { get => fartalotCanvas; 
      set => fartalotCanvas = value; 
    }


    public GameObject[] EnemyMinionPrefab
    {
        get { return enemyMinionPrefab; }
    }
    public GameObject[] EnemyBossPrefab
    {
        get { return enemyBossPrefab; }
    }
    public GameObject PlayerPrefab
    {
        get { return playerPrefab; }
    }
    public GameObject[] ItemPrefab
    {
        get { return itemPrefab; }
    }
    public GameObject[] POIPrefab
    {
        get { return pOIPrefab; }
    }

    public int MinMinionNumber
    {
        get { return (int)(data.MapLevel * data.EnemyBaseNumberPerRoom * data.MapLevelEnemyNumberMultiplier); }
    }
    
    public enum ERooms
    {
        invalid = -1,
        free,
        start,
        normal,
        boss,
        item,
        poi
    }
    
    [SerializeField] private MapManagerScriptableObject data;
    [SerializeField] private SaveDataScriptableObject saveData;
    [SerializeField] private EnemyListScriptableObject enemyList;

    
    private ERooms[,] map;
    public ERooms[,] Map
    {
        get { return map; }
        set { map = value; }
    }

    public bool[] SpawnEnemies
    {
        get { return saveData.spawnEntities; }
        set { saveData.spawnEntities = value; }
    }
    
    
    public DungeonBuilder DBuilder { get; set; }


    [SerializeField] private bool saveDebug = false;
    public bool SaveDebug
    {
        get{ return saveDebug; }
    }
    
    
    private void Awake()
    {
        DBuilder = new DungeonBuilder(this);
        dungenRoomCreator = GetComponent<DungenRoomCreator>();

        enemyList.Minion = new List<GameObject>();
        enemyList.Boss = new List<GameObject>();
        
    }


    private void Start()
    {
        if (saveData.loadGame)
        {
            LoadMap();
            GameLoaded = saveData.loadGame;
            PlayerPosition = saveData.playerPosAtLoad;
            dungenRoomCreator.GenerateRooms();
        }
        else
        {
            Map = DBuilder.GenerateDungeon(data.MapLevel, data.MapLevelMultiplier);
            SaveMap();
            dungenRoomCreator.GenerateRooms();
        }
    }

    

    public void RegisterEnemies(GameObject _enemy)
    {
        switch (_enemy.tag)
        {
            case "Minion":
            {
                if (!enemyList.Minion.Contains(_enemy))
                    {
                        enemyList.Minion.Add(_enemy);
                        enemyList.currentEnemiesCapacity += 1;
                    }
                    break;
            }
            case "Enemy":
            {
                if (!enemyList.Boss.Contains(_enemy))
                    {

                        enemyList.Boss.Add(_enemy);
                        enemyList.currentEnemiesCapacity += 1;
                    }
                    break;

            }
        }
        
    }


    #region  enum array to int arraytransformation for save and load functionality
    
    /// <summary>
    /// transforms int array in a 2d enum array for load
    /// </summary>
    private void LoadMap()
    {
        Map = new ERooms[MAP_WIDTH, MAP_HEIGTH];
        
        for (int y = 0; y < MAP_HEIGTH; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                
                switch (saveData.map[y * MAP_WIDTH + x])
                {
                    case 1:
                    {
                        Map[x, y] = ERooms.start;
                        break;
                    }
                    case 2:
                    {
                        Map[x, y] = ERooms.normal;
                        break;
                    }
                    case 3:
                    {
                        Map[x, y] = ERooms.boss;
                        break;
                    }
                    case 4:
                    {
                        Map[x, y] = ERooms.item;
                        break;
                    }
                    case 5:
                    {
                        Map[x, y] = ERooms.poi;
                        break;
                    }
                    default:
                    {
                        Map[x, y] = ERooms.free;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// transforms 2d Enum array in a int array for save
    /// </summary>
    private void SaveMap()
    {
        saveData.map = new int[MAP_WIDTH * MAP_HEIGTH];

        SpawnEnemies = new bool[MAP_WIDTH * MAP_HEIGTH];

        for (int y = 0; y < MAP_HEIGTH; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                switch (Map[x,y])
                {
                    case ERooms.start:
                    {
                        saveData.map[y * MAP_WIDTH + x] = 1;
                        break;
                    }
                    case ERooms.normal:
                    {
                        saveData.map[y * MAP_WIDTH + x] = 2;
                        break;
                    }
                    case ERooms.boss:
                    {
                        saveData.map[y * MAP_WIDTH + x] = 3;
                        break;
                    }
                    case ERooms.item:
                    {
                        saveData.map[y * MAP_WIDTH + x] = 4;
                        break;
                    }

                    case ERooms.poi:
                    {
                        saveData.map[y * MAP_WIDTH + x] = 5;
                        break;
                    }
                    default:
                    {
                        saveData.map[y * MAP_WIDTH + x] = 0;
                        break;
                    }
                }
                
                SpawnEnemies[y * MAP_WIDTH + x] = false;
            }
        }
    }
    
    #endregion
}


