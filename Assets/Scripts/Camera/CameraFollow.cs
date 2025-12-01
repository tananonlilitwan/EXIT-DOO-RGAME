using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public string playerTag = "Player"; 

    private Transform target;

    void Start()
    {
        Invoke(nameof(FindPlayer), 0.1f); 
    }

    public void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPos = target.position;
            newPos.z = transform.position.z;
            transform.position = newPos;
        }
        
    }
}
