using System.Collections.Generic;
using UnityEngine;

public class EndlessMap : MonoBehaviour
{
    public GameObject cubePrefab;
    public Transform player;

    public float cubeSize = 4f;

    public int spawnAhead = 15;
    public int spawnBehind = 10;

    public int maxPlatformHeight = 2;

    public float platformChance = 0.5f;

    public int safeZone = 2;

    private Dictionary<int, List<GameObject>> columns =
        new Dictionary<int, List<GameObject>>();

    void Start()
    {
        int playerColumn = Mathf.RoundToInt(player.position.x / cubeSize);

        for (int x = playerColumn - spawnBehind; x <= playerColumn + spawnAhead; x++)
        {
            GenerateColumn(x);
        }
    }

    void Update()
    {
        int playerColumn = Mathf.RoundToInt(player.position.x / cubeSize);

        int leftBound = playerColumn - spawnBehind;
        int rightBound = playerColumn + spawnAhead;

        for (int x = leftBound; x <= rightBound; x++)
        {
            if (!columns.ContainsKey(x))
            {
                GenerateColumn(x);
            }
        }
    }

    void GenerateColumn(int x)
    {
        if (columns.ContainsKey(x))
            return;

        List<GameObject> column = new List<GameObject>();

        // ground
        Vector3 groundPos = new Vector3(
            x * cubeSize,
            0,
            0
        );

        GameObject ground = Instantiate(
            cubePrefab,
            groundPos,
            Quaternion.identity
        );

        column.Add(ground);

        // safe zone
        if (Mathf.Abs(x) <= safeZone)
        {
            columns.Add(x, column);
            return;
        }

        // platform above
        for (int y = 1; y <= maxPlatformHeight; y++)
        {
            if (Random.value < platformChance)
            {
                Vector3 pos = new Vector3(
                    x * cubeSize,
                    y * cubeSize,
                    0
                );

                GameObject cube = Instantiate(
                    cubePrefab,
                    pos,
                    Quaternion.identity
                );

                column.Add(cube);
            }
        }

        columns.Add(x, column);
    }
}