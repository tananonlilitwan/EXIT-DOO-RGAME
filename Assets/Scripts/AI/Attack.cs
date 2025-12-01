using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Node
{
    private Blackborad blackboard;
    private Animator animator;

    public Attack(Blackborad bb)
    {
        blackboard = bb;
        animator = bb.agent.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Debug.Log($"{blackboard.agent.name} is ATTACKING");

        Transform agent = blackboard.agent;
        Transform player = blackboard.player;

        if (Vector3.Distance(agent.position, player.position) <= blackboard.attackRange)
        {
            Debug.Log("Enemy Attacks!");
            animator?.Play("Attack"); // Play attack animation
            state = NodeState.Success; // Attack succeeds
        }
        else
        {
            state = NodeState.Failure; // Can't attack if out of range
        }
        return state;
    }

    
}
