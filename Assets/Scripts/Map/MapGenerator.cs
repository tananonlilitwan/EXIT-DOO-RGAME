using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    [Header("Map size")]
    [SerializeField] public int mapWidth;
    [SerializeField] public int mapHeight;

    [Header("Prefab Object In Map")]
    public GameObject wallPrefab;
    public GameObject grassPrefab;
    public GameObject doorPrefab;
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    
    [Header("ObjNotes")]
    public GameObject[] notePrefabs; 
    [SerializeField] public int numberOfNotes;
    
    [Header("Obj Tree")]
    public GameObject[] treePrefabs;
    [SerializeField] public int numberOfTrees;
    
    [Header("Obj Flower")]
    public GameObject flowerPrefab;
    
    [Header("Empty Obj saves")]
    public Transform mapParent;

    [SerializeField] private Vector3 mapOffsetPosition = Vector3.zero;

    private GameObject[,] mapGrid;
    private Vector2 playerPosition;
    private float minDistanceBetweenPlayerAndEnemy = 5f;
    [SerializeField] private float minNoteDistance = 3f;
    private List<Vector2> placedNotePositions = new List<Vector2>();

    public int currentLevel = 1;
    private const int maxLevel = 5;
    
    private GameObject playerInstance;
    
    [Header("Flashlight")]
    public GameObject flashlightPrefab;
    private GameObject flashlightInstance;
    private bool flashlightCollected = false;
    public static MapGenerator Instance { get; private set; }

    private bool gameEnded = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        SetupLevel();
    }
    
    public void GenerateMap()
    {
        mapGrid = new GameObject[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, mapParent);
                    wall.tag = "Wall";
                    mapGrid[x, y] = wall;
                }
            }
        }

        PlaceRandomDoor();
        PlaceRandomPlayer();
    }

    void PlaceRandomDoor()
    {
        int side = UnityEngine.Random.Range(0, 4);
        int x = 0, y = 0;

        switch (side)
        {
            case 0: x = UnityEngine.Random.Range(1, mapWidth - 1); y = mapHeight - 1; break;
            case 1: x = UnityEngine.Random.Range(1, mapWidth - 1); y = 0; break;
            case 2: x = 0; y = UnityEngine.Random.Range(1, mapHeight - 1); break;
            case 3: x = mapWidth - 1; y = UnityEngine.Random.Range(1, mapHeight - 1); break;
        }

        Destroy(mapGrid[x, y]);
        GameObject door = Instantiate(doorPrefab, new Vector3(x, y, 0) + mapOffsetPosition, Quaternion.identity, mapParent);
        door.tag = "Door";
        mapGrid[x, y] = door;
    }

    void PlaceRandomEnemy()
    {
        int tries = 100;
        while (tries-- > 0)
        {
            int x = UnityEngine.Random.Range(1, mapWidth - 1);
            int y = UnityEngine.Random.Range(1, mapHeight - 1);

            if (IsCellEmpty(x, y))
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, playerPosition);

                if (distance >= minDistanceBetweenPlayerAndEnemy)
                {
                    GameObject enemy = Instantiate(enemyPrefab, new Vector3(x, y, 0) + mapOffsetPosition, Quaternion.identity, mapParent);
                    enemy.tag = "Enemy";
                    mapGrid[x, y] = enemy;
                    break;
                }
            }
        }
    }
    
    void PlaceRandomPlayer()
    {
        int tries = 100;
        while (tries-- > 0)
        {
            int x = UnityEngine.Random.Range(1, mapWidth - 1); 
            int y = UnityEngine.Random.Range(1, mapHeight - 1);

            if (IsCellEmpty(x, y) && IsFarEnoughFromEnemies(x, y))
            {
                Vector3 spawnPos = new Vector3(x, y, 0);
                playerPosition = new Vector2(x, y);
                
                if (playerInstance == null)
                {
                    playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                    playerInstance.tag = "Player";
                }
                else
                {
                    playerInstance.transform.position = spawnPos;
                }

                mapGrid[x, y] = playerInstance;
                break;
            }
        }
    }
    
    void PlaceGrassTiles()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 pos = new Vector3(x, y, 1) + mapOffsetPosition;
                GameObject grass = Instantiate(grassPrefab, pos, Quaternion.identity, mapParent);
                grass.tag = "Ground";
            }
        }
    }

    void PlaceRandomNotes()
    {
        int placedNotes = 0;
        int tries = 500;

        while (placedNotes < numberOfNotes && tries-- > 0)
        {
            int x = UnityEngine.Random.Range(3, mapWidth - 3);
            int y = UnityEngine.Random.Range(3, mapHeight - 3);
            Vector2 newPos = new Vector2(x, y);

            if (IsCellEmpty(x, y) && IsFarFromOtherNotes(newPos))
            {
                int randomNoteIndex = UnityEngine.Random.Range(0, notePrefabs.Length);
                GameObject note = Instantiate(notePrefabs[randomNoteIndex], new Vector3(x, y, 0) + mapOffsetPosition, Quaternion.identity, mapParent);
                note.tag = "Note";
                mapGrid[x, y] = note;
                placedNotePositions.Add(newPos);
                placedNotes++;
            }
        }
    }
    
    void PlaceRandomTrees()
    {
        int placedTrees = 0;
        int tries = 500;

        while (placedTrees < numberOfTrees && tries-- > 0)
        {
            int x = Random.Range(1, mapWidth - 1);
            int y = Random.Range(1, mapHeight - 1);

            if (IsCellEmpty(x, y))
            {
                int randomTreeIndex = Random.Range(0, treePrefabs.Length);
                GameObject tree = Instantiate(treePrefabs[randomTreeIndex], new Vector3(x, y, 0), Quaternion.identity, mapParent);
                tree.tag = "Tree";
                mapGrid[x, y] = tree;
                placedTrees++;
            }
        }
    }
    void PlaceRandomFlowers(int amount)
    {
        int placedFlowers = 0;
        int tries = 500;

        while (placedFlowers < amount && tries-- > 0)
        {
            int x = Random.Range(1, mapWidth - 1);
            int y = Random.Range(1, mapHeight - 1);

            if (IsCellEmpty(x, y))
            {
                GameObject flower = Instantiate(flowerPrefab, new Vector3(x, y, 0), Quaternion.identity, mapParent);
                flower.tag = "ItemFlower";
                mapGrid[x, y] = flower;
                placedFlowers++;
            }
        }
    }
    bool IsCellEmpty(int x, int y)
    {
        if (mapGrid[x, y] != null)
        {
            string tag = mapGrid[x, y].tag;
            return tag != "Wall" && tag != "Door" && tag != "Enemy" && tag != "Player" && tag != "Note" && tag != "Tree" && tag != "Flashlight" && tag != "ItemFlower";
        }
        return true;
    }
    bool IsFarEnoughFromEnemies(int x, int y)
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            float distance = Vector2.Distance(new Vector2(x, y), (Vector2)enemy.transform.position);
            if (distance < minDistanceBetweenPlayerAndEnemy)
            {
                return false;
            }
        }
        return true;
    }
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel > maxLevel)
        {
            GameManager.Instance.ShowWinPanel();
            return;
        }

        ClearOldMap();
        SetupLevel();
    }
    bool IsFarFromOtherNotes(Vector2 position)
    {
        foreach (Vector2 notePos in placedNotePositions)
        {
            if (Vector2.Distance(position, notePos) < minNoteDistance)
                return false;
        }
        return true;
    }
    void ClearOldMap()
    {
        foreach (Transform child in mapParent)
        {
            string tag = child.tag;
            
            if (tag == "Wall" || tag == "Ground" || tag == "Enemy" || tag == "Door" || tag == "Tree" || tag == "ItemFlower")
            {
                Destroy(child.gameObject);
            }
        }
    }
    void SetupLevel()
    {
        if (currentLevel == 1)
        {
            GenerateMap();
            PlaceGrassTiles();
            PlaceRandomPlayer();
            PlaceFlashlight();
        }
        else
        {
            mapWidth = 15 + currentLevel * 5;
            mapHeight = 10 + currentLevel * 5;
            mapGrid = new GameObject[mapWidth, mapHeight];
            GenerateMap();
            PlaceGrassTiles();
            for (int i = 0; i < currentLevel - 1; i++) PlaceRandomEnemy();
            numberOfNotes = currentLevel - 1;
            PlaceRandomNotes();
            PlaceRandomTrees();

            int flowerCount = 0;
            switch (currentLevel)
            {
                case 2: flowerCount = 5; break; // MapLevel 2
                case 3: flowerCount = 8; break; // MapLevel 3
                case 4: flowerCount = 10; break; // MapLevel 4
                case 5: flowerCount = 12; break; // MapLevel 5
            }
            PlaceRandomFlowers(flowerCount);
            /*if (FlowerManager.Instance != null)
            {
                FlowerManager.Instance.SetFlowerCount(flowerCount);
            }*/
            
            // ✅ เพิ่มตรงนี้เพื่อรีเซต UI ตอนเริ่มด่านใหม่
            GameManager.Instance.UpdateItemFlowerUI(0, flowerCount, currentLevel);
            
            PlaceRandomPlayer(); 
            
            if (!flashlightCollected && flashlightInstance != null)
            {
                flashlightInstance.transform.SetParent(mapParent);
                PlaceFlashlight();
            }
        }
        
        StartCoroutine(WaitUntilPlayerExists());
    }
    IEnumerator WaitUntilPlayerExists()
    {
        GameObject player = null;

        while ((player = GameObject.FindGameObjectWithTag("Player")) == null)
        {
            yield return null;
        }
    }
    void PlaceFlashlight()
    {
        if (flashlightCollected)
        {
            return;
        }

        int tries = 100;
        while (tries-- > 0)
        {
            int x = Random.Range(1, mapWidth - 1);
            int y = Random.Range(1, mapHeight - 1);

            if (IsCellEmpty(x, y))
            {
                if (flashlightInstance == null)
                {
                    flashlightInstance = Instantiate(flashlightPrefab, new Vector3(x, y, 0), Quaternion.identity, mapParent);
                    flashlightInstance.tag = "Flashlight";
                }
                else
                {
                    flashlightInstance.transform.position = new Vector3(x, y, 0);
                }

                mapGrid[x, y] = flashlightInstance;
                break;
            }
        }
    }
    public void FlashlightCollected()
    {
        flashlightCollected = true;
    }
    public void RespawnEnemy()
    {
        int tries = 100;

        while (tries-- > 0)
        {
            int x = UnityEngine.Random.Range(1, mapWidth - 1);
            int y = UnityEngine.Random.Range(1, mapHeight - 1);

            if (IsCellEmpty(x, y))
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, playerPosition);

                if (distance >= minDistanceBetweenPlayerAndEnemy)
                {
                    GameObject enemy = Instantiate(enemyPrefab, new Vector3(x, y, 0) + mapOffsetPosition, Quaternion.identity, mapParent);
                    enemy.tag = "Enemy";
                    mapGrid[x, y] = enemy;
                    break;
                }
            }
        }
    }
    public void ClearCell(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < mapWidth && pos.y >= 0 && pos.y < mapHeight)
        {
            mapGrid[pos.x, pos.y] = null;
        }
    }
    public GameObject GetObjectAt(int x, int y)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            return mapGrid[x, y];
        }
        return null;
    }
    public void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
}
