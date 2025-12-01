using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public LayerMask wallLayer; 

    private Vector2 moveInput;
    private Animator animator;
    private GameObject nearbyFlashlight;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        int currentLevel = MapGenerator.Instance.GetCurrentLevel(); // ต้องมั่นใจว่า method นี้มีใน MapGenerator
        int required = GameManager.Instance.GetRequiredFlowerCount(currentLevel); // ใช้ method ที่มีแล้วใน GameManager
        GameManager.Instance.UpdateItemFlowerUI(0, required, currentLevel);
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        TryMove(moveInput);
        UpdateAnimation(moveInput);
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nearbyFlashlight != null)
            {
                PickUpFlashlight();
            }
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.ResumeGame();
        }
    }

    void TryMove(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return;
        }

        Vector2 newPos = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;

        Collider2D hit = Physics2D.OverlapBox(newPos, Vector2.one * 0.9f, 0f, wallLayer);
        if (hit == null)
        {
            
            transform.position = newPos;
        }
    }
    
    void UpdateAnimation(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            animator.SetBool("IsMoving", false);
            animator.speed = 0;
            return;
        }
        else
        {
            animator.speed = 1;
        }

        animator.SetBool("IsMoving", true);

        if (direction.y > 0)
        {
            animator.Play("AnimationUp");
        }
        else if (direction.y < 0)
        {
            animator.Play("AnimationDown");
        }
        else if (direction.x > 0)
        {
            animator.Play("AnimationRight");
        }
        else if (direction.x < 0)
        {
            animator.Play("AnimationLeft");
        }
    }
    
    private void PickUpFlashlight()
    {
        Light2D childLight = nearbyFlashlight.GetComponentInChildren<Light2D>();
        if (childLight != null)
        {
            childLight.enabled = false; 
        }
        
        nearbyFlashlight.transform.SetParent(this.transform);
        nearbyFlashlight.SetActive(true);
        
        
        FlashlightController flashlightController = nearbyFlashlight.GetComponent<FlashlightController>();
        if (flashlightController != null)
        {
            flashlightController.EnableFlashlight(this.transform);
            flashlightController.enabled = true;
        }

        nearbyFlashlight.transform.localPosition = new Vector3(0.56f, 0f, 0f);
        nearbyFlashlight.transform.localRotation = Quaternion.Euler(0f, 0f, -31.059f);

        BoxCollider2D col = nearbyFlashlight.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        nearbyFlashlight = null;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Flashlight"))
        {
            nearbyFlashlight = other.gameObject;
            MapGenerator.Instance.FlashlightCollected();
            GameManager.Instance.ShowDialogPanel();
        }
        
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.ShowLosePanel();
            GameManager.Instance.CloseItemFlowerPanel();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Flashlight") && other.gameObject == nearbyFlashlight)
        {
            nearbyFlashlight = null;

            GameManager.Instance.HitDialogPanel();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)(moveInput * moveSpeed * Time.deltaTime), Vector3.one * 0.9f);
    }
}