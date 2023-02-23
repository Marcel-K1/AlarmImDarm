/*****************************************************************************
* Project: Alarm Im Darm
* File   : DungenRoomCreator.cs
* Date   : 06.10.2022
* Author : Martin Stasch (MS)
*
* Dungeon room creation methode for generation the dungeon according to the procedurally generated map
*
* History:
*	06.10.2022	MS	Created
******************************************************************************/



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DungenRoomCreator : MonoBehaviour

{
    
    private MapManager manager;

    [Header("Room prefabs")]
    [SerializeField] private GameObject[] backGround_Prefab;
    [SerializeField] private GameObject[] wallSide_Prefab;
    [SerializeField] private GameObject[] wallSideDoor_Prefab;
    [SerializeField] private GameObject[] wallBottom_Prefab;
    [SerializeField] private GameObject[] wallBottomDoor_Prefab;
    [SerializeField] private GameObject[] doorsH_Prefab;
    [SerializeField] private GameObject[] doorsV_Prefab;
    [SerializeField] private GameObject[] obstacle_Prefab;

    [Header("Obstacle distribution settings")] 
    [SerializeField, Range(0, 10)] private int maxStartRoom = 3;
    [SerializeField, Range(0, 10)] private int maxNormalRoom = 3;
    [SerializeField, Range(0, 10)] private int maxItemRoom = 3;
    [SerializeField, Range(0, 10)] private int maxPOIRoom = 3;
    [SerializeField, Range(0, 10)] private int maxBoosRoom = 3;
    
    
    private GameObject emptyObject;
    RoomManager roomManager;
    private bool isRoomManager = false;

    private int wallSets = 0;
    private int doorSets = 0;

    private void Awake()
    {
        manager = GetComponent<MapManager>();
        
        CheckDoorSetsCound();
        CheckWallSetsCount();
    }
    
    /// <summary>
    /// main dungeon rooms  generation method to create background, walls, doors and obstacles
    /// </summary>
    public void GenerateRooms()
    {
        for (int y = 0; y < manager.MAP_HEIGTH; y++)
        {
            for (int x = 0; x < manager.MAP_WIDTH; x++)
            {

                // check if there is a room to generate or not
                if (manager.Map[x, y] == MapManager.ERooms.free)
                    continue;

                Vector2Int currentPosition = new Vector2Int(x, y);
                
                int neighbourCount = 0;
                bool[] isNeighbourARoom =
                {
                    false,
                    false,
                    false,
                    false
                };

                Vector2Int[] positionsToCheck =
                {
                    currentPosition + Vector2Int.up,
                    currentPosition + Vector2Int.down,
                    currentPosition + Vector2Int.left,
                    currentPosition + Vector2Int.right
                };

                
                // double for loop to check for neighbours of the room
                for (int i = 0; i < positionsToCheck.Length; i++)
                {
                    Vector2Int toCheck = positionsToCheck[i];
                    
                    // check if in bound
                    if (toCheck.x >= 0 && toCheck.y >= 0 && toCheck.x < manager.MAP_WIDTH && toCheck.y < manager.MAP_HEIGTH)
                    {
                        if (manager.Map[toCheck.x, toCheck.y] != MapManager.ERooms.free)
                        {
                            isNeighbourARoom[i] = true;
                            neighbourCount++;
                        }
                    }
                }

                
                // creates first the background of the room which also serves as the room manager for the created room.
                // The room entities are also attached to en empty object with the id of the room - every entities that is part of the room
                // or is spawned by the room manager is a child of this empty object.
                emptyObject = new GameObject($"Room {x}-{y}");
                emptyObject.transform.parent = this.gameObject.transform;
                emptyObject.transform.localPosition = new Vector3(currentPosition.x * manager.ROOM_SIZE_X,
                    currentPosition.y * manager.ROOM_SIZE_Y, 0f);

                isRoomManager = true;
                SpawnRoomItem(backGround_Prefab[Random.Range(0,backGround_Prefab.Length)], currentPosition, 0);
                isRoomManager = false;


                #region Generate room obstacles

                // switch to generate obstacles for rooms dependent of the room 
                
                switch (manager.Map[x,y])
                {
                    case MapManager.ERooms.boss:
                    {
                        if(maxBoosRoom != 0)
                            DefaultObstacleSetup(maxBoosRoom, currentPosition);
                        
                        break;
                    }
                    case MapManager.ERooms.normal:
                    {
                        if(maxNormalRoom != 0)
                            DefaultObstacleSetup(maxNormalRoom, currentPosition);
                        
                        break;
                    }
                    case MapManager.ERooms.item:
                    {
                        if(maxItemRoom != 0)
                            DefaultObstacleSetup(maxItemRoom, currentPosition);
                        
                        break;
                    }
                    case MapManager.ERooms.poi:
                    {
                        if(maxPOIRoom != 0)
                            DefaultObstacleSetup(maxPOIRoom, currentPosition);
                        
                        break;
                    }
                    case MapManager.ERooms.start:
                    {
                        if(maxStartRoom != 0)
                            DefaultObstacleSetup(maxStartRoom, currentPosition);
                        
                        break;
                    }
                
                }
                
                #endregion
                
                
                #region Switch for room walls generation

                // generates walls for every created room and rotates the prefab correctly so doors are connected to each other

                int idxWall = Random.Range(0, wallSets);
                int idxDoor = Random.Range(0, doorSets);
                
                switch (neighbourCount)
                {
                    case 1:
                        {
                            if (isNeighbourARoom[0])
                            {
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,180);
                                
                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 180);
                                
                            }
                            else if (isNeighbourARoom[1])
                            {
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 180);
                                
                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 180);

                            }
                            else if (isNeighbourARoom[2])
                            {
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 180);
                                
                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 180);
                            }
                            else
                            {
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 180);
                                
                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,180);
                            }
                            
                            break;

                        }
                    case 2:
                        {

                            if (isNeighbourARoom[0] && isNeighbourARoom[1])
                            {
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,180);

                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 180);
                            }
                            else if (isNeighbourARoom[0] && isNeighbourARoom[2])
                            {
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,180);

                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 180);
                            }
                            else if (isNeighbourARoom[0] && isNeighbourARoom[3])
                            {
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,180);

                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,180);
                            }
                            else if (isNeighbourARoom[1] && isNeighbourARoom[2])
                            {
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 180);
                                
                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 180);
                            }
                            else if (isNeighbourARoom[1] && isNeighbourARoom[3])
                            {
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 180);
                                
                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,180);
                            }
                            else
                            {
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 180);
                                
                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,180);
                            }
                            
                            break;
                        }
                    case 3:
                        {
                            if (!isNeighbourARoom[0])
                            {
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 180);
                                
                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,180);
                            }
                            else if (!isNeighbourARoom[1])
                            {
                                SpawnRoomItem(wallBottom_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,180);

                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,180);
                            }
                            else if (!isNeighbourARoom[2])
                            {
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,180);

                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 0);
                                
                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,180);
                            }
                            else
                            {
                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 180);
                                SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,180);

                                SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 0);
                                SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,0);

                                SpawnRoomItem(wallSide_Prefab[idxWall], currentPosition, 180);
                            }
                            
                            break;
                        }
                    case 4:
                        {
                            SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 0);
                            SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,0);

                            SpawnRoomItem(wallBottomDoor_Prefab[idxWall], currentPosition, 180);
                            SpawnRoomItem(doorsH_Prefab[idxDoor],currentPosition,180);

                            SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 0);
                            SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,0);

                            SpawnRoomItem(wallSideDoor_Prefab[idxWall], currentPosition, 180);
                            SpawnRoomItem(doorsV_Prefab[idxDoor],currentPosition,180);
                            break;
                        }
                }
                
                #endregion
            }
        }
    }


    #region spawns walls and doors for the room

    
    /// <summary>
    /// spawn methode to generate the room items (walls and background)
    /// also has some additional code for the room manager
    /// </summary>
    /// <param name="_roomToSpawn"></param>
    /// <param name="_spawnPosition"></param>
    /// <param name="_rotaion"></param>
    private void SpawnRoomItem(GameObject _roomToSpawn, Vector2Int _spawnPosition, float _rotaion)
    {
        GameObject newSpawnItem = null ;
        
        newSpawnItem = Instantiate(_roomToSpawn, this.transform);
        newSpawnItem.transform.localPosition = new Vector3(_spawnPosition.x * manager.ROOM_SIZE_X, 
            _spawnPosition.y * manager.ROOM_SIZE_Y, 0); 
        newSpawnItem.transform.rotation = Quaternion.Euler(0,0,_rotaion);

        newSpawnItem.transform.parent = emptyObject.transform;

        
        if (isRoomManager)
        {
            roomManager = newSpawnItem.GetComponent<RoomManager>();
            
            roomManager.RoomID = _spawnPosition;
            roomManager.Room = manager.Map[_spawnPosition.x, _spawnPosition.y];
            roomManager.Manager = manager;

            if (manager.GameLoaded)
            {
                roomManager.SpawnContentAtGameLoad();
            }
            else
            {
                roomManager.SpawnContentAtGeneration();
            }
        }

        if (newSpawnItem.gameObject.tag == "Door")
        {
            roomManager.Doors.Add(newSpawnItem);
            newSpawnItem.gameObject.SetActive(false);
        }
        
    }

    #endregion

    #region Room obstical distribution methodes


    /// <summary>
    /// devides the room into 4 quadrants and generates random positions for obstacles to spawn
    /// </summary>
    /// <param name="_maxnumber"></param>
    /// <param name="_currentPosition"></param>
    private void DefaultObstacleSetup(int _maxnumber, Vector2Int _currentPosition)
    {
        Vector2Int spawn = new Vector2Int(
            (int)(_currentPosition.x * manager.ROOM_SIZE_X * manager.Scale.x), 
            (int)(_currentPosition.y * manager.ROOM_SIZE_Y * manager.Scale.y));
        
        // creates 2 offsets values that have 1/4 of the room length - 1 to ensure that the obstacle is not spawned at the edge
        int offsetX = (int)(manager.ROOM_SIZE_X * manager.Scale.x * 0.25f - 1f);
        int offsetY = (int)(manager.ROOM_SIZE_Y * manager.Scale.y * 0.25f - 1f);

        
        
        Vector2Int offsetUL = new Vector2Int((int)(spawn.x - offsetX), (int)(spawn.y + offsetY));
        SpawnRoomObstecals(offsetUL, Random.Range(0,_maxnumber));
                        
        Vector2Int offsetUR = new Vector2Int((int)(spawn.x + offsetX), (int)(spawn.y + offsetY));
        SpawnRoomObstecals(offsetUR, Random.Range(0,_maxnumber));
                        
        Vector2Int offsetDL = new Vector2Int((int)(spawn.x - offsetX), (int)(spawn.y - offsetY));
        SpawnRoomObstecals(offsetDL, Random.Range(0,_maxnumber));

        Vector2Int offsetDR = new Vector2Int((int)(spawn.x + offsetX), (int)(spawn.y - offsetY));
        SpawnRoomObstecals(offsetDR, Random.Range(0,_maxnumber));
    }

    /// <summary>
    /// Methode to spawn room obstacles. 
    /// </summary>
    /// <param name="_spawnPosition"></param>
    /// <param name="_obsticlesToSpawn"></param>
    private void SpawnRoomObstecals(Vector2Int _spawnPosition, int _obsticlesToSpawn)
    {
        GameObject newSpawnItem = null ;

        int count = 0;

        while (count < _obsticlesToSpawn)
        {   
            
            newSpawnItem = Instantiate(obstacle_Prefab[Random.Range(0,obstacle_Prefab.Length)], this.transform);
            
            int offsetX = (int)(manager.ROOM_SIZE_X * manager.Scale.x * 0.25f - 2); 
            int offsetY = (int)(manager.ROOM_SIZE_Y * manager.Scale.y * 0.25f - 2);

            Vector3 offset = new Vector3(UnityEngine.Random.Range(-offsetX, offsetX + 1), 
                UnityEngine.Random.Range(-offsetY, offsetY + 1), 0f);

            newSpawnItem.transform.position = new Vector3(_spawnPosition.x, _spawnPosition.y, 0) + offset;

            // may not be needed to rotate the obstacle 
            // newSpawnItem.transform.rotation = Quaternion.Euler(0,0,Random.Range(0f,360f)); 
            
            newSpawnItem.transform.parent = emptyObject.transform;

            count++;
        }

    }
    
    #endregion

    private void CheckWallSetsCount()
    {
        
        int i = Int32.MaxValue;

        if (wallBottom_Prefab.Length < i)
            i = wallBottom_Prefab.Length;

        if (wallSide_Prefab.Length < i)
            i = wallSide_Prefab.Length;

        if (wallBottomDoor_Prefab.Length < i)
            i = wallBottomDoor_Prefab.Length;

        if (wallSideDoor_Prefab.Length < i)
            i = wallSideDoor_Prefab.Length;

        wallSets = i;

    }

    private void CheckDoorSetsCound()
    {
        int i = Int32.MaxValue;

        if (doorsH_Prefab.Length < i)
            i = doorsH_Prefab.Length;

        if (doorsV_Prefab.Length < i)
            i = doorsV_Prefab.Length;
        
        doorSets = i;
    }

}
