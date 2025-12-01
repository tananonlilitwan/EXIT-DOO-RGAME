using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flee : Node
{
    private Blackborad blackboard;
    private AIAgentController agentController;
    private float fleeSpeed = 1.0f;
    private float safeDistance = 5.0f;
    private Vector2 targetPos;
    private bool hasTarget = false;
    private float seenTimer = 0f;
    private float maxSeenDuration = 4f;


    public Flee(Blackborad bb)
    {
        blackboard = bb;
        agentController = bb.GetComponent<AIAgentController>();
    }

    public override NodeState Evaluate()
    {
        Transform agent = blackboard.agent;
        Transform player = blackboard.player;

        if (agentController.IsBeingSeen)  //if (agentController.IsBeingSeen())
        {
            seenTimer += Time.deltaTime;
            Vector2 agentPos = agent.position;
            Vector2 playerPos = player.position;

            if (seenTimer > maxSeenDuration)
            {
                Debug.LogWarning($"{agentController.name} has been seen too long! Respawning...");
                Respawn(agent);
                state = NodeState.Success;
                return state;
            }
            if (!hasTarget)
            {
                targetPos = FindSafeFleePosition(agentPos, playerPos);

                if (!IsValidVector(targetPos))
                {
                    Debug.LogWarning($"{agentController.name} can't find a safe direction. Respawning...");
                    Respawn(agent);
                    state = NodeState.Success;
                    return state;
                }

                hasTarget = true;
               
                
            }
                    

            agent.position = Vector2.MoveTowards(agent.position, targetPos, fleeSpeed * Time.deltaTime);

            if (Vector2.Distance(agent.position, targetPos) < 0.1f)
            {
                hasTarget = false;
                state = NodeState.Success;
            }
            else
            {
                state = NodeState.Running;
            }

            return state;
        }
        else
        {
            seenTimer = 0f;       // รีเซ็ตเมื่อไม่ถูกมอง
            hasTarget = false;
            state = NodeState.Success;
            return state;
        }
    }

    private Vector2 FindSafeFleePosition(Vector2 agentPos, Vector2 playerPos)
    {
        Vector2 awayDir = agentPos - playerPos;

        if (awayDir == Vector2.zero)
            awayDir = Random.insideUnitCircle.normalized;
        else
            awayDir.Normalize();

        for (int i = 0; i < 64; i++)
        {
            float angle = Random.Range(-150f, 150f);
            Vector2 tryDir = Quaternion.Euler(0, 0, angle) * awayDir;
            Vector2 tryPos = agentPos + tryDir * safeDistance;

            if (IsPositionValid(tryPos))
                return tryPos;
        }

        return Vector2.negativeInfinity; // ใช้บอกว่า "หาทางไม่ได้"
    }

    private bool IsValidVector(Vector2 v)
    {
        return !(float.IsNaN(v.x) || float.IsNaN(v.y) || v == Vector2.negativeInfinity);
    }

    private bool IsPositionValid(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);

        if (x <= 0 || x >= MapGenerator.Instance.mapWidth - 1 ||
            y <= 0 || y >= MapGenerator.Instance.mapHeight - 1)
            return false;

        GameObject obj = MapGenerator.Instance.GetObjectAt(x, y);
        if (obj != null && (obj.tag == "Wall" || obj.tag == "Door" || obj.tag == "Tree" || obj.tag == "Note"))
            return false;

        return true;
    }

    private void Respawn(Transform agent)
    {
        GameObject oldAgent = agent.gameObject;
        MapGenerator.Instance.ClearCell(Vector2Int.RoundToInt(agent.position));
        Object.Destroy(oldAgent);
        MapGenerator.Instance.RespawnEnemy();
    }
}