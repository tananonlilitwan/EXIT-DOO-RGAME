using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Patrol : Node
{
    private Blackborad blackboard;
    private Animator animator;

    public Patrol(Blackborad bb)
    {
        blackboard = bb;
        animator = bb.agent.GetComponent<Animator>();
    }
    public override NodeState Evaluate()
    {
        Debug.Log($"{blackboard.agent.name} is PATROLLING");

        Transform agent = blackboard.agent;
        Transform player = blackboard.player;
        

        float distance = Vector3.Distance(agent.position, player.position);
        if (distance <= blackboard.chaseRange)
        {
            blackboard.hasPatrolTarget = false;
            state = NodeState.Failure;
            return state;
        }

        // ตั้งตำแหน่งใหม่หากยังไม่มี
        if (!blackboard.hasPatrolTarget)
        {
            blackboard.currentPatrolTarget = blackboard.GetRandomPatrolPosition();
            blackboard.hasPatrolTarget = true;
        }

        Vector3 target = blackboard.currentPatrolTarget;

        if (Vector3.Distance(agent.position, target) < 0.5f)
        {
            blackboard.hasPatrolTarget = false;
        }
        else
        {
            animator?.Play("Walk");
            Vector3 dir = (target - agent.position).normalized;
            agent.position += dir * Time.deltaTime * 2.0f;

             state = NodeState.Running;
            return state;
        }

        state = NodeState.Success;
        return state;
    }
}
