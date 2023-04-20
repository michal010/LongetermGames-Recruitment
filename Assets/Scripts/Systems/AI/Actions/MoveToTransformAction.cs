using Game.Systems.AI.Actions;
using Game.Systems.AI.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTransformAction : MoveAction
{
    [ShowInEditor]
    public Transform Target;
    public override IEnumerator Execute(GameObject caller)
    {
        UnityEngine.AI.NavMeshAgent agent;
        if (!caller.TryGetComponent<UnityEngine.AI.NavMeshAgent>(out agent))
        {
            Debug.Log("NavmeshAgent component not found. Make sure agent has NavMeshAgent component on it.");
            yield return ActionStatus.Failure;
        }

        Debug.Log("Setting destination to: " + Goal.ToString());
        agent.SetDestination(Goal);
        Debug.Log("Agent destination set to: " + agent.destination);
        Debug.Log(agent.ToString());
        yield return ActionStatus.Running;
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            yield return ActionStatus.Running;
        }
        Debug.Log("Path completed.");
        yield return ActionStatus.Success;
    }
}
