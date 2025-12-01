/*using UnityEngine;

public class FlowerManager : MonoBehaviour
{
    public static FlowerManager Instance;

    private int flowerCount = 0;
    private int flowersCollected = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void SetFlowerCount(int count)
    {
        flowerCount = count;
        flowersCollected = 0;
        GameManager.Instance.UpdateItemFlowerUI(flowersCollected);
    }

    public void CollectFlower()
    {
        flowersCollected++;
        GameManager.Instance.UpdateItemFlowerUI(flowersCollected);
    }

    public bool HasCollectedAllFlowers()
    {
        return flowersCollected >= flowerCount;
    }
}*/