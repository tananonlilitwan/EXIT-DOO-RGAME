using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beingseen : Node
{
    private AIAgentController _aiAgentController;
    public Beingseen(AIAgentController aiAgentController)
    {
        _aiAgentController = aiAgentController;
    }
    public override NodeState Evaluate()
    {
        // Check if the AI agent is being seen by the player
        if (_aiAgentController.IsBeingSeen)  //if (_aiAgentController.IsBeingSeen())
        {
            // If the AI agent is being seen, return success
            return NodeState.Success;
        }
        else
        {
            // If the AI agent is not being seen, return failure
            return NodeState.Failure;
        }
        return state;
    }
}
