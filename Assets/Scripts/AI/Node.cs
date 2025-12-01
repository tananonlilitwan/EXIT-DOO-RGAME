using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState { Running, Success, Failure }

    public abstract class Node
    {
        protected NodeState state;
        public NodeState State => state;

        public abstract NodeState Evaluate();
    }

    public class Selector : Node
    {
        private List<Node> children = new List<Node>();

        public Selector(List<Node> nodes) => children = nodes;

        public override NodeState Evaluate()
        {
            foreach (var node in children)
            {
                NodeState ret = node.Evaluate();
                if (ret == NodeState.Success)
                {
                    state = NodeState.Success;
                    return state;
                }
                else if (ret == NodeState.Running)
                {
                    state = NodeState.Success;
                    return state;
                }
            }
            state = NodeState.Failure;
            return state;
        }
    }

    public class Sequence : Node
    {
        private List<Node> children = new List<Node>();
        private int currentNodeIdx = 0;

        public Sequence(List<Node> nodes) => children = nodes;

        public override NodeState Evaluate()
        {
            if (currentNodeIdx == children.Count)
            {
                state = NodeState.Success;
                currentNodeIdx = 0;
                return state;
            }
            else
            {
                NodeState result = children[currentNodeIdx].Evaluate();

                if (result == NodeState.Failure)
                {
                    state = NodeState.Failure;
                    currentNodeIdx = 0;
                    return state;
                }
                else if (result == NodeState.Success)
                {
                    currentNodeIdx++;
                }

                state = NodeState.Running;
                return state;
            }
        }
    }
public class Interruptions : Node
{
    private Node conditionNode;   
    private Node interruptAction; 

    public Interruptions(Node condition, Node action)
    {
        this.conditionNode = condition;
        this.interruptAction = action;
    }

    public override NodeState Evaluate()
    {
        NodeState conditionResult = conditionNode.Evaluate();

        if (conditionResult == NodeState.Success)
        {
            // ถ้าถูกมองอยู่ → ทำการ Flee
            state = interruptAction.Evaluate();
        }
        else
        {
            // ไม่ถูกมอง → ไม่ต้อง interrupt, ปล่อยให้ Selector ไป Node ต่อไปได้
            state = NodeState.Failure;
        }

        return state;
    }
}

