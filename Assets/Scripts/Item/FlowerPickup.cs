/*using UnityEngine;

public class FlowerPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (FlowerManager.Instance != null)
            {
                FlowerManager.Instance.CollectFlower();
            }
            Destroy(gameObject);
        }
    }
}*/


using UnityEngine;

public class FlowerPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddCollectedFlower(); // บวกจำนวนดอกไม้
            Destroy(gameObject); // ลบตัวเองออกจากฉาก
        }
    }
}
