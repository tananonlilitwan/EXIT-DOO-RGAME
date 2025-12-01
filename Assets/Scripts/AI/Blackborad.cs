using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackborad : MonoBehaviour
{
    public Transform player;
    public Transform agent;
    private Animator animator;
    private string currentAnim = "";
    public float attackRange = 1.0f;
    public float chaseRange = 10.0f;

    public Vector2 mapMinBounds = new Vector2(-10, -10);
    public Vector2 mapMaxBounds = new Vector2(10, 10);
    public Vector3 currentPatrolTarget;
    public bool hasPatrolTarget = false;

    private bool hasPlayedEnemySound = false;

    void Awake()
    {
        animator = GetComponent<Animator>();

        agent = transform;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
            return;
        }

        float distance = Vector3.Distance(agent.position, player.position);

        // ถ้าเจอในรัศมีไล่ล่า และยังไม่เล่นเสียง
        if (distance <= chaseRange && !hasPlayedEnemySound)
        {
            //SoundManager.Instance.PlayEnemySound();
            hasPlayedEnemySound = true;
        }

        // ถ้าออกนอกระยะ ให้ reset flag เพื่อให้เล่นได้อีกเมื่อเข้าใหม่
        if (distance > chaseRange)
        {
            hasPlayedEnemySound = false;
        }
    }

    public Vector3 GetRandomPatrolPosition()
    {
        float x = Random.Range(mapMinBounds.x, mapMaxBounds.x);
        float y = Random.Range(mapMinBounds.y, mapMaxBounds.y);
        return new Vector3(x, y, agent.position.z);
    }
    void PlayAnim(string animName)
    {
        if (currentAnim == animName || animator == null) return;

        animator.Play(animName);
        currentAnim = animName;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

}