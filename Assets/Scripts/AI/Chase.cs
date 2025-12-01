using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : Node
{
    private Blackborad blackboard;
    private Animator animator;

    public Chase(Blackborad bb)
    {
        blackboard = bb;
        animator = bb.agent.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Debug.Log($"{blackboard.agent.name} is CHASING");
        Transform agent = blackboard.agent;
        Transform player = blackboard.player;

        float distance = Vector2.Distance(agent.position, player.position); // ใช้ Vector2 สำหรับ 2D

        if (distance > blackboard.chaseRange)
        {
            animator?.Play("Idle");
            state = NodeState.Failure;  // Stop chasing if player is too far
            return state;
        }
        else
        {
            animator?.Play("Walk");
            Vector2 direction = (player.position - agent.position).normalized;
            agent.position += (Vector3)(direction * Time.deltaTime * 1.0f); // ปรับ speed ได้

            // ถ้ามี SpriteRenderer ให้หันซ้ายขวาแบบ 2D:
            SpriteRenderer sr = agent.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.flipX = direction.x < 0;
            }

            state = NodeState.Running;
            return state;
        }
    }
}
