using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : Node
{
    private Blackborad blackboard;
    private float waitTime = 2.0f;
    private float elapsedTime = 0f;

    public Wait(Blackborad bb, float waitDuration)
    {
        blackboard = bb;
        waitTime = waitDuration;
    }

    public override NodeState Evaluate()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= waitTime)
        {
            elapsedTime = 0f;
            state = NodeState.Success;
        }
        else
        {
            state = NodeState.Running;
        }
        return state;
    }
}
