using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Room startRoom;
    public Room[] roomPrefabs;
    public Room[] pathPrefabs;

    public Transform player;

    public float spawnDistance = 60f;
    public int maxNearbyRooms = 5;

    private List<Room> spawnedRooms = new List<Room>();

    void Start()
    {
        GenerateStartRoom();
    }

    void Update()
    {
        foreach (Room room in spawnedRooms)
        {
            float dist = Vector3.Distance(player.position, room.transform.position);

            if (dist < spawnDistance && !room.expanded)
            {
                ExpandRoom(room);
            }
        }
    }

    void GenerateStartRoom()
    {
        Room room = Instantiate(startRoom, Vector3.zero, Quaternion.identity);
        spawnedRooms.Add(room);
    }

    void ExpandRoom(Room room)
    {
        int nearby = CountNearbyRooms(room);

        if (nearby >= maxNearbyRooms)
            return;

        foreach (Door exitDoor in room.doors)
        {
            if (exitDoor.used)
                continue;

            TrySpawnPath(exitDoor);
        }

        room.expanded = true;
    }

    int CountNearbyRooms(Room room)
    {
        int count = 0;

        foreach (Room r in spawnedRooms)
        {
            float dist = Vector3.Distance(r.transform.position, room.transform.position);

            if (dist < 40)
                count++;
        }

        return count;
    }

    void TrySpawnPath(Door exitDoor)
    {
        Room pathPrefab = pathPrefabs[Random.Range(0, pathPrefabs.Length)];
        Room path = Instantiate(pathPrefab);

        Door pathEnter = null;

        foreach (Door d in path.doors)
        {
            if (IsOpposite(d.direction, exitDoor.direction))
            {
                pathEnter = d;
                break;
            }
        }

        if (pathEnter == null)
        {
            Destroy(path.gameObject);
            return;
        }

        Vector3 offset =
            exitDoor.transform.position -
            pathEnter.transform.position;

        path.transform.position += offset;

        exitDoor.used = true;
        pathEnter.used = true;

        SpawnRoomFromPath(path);
    }

    void SpawnRoomFromPath(Room path)
    {
        foreach (Door exitDoor in path.doors)
        {
            if (exitDoor.used)
                continue;

            Room prefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
            Room newRoom = Instantiate(prefab);

            Door enterDoor = null;

            foreach (Door d in newRoom.doors)
            {
                if (IsOpposite(d.direction, exitDoor.direction))
                {
                    enterDoor = d;
                    break;
                }
            }

            if (enterDoor == null)
            {
                Destroy(newRoom.gameObject);
                return;
            }

            Vector3 offset =
                exitDoor.transform.position -
                enterDoor.transform.position;

            newRoom.transform.position += offset;

            exitDoor.used = true;
            enterDoor.used = true;

            spawnedRooms.Add(newRoom);
            return;
        }
    }

    bool IsOpposite(DoorDirection a, DoorDirection b)
    {
        return
        (a == DoorDirection.Left && b == DoorDirection.Right) ||
        (a == DoorDirection.Right && b == DoorDirection.Left) ||
        (a == DoorDirection.Up && b == DoorDirection.Down) ||
        (a == DoorDirection.Down && b == DoorDirection.Up);
    }
}