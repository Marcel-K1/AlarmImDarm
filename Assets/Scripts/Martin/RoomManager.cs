/*****************************************************************************
* Project: Alarm Im Darm
* File   : RoomManager.cs
* Date   : 06.10.2022
* Author : Martin Stasch (MS)
*
* main script to manage the rooms which includes spawning and tracking room entities
*
* History:
*	20.10.2022	MS	Created
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Net.Http;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{

    #region Properties and variables

    public MapManager Manager { private get; set; }
    private Camera camera;

    [SerializeField] private Vector2Int roomID;
    public Vector2Int RoomID
    {
        get { return roomID; }
        set { roomID = value; }
    }
    
    [SerializeField] private MapManager.ERooms room;
    public MapManager.ERooms Room
    {
        get { return room; }
        set { room = value; }
    }

    [SerializeField] private Vector2Int currentPosition;
    [SerializeField] private List<GameObject> npcInRoom;
    [SerializeField] private List<GameObject> doors;
    private bool npcToSpawn = false;
    
    public List<GameObject> Doors
    {
        get { return doors; }
        set { doors = value; }
    }

    #endregion
    
    private void Awake()
    {
        RoomID = new Vector2Int();
        Room = MapManager.ERooms.free;
        camera = FindObjectOfType<Camera>();
        npcInRoom = new List<GameObject>();
        doors = new List<GameObject>();

        
    }

    #region room entity spawning for load and map generation
    
    /// <summary>
    /// logic for room entity spawn at map generation
    /// </summary>
    public void SpawnContentAtGeneration()
    {
        
        currentPosition = new Vector2Int((int)(RoomID.x * Manager.ROOM_SIZE_X * Manager.Scale.x),
            (int)(RoomID.y * Manager.ROOM_SIZE_Y * Manager.Scale.y));
        
        
        switch (room)
        {
            case MapManager.ERooms.normal:
            {
                npcToSpawn = true;
                Manager.SpawnEnemies[roomID.y * Manager.MAP_WIDTH + roomID.x] = true;
                int spawnCount = (int)(Random.Range(0, 2) + Manager.MinMinionNumber);
                EntitySpawner(Manager.EnemyMinionPrefab[Random.Range(0, Manager.EnemyMinionPrefab.Length)], currentPosition, spawnCount);
                npcToSpawn = false;
                break;
            }
            case MapManager.ERooms.boss:
            {
                npcToSpawn = true;
                EntitySpawner(Manager.EnemyBossPrefab[Random.Range(0,Manager.EnemyBossPrefab.Length)], currentPosition, 1);
                npcToSpawn = false;
                break;
            }
            case MapManager.ERooms.start:
            {
                EntitySpawner(Manager.PlayerPrefab, currentPosition, 1);
                break;
            }
            case MapManager.ERooms.poi:
            {
                var currentPOI = Manager.POIPrefab[Random.Range(0, Manager.POIPrefab.Length)];
                EntitySpawner(currentPOI, currentPosition, 1);
                break;
            }
            case MapManager.ERooms.item:
            {
                EntitySpawner(Manager.ItemPrefab[Random.Range(0, Manager.ItemPrefab.Length)] , currentPosition, 1);
                break;
            }
        }
        
    }
    
    /// <summary>
    /// logic for room entity spawn at game load
    /// </summary>
    public void SpawnContentAtGameLoad()
    {

        currentPosition = new Vector2Int((int)(RoomID.x * Manager.ROOM_SIZE_X * Manager.Scale.x),
            (int)(RoomID.y * Manager.ROOM_SIZE_Y * Manager.Scale.y));
        
        if (Manager.PlayerPosition == RoomID)
        {
            EntitySpawner(Manager.PlayerPrefab, currentPosition, 1);
        }
        
        switch (room)
        {
            case MapManager.ERooms.normal:
            {
                if (Manager.SpawnEnemies[roomID.y * Manager.MAP_WIDTH + roomID.x])
                {
                    npcToSpawn = true;
                    int spawnCount = (int)(Random.Range(0, 2) + Manager.MinMinionNumber);
                    EntitySpawner(Manager.EnemyMinionPrefab[Random.Range(0, Manager.EnemyMinionPrefab.Length)], currentPosition, spawnCount);
                    npcToSpawn = false;
                }
                break;
            }
            case MapManager.ERooms.boss:
            {
                
                npcToSpawn = true;
                EntitySpawner(Manager.EnemyBossPrefab[Random.Range(0,Manager.EnemyBossPrefab.Length)], currentPosition, 1);
                npcToSpawn = false;
                
                break;
            }
            case MapManager.ERooms.poi:
            {
                EntitySpawner(Manager.POIPrefab[Random.Range(0,Manager.POIPrefab.Length)], currentPosition, 1);
                break;
            }
            case MapManager.ERooms.item:
            {
                break;
            }
        }
    }
    
    /// <summary>
    /// Entity spawning methode. Spawns npc inside a room at a random place in the near of the room middle point.
    /// </summary>
    /// <param name="_spawnObject"></param>
    /// <param name="_spawnPosition"></param>
    /// <param name="_numbersToSpawn"></param>
    private void EntitySpawner(GameObject _spawnObject, Vector2Int _spawnPosition, int _numbersToSpawn)
    {
        int spawncount = 0;
        GameObject newSpawnObject = null ;

        while (spawncount < _numbersToSpawn)
        {
            newSpawnObject = Instantiate(_spawnObject, this.transform);

            int offsetX = (int)(Manager.ROOM_SIZE_X * Manager.Scale.x * 0.5f - 1);
            int offsetY = (int)(Manager.ROOM_SIZE_Y * Manager.Scale.y * 0.5f - 1);

            Vector3 offset = new Vector3(UnityEngine.Random.Range(-offsetX, offsetX + 1), 
                UnityEngine.Random.Range(-offsetY, offsetY + 1), 0f);

            if (newSpawnObject.tag == "POI")
            {
                Manager.PointsOfInterest.Add(newSpawnObject);
                newSpawnObject.GetComponent<PointOfInterest>().POITextEvent += Manager.FartalotCanvas.
                    GetComponent<FartalotManager>().OnPOITextEvent;
            }
            
            newSpawnObject.transform.position = new Vector3(_spawnPosition.x, _spawnPosition.y, 0) + offset;
            spawncount++;
            
            if (npcToSpawn)
            {
                npcInRoom.Add(newSpawnObject);
                Manager.RegisterEnemies(newSpawnObject);
                
                newSpawnObject.GetComponent<NPCKillNotifier>().RoomManager = this;
                
                newSpawnObject.gameObject.SetActive(false);

            }
            
        }
    }

    #endregion

    #region NPC tracking 

    /// <summary>
    /// Methode tracks killed NPC's in room and opens the doors if the room was cleared.
    /// This also updates the saveData scriptable object with a bool false tag for the room coordinate
    /// so the load/save system knows that in this room a spawn of enemies is no longer needed on load.
    ///
    /// IMPORTANT : Logical issue with load / save system in editor! NPC tracking is working with OnDesrtroy() event methode!
    /// Closing and restart game will cause this method to set all flags to false!
    /// save Debug bool needs to be FALSE for build!
    /// </summary>
    /// <param name="_enemy"></param>
    public void NPCkilled(GameObject _npc)
    {

        npcInRoom.Remove(_npc);

        if (npcInRoom.Count == 0)
        {
            for (int i = 0; i < doors.Count; i++)
            {
                doors[i].gameObject.SetActive(false);
                
                if(!Manager.SaveDebug)
                    Manager.SpawnEnemies[roomID.y * Manager.MAP_WIDTH + roomID.x] = false;

                Manager.PlayerPosition = RoomID;
            }
        }
        
    }

    #endregion
    
    #region PlayerMovement Tracking and CameraFokusChange

    
    /// <summary>
    /// Player tracking trigger methode to move the camera to the new room position and also activates npcs in the room and close doors.
    /// Also updates player position for save file
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        
        if (col.gameObject.tag == "Player")
        {
            camera.gameObject.transform.position = this.transform.position + new Vector3(0,0,-10f);
            
            if (npcInRoom.Count > 0)
            {
                for (int i = 0; i < npcInRoom.Count; i++)
                {
                    npcInRoom[i].gameObject.SetActive(true);
                    
                }

                for (int i = 0; i < doors.Count; i++)
                {
                    doors[i].gameObject.SetActive(true);
                }
                
            }
        }
        
        if(npcInRoom.Count == 0)
            Manager.PlayerPosition = RoomID;
        
    }

    
    #endregion
    
}
