using Game.Systems.AI.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Systems.AI.Actions
{
    public class MoveAction : BaseAction
    {
        [ShowInEditor]
        public Vector3 Goal = Vector2.zero;

        public override IEnumerator Execute(GameObject caller)
        {
            NavMeshAgent agent;
            if (!caller.TryGetComponent<NavMeshAgent>(out agent))
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
}