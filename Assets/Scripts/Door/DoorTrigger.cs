using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private bool hasWon = false;
    private Collider2D doorCollider;

    private void Start()
    {
        doorCollider = GetComponent<Collider2D>();

        int level = MapGenerator.Instance.GetCurrentLevel();
        /*if (level >= 2 && level == 5)
        {
            doorCollider.isTrigger = false;
        }*/
        
        // ✅ ปิดการผ่านประตูถ้าอยู่ใน Level ที่มีเงื่อนไข
        if (level >= 2 && level <= 5)
        {
            doorCollider.isTrigger = false;
            Debug.Log($"🔒 เริ่มเกมที่ Level {level} → ล็อคประตูก่อน");
        }
        else
        {
            doorCollider.isTrigger = true; // Level 1 ผ่านได้เลย
        }
    }

    /*private void Update()
    {
        int level = MapGenerator.Instance.GetCurrentLevel();
        if (level >= 2 && level <= 5 && !doorCollider.isTrigger)
        {
            if (FlowerManager.Instance != null && FlowerManager.Instance.HasCollectedAllFlowers())
            {
                doorCollider.isTrigger = true;
                Debug.Log("Door ปลดล็อคแล้ว!");
            }
        }
    }*/
    private void Update()
    {
        int level = MapGenerator.Instance.GetCurrentLevel();

        if (level >= 2 && level <= 5 && !doorCollider.isTrigger)
        {
            int collected = GameManager.Instance.GetCollectedFlowerCount();
            int required = GetRequiredFlowerCount(level);

            Debug.Log($"🟡 Level {level} | เก็บแล้ว: {collected} / ต้องการ: {required}");
            
            if (collected >= required)
            {
                doorCollider.isTrigger = true;
                Debug.Log($"✅ เก็บดอกไม้ครบ {collected}/{required} → ปลดล็อคประตูแล้ว");
            }
        }
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasWon && other.CompareTag("Player"))
        {
            hasWon = true;
            Debug.Log("Player ชน Door แล้ว");

            if (GameManager.Instance != null)
            {
                if (MapGenerator.Instance.GetCurrentLevel() >= 5)
                {
                    MapGenerator.Instance.DestroyAllEnemies();
                    GameManager.Instance.ShowWinPanel();
                    GameManager.Instance.CloseItemFlowerPanel();
                }
                else
                {
                    GameManager.Instance.ShowNextLevelPanel();
                    GameManager.Instance.CloseItemFlowerPanel();
                }
            }
        }
    }*/
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasWon && other.CompareTag("Player"))
        {
            hasWon = true;
            Debug.Log("🎉 Player เดินชนประตูแล้ว");

            int level = MapGenerator.Instance.GetCurrentLevel();

            if (GameManager.Instance != null)
            {
                if (level >= 5)
                {
                    MapGenerator.Instance.DestroyAllEnemies();
                    GameManager.Instance.ShowWinPanel();
                    GameManager.Instance.CloseItemFlowerPanel();
                }
                else
                {
                    GameManager.Instance.ShowNextLevelPanel();
                    GameManager.Instance.CloseItemFlowerPanel();
                }
            }
        }
    }

    private int GetRequiredFlowerCount(int level)
    {
        switch (level)
        {
            case 2: return 5;
            case 3: return 8;
            case 4: return 10;
            case 5: return 12;
            default: return 0;
        }
    }
}