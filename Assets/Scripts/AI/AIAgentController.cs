using UnityEngine;

public class AIAgentController : MonoBehaviour
{
    public Transform player;               
    public float chaseDistance = 10f;      
    public float fleeDistance = 5f;        
    public float chaseSpeed = 3f;         
    public float wanderSpeed = 1.5f;     
    public float wanderWaitTime = 2f;     
    private bool isChasing = false;      
    private bool isFleeing = false;        
    private Vector3 wanderTarget;        
    private float wanderTimer;          
    public bool IsBeingSeen { get; private set; }
    private float timeSinceSeen = 0f;
    private Animator animator;
    private SpriteRenderer sr;
    private Blackborad blackboard;
    private bool hasPlayedEnemySound = false;

    
    void Start()
    {
        blackboard = GetComponent<Blackborad>();
        player = GameObject.FindWithTag("Player").transform;  
        wanderTarget = transform.position;                      
        wanderTimer = wanderWaitTime;                         
        
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    
    
    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (blackboard != null && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= blackboard.chaseRange && !hasPlayedEnemySound)
            {
                SoundManager.Instance.PlayEnemySound();
                hasPlayedEnemySound = true;
            }


            if (distance > blackboard.chaseRange)
            {
                hasPlayedEnemySound = false;
            }
        }
        
        if (IsBeingSeen)
        {
            timeSinceSeen += Time.deltaTime;
            Debug.Log($"{name} ถูกส่องไฟ {timeSinceSeen} วินาที");

            if (!isFleeing)
            {
                StopChasingDueToFlashlight(); // เริ่มหนีทันที
            }

            if (timeSinceSeen >= 3f)
            {
                Debug.LogWarning($"{name} ถูกส่องไฟนานเกินไป! วาร์ปหนี!");
                TeleportToSafePosition();
                IsBeingSeen = false;
                timeSinceSeen = 0f;
                isFleeing = false;
                animator?.Play("Idle");
                return;
            }
        }

       
        if (isFleeing)
        {
            FleeFromPlayer();
            return;
        }
        
        if (isChasing)
        {
            if (distanceToPlayer > chaseDistance + 2f)
            {
                StopChasing();
            }
            else
            {
                ChasePlayer();
            }
        }
        
        else if (distanceToPlayer <= chaseDistance)
        {
            StartChasing();
        }
        
        else
        {
            Wander();
        }
    }

    void StartChasing()
    {
        isChasing = true;
        isFleeing = false;
    }

    void StopChasing()
    {
        isChasing = false;
        animator?.Play("Idle");
    }

    void FleeFromPlayer()
    {
        animator?.Play("Walk");
        
        Vector3 direction = (transform.position - player.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, chaseSpeed * Time.deltaTime);
        
        FlipSprite(direction);
        FaceDirection(direction);
        
        // ตรวจสอบว่าไปไกลพอแล้วหรือยัง
        if (Vector3.Distance(transform.position, player.position) > fleeDistance)
        {
            StopFleeing(); 
        }
    }

    void StopFleeing()
    {
        isFleeing = false;
        animator?.Play("Idle");
        Wander();  
    }

    void ChasePlayer()
    {
        animator?.Play("Walk");
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        FlipSprite(direction);
        FaceDirection(direction);
    }
    
    void Wander()
    {
        animator?.Play("Walk");
        wanderTimer -= Time.deltaTime;
        
        if (wanderTimer <= 0)

        {
            for (int i = 0; i < 10; i++) // พยายามสุ่มตำแหน่งใหม่สูงสุด 10 ครั้ง
            {
                Vector3 offset = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
                Vector3 newTarget = transform.position + offset;
                int x = Mathf.RoundToInt(newTarget.x);
                int y = Mathf.RoundToInt(newTarget.y);

                GameObject obj = MapGenerator.Instance.GetObjectAt(x, y);

                if (obj == null || (obj.tag != "Wall" && obj.tag != "Door" && obj.tag != "Tree" && obj.tag != "Note"))
                {
                    wanderTarget = newTarget;
                    break;
                }
            }

            wanderTimer = wanderWaitTime;
        }

        Vector3 direction = (wanderTarget - transform.position).normalized;
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, wanderTarget, wanderSpeed * Time.deltaTime);
        
        int nextX = Mathf.RoundToInt(nextPosition.x);
        int nextY = Mathf.RoundToInt(nextPosition.y);
        GameObject nextObj = MapGenerator.Instance.GetObjectAt(nextX, nextY);

        if (nextObj != null && (nextObj.tag == "Wall" || nextObj.tag == "Door" || nextObj.tag == "Tree" || nextObj.tag == "Note"))
        {
            // ติดวัตถุ — สุ่มเป้าหมายใหม่ทันที
            wanderTimer = 0f; 
            return;
        }

        transform.position = nextPosition;
        FlipSprite(direction);
        FaceDirection(direction);
    }


   
    public void StopChasingDueToFlashlight()
    {
        isFleeing = true;  // เริ่มหลบหนี
        isChasing = false; // หยุดการไล่ล่า
        animator?.Play("Walk");
    }

    public void SetSeen(bool seen)
    {
        if (seen)
        {
            if (!IsBeingSeen)
            {
                // เริ่มจับเวลาเมื่อเริ่มถูกส่อง
                timeSinceSeen = 0f;
            }
        }
        else
        {
            // หยุดถูกส่อง รีเซ็ตเวลา
            timeSinceSeen = 0f;
        }

        IsBeingSeen = seen;
    }
    
    private void TeleportToSafePosition()
    {
        Debug.Log("เริ่มการวาร์ป");
        for (int i = 0; i < 64; i++)
        {
            Vector2 tryPos = new Vector2(Random.Range(1, MapGenerator.Instance.mapWidth - 1),
                Random.Range(1, MapGenerator.Instance.mapHeight - 1));

            GameObject obj = MapGenerator.Instance.GetObjectAt(Mathf.RoundToInt(tryPos.x), Mathf.RoundToInt(tryPos.y));
            if (obj == null || (obj.tag != "Wall" && obj.tag != "Door" && obj.tag != "Tree" && obj.tag != "Note"))
            {
                MapGenerator.Instance.ClearCell(Vector2Int.RoundToInt(transform.position)); // ล้างตำแหน่งเดิม
                transform.position = new Vector3(tryPos.x, tryPos.y, transform.position.z);
                Debug.Log($"{name} วาร์ปสำเร็จไปที่ {transform.position}");
                return;
            }
        }

        Debug.LogWarning($"{name} หาจุดวาร์ปไม่ได้!");
    }
    
    private void FlipSprite(Vector3 direction)
    {
        if (sr != null)
        {
            sr.flipX = direction.x < 0;
        }
    }
    
    private void FaceDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            transform.up = direction;
        }
    }
    
}
