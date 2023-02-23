/*****************************************************************************
* Project: Alarm Im Darm
* File   : DungeonBuilder.cs
* Date   : 06.10.2022
* Author : Martin Stasch (MS)
*
* ^Creates a map candidate for the dungeon
*
* History:
*	06.10.2022	MS	Created
******************************************************************************/


using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder
{

    public MapManager Manager { get; set; }
    
    private List<Vector2Int> endRooms;

    private Vector3 startPos;
    

    private MapManager.ERooms[,] map;
    
    /* Rules for dungeon generation
     *
     * 1. are already enough rooms generates -> Skip
     * 2. is the room already occupied -> Skip
     * 3. 50% chance for room creation -> Skip if failed
     * 4. has the room more then 1 neighbour -> Skip
     *
     * 5. if not skipped generate a room
     * 
     * if a room is generated add this to a queue
     * 
     * Define number of rooms in relation to the dungeon level :
     * random(2) -> 0 or 1 rooms
     * random(2) + 5 + Level * mapLevelMultiplier
     */

    public DungeonBuilder(MapManager _manager)
    {
        Manager = _manager;
    }
    

    #region Generate the Dungeon
    
    /// <summary>
    /// Main map generation method for procedural generation of layout
    /// </summary>
    /// <param name="_mapLevel"></param>
    /// <param name="_mapLevelMultiplier"></param>
    public MapManager.ERooms[,] GenerateDungeon(int _mapLevel, float _mapLevelMultiplier)
    {
        int numberOfRooms = 0;
        numberOfRooms = (int)(Random.Range(0, 2) + 5 + _mapLevel * _mapLevelMultiplier);

        bool validateMap = false;
        
        // Variables to ensure that the while loop does not run infinitely
        int iterationCount = 0;
        int maxIterations = 5000;


        while (!validateMap && iterationCount < maxIterations)
        {
            GenerateRandomLevel(numberOfRooms);
            
            validateMap = ValidateMap(numberOfRooms);
            
            iterationCount++;
        }

        GenerateSpecialRooms();
        
        return map;
    }
    

    #endregion
    
    #region Generate map candidate

    /// <summary>
    /// Generates an empty map and fills it with rooms according to the set of rules
    /// </summary>
    /// <param name="_numberOfRooms"></param>
    private void GenerateRandomLevel(int _numberOfRooms)
    {
        map = new MapManager.ERooms[Manager.MAP_WIDTH, Manager.MAP_HEIGTH];

        for (int y = 0; y < Manager.MAP_HEIGTH; y++)
        {
            for (int x = 0; x < Manager.MAP_WIDTH; x++)
            {
                map[x, y] = MapManager.ERooms.free; // 0 for a free slot
            }
        }

        endRooms = new List<Vector2Int>();

        Queue<Vector2Int> positionsForExpansion = new Queue<Vector2Int>();

        Vector2Int startRoomIdx = new Vector2Int(Manager.MAP_WIDTH / 2, Manager.MAP_HEIGTH / 2); //sets position of start room in the middle of the map area 

        map[startRoomIdx.x, startRoomIdx.y] = MapManager.ERooms.start; //sets the start room as a start room
        
        positionsForExpansion.Enqueue(startRoomIdx);

        int currentRooms = 1;


        while (positionsForExpansion.Count > 0)
        {

            Vector2Int currentPosition = positionsForExpansion.Dequeue();
            
            // is used to check the neighbours of the position
            Vector2Int[] positionsToCheck = new Vector2Int[]
            {
                currentPosition + Vector2Int.up,
                currentPosition + Vector2Int.down,
                currentPosition + Vector2Int.left,
                currentPosition + Vector2Int.right
            };

            bool addedARoom = false;


            for (int i = 0; i < positionsToCheck.Length; i++)
            {
                Vector2Int toCheck = positionsToCheck[i];
                
                // Check if not out of boundaries
                if (toCheck.x >= 0 && toCheck.y >= 0 && toCheck.x < Manager.MAP_WIDTH && toCheck.y < Manager.MAP_HEIGTH)
                {
                    
                    // checks for room creation rules
                    if(currentRooms >= _numberOfRooms)
                        continue;
                    
                    if(map[toCheck.x, toCheck.y] != MapManager.ERooms.free) // 0 is free
                        continue;

                    float rngPercent = Random.Range(0, 2);
                    if(rngPercent == 0)
                        continue;

                    int neighbourCount = GetNeighbourCount(toCheck);
                    if(neighbourCount > 1)
                        continue;

                    map[toCheck.x, toCheck.y] = MapManager.ERooms.normal; // 1 is normal room
                    positionsForExpansion.Enqueue(toCheck);

                    currentRooms++;
                    addedARoom = true;
                }
            }
            
            if (!addedARoom)
                endRooms.Add(currentPosition);
        }
    }

    
    /// <summary>
    /// method checks direct neighbour positions and returns the number of used rooms
    /// </summary>
    /// <param name="_toCheck">position to check for neighbours</param>
    /// <returns>the number of not free neighbour rooms at the position that is checked</returns>
    private int GetNeighbourCount(Vector2Int _toCheck)
    {
        int count = 0;
        
        Vector2Int[] positionsToCheck = new Vector2Int[]
        {
            _toCheck + Vector2Int.up,
            _toCheck + Vector2Int.down,
            _toCheck + Vector2Int.left,
            _toCheck + Vector2Int.right
        };

        for (int i = 0; i < positionsToCheck.Length; i++)
        {
            Vector2Int currentNeighboursPosition = positionsToCheck[i];

            // Check if not out of boundaries
            if (currentNeighboursPosition.x >= 0 && currentNeighboursPosition.y >= 0 && 
                currentNeighboursPosition.x < Manager.MAP_WIDTH && currentNeighboursPosition.y < Manager.MAP_HEIGTH)
            {
                if (map[currentNeighboursPosition.x, currentNeighboursPosition.y] != 0)
                    count++;
            }
        }

        return count;
    }
    
    #endregion

    #region Map validation check

    /// <summary>
    /// Method counts the number of generated rooms on the map and checks if the number is equal the number of required rooms
    /// </summary>
    /// <param name="numberOfRooms">number of required rooms</param>
    /// <returns>return true if room count is equal to required rooms</returns>
    private bool ValidateMap(int _numberOfRooms)
    {
        int roomCount = 0;

        for (int y = 0; y < Manager.MAP_HEIGTH; y++)
        {
            for (int x = 0; x < Manager.MAP_WIDTH; x++)
            {
                if (map[x, y] != MapManager.ERooms.free)
                    roomCount++;
            }
        }

        if (roomCount == _numberOfRooms && endRooms.Count > 2)
            return true;
        
        return false;
        
    }

    #endregion

    #region Generate special rooms

    /// <summary>
    /// Methode to create special rooms within the dungeon 
    /// </summary>
    private void GenerateSpecialRooms()
    {
        //Boss Room
        Vector2Int bossRoomIdx = endRooms[endRooms.Count - 1];
        endRooms.RemoveAt(endRooms.Count - 1);

        map[bossRoomIdx.x, bossRoomIdx.y] = MapManager.ERooms.boss; // for boos room
        
        //Item Room
        int rndEndRoomIdx = Random.Range(0, endRooms.Count);
        Vector2Int itemRoomIdx = endRooms[rndEndRoomIdx];
        endRooms.RemoveAt(rndEndRoomIdx);

        map[itemRoomIdx.x, itemRoomIdx.y] = MapManager.ERooms.item; // for item room

        if (endRooms.Count > 0)
        {
            rndEndRoomIdx = Random.Range(0, endRooms.Count);
            Vector2Int shopRoomIdx = endRooms[rndEndRoomIdx];
            endRooms.RemoveAt(rndEndRoomIdx);

            map[shopRoomIdx.x, shopRoomIdx.y] = MapManager.ERooms.poi; // for Point of Interest room

        }

    }

    #endregion
}
