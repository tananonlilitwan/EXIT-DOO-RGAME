using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    public Light2D flashlight;
    public Transform flashlightPivot;
    public float maxRayDistance = 10f;
    public LayerMask raycastMask;

    private bool isActive = false;
    private Transform player;

    void Start()
    {
        //flashlight.enabled = false;  // เริ่มต้นไฟฉายปิด
        
        if (flashlight == null)
        {
            flashlight = GetComponentInChildren<Light2D>();
        }

        if (flashlight != null)
        {
            flashlight.enabled = false; // เริ่มต้นปิดไฟฉาย
        }
        else
        {
            Debug.LogWarning("⚠️ Flashlight (Light component) not found in children!");
        }
    }

    void Update()
    {
        if (isActive)
        {
            RotateToMouse();
            ShootRay();
        }
    }

    void RotateToMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - flashlightPivot.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        flashlightPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

    void ShootRay()
    {
        Vector2 origin = flashlightPivot.position;
        Vector2 direction = flashlightPivot.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxRayDistance, raycastMask);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                AIAgentController ai = hit.collider.GetComponent<AIAgentController>();
                if (ai != null)
                {
                    ai.SetSeen(true);
                    ai.StopChasingDueToFlashlight();  // หยุดการไล่ล่า
                }
            }
            else
            {
                // ถ้าเจอสิ่งอื่นที่ไม่ใช่ Enemy ให้ปิดสถานะเห็น
                AIAgentController[] allEnemies = FindObjectsOfType<AIAgentController>();
                foreach (AIAgentController enemy in allEnemies)
                {
                    enemy.SetSeen(false);
                }
            }
        }
        else
        {
            // ถ้าไม่เจออะไรเลย ให้ปิดสถานะเห็นทุกตัว
            AIAgentController[] allEnemies = FindObjectsOfType<AIAgentController>();
            foreach (AIAgentController enemy in allEnemies)
            {
                enemy.SetSeen(false);
            }
        }
    }

    public void EnableFlashlight(Transform playerTransform)
    {
        /*player = playerTransform;
        isActive = true;
        flashlight.enabled = true;  // เปิดไฟฉาย*/
        
        player = playerTransform;
        isActive = true;

        if (flashlight != null)
        {
            flashlight.enabled = true;  // เปิดไฟฉาย
        }
    }
}
