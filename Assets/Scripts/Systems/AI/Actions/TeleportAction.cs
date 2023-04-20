using Game.Systems.AI.Actions;
using Game.Systems.AI.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Systems.AI.Actions
{
    public class TeleportAction : BaseAction
    {
        [ShowInEditor]
        public Vector3 position;
        public override IEnumerator Execute(GameObject caller)
        {
            NavMeshAgent agent;
            if (!caller.TryGetComponent<NavMeshAgent>(out agent))
            {
                Debug.Log("NavmeshAgent component not found. Make sure agent has NavMeshAgent component on it.");
                yield return ActionStatus.Failure;
            }
            agent.isStopped = true;
            yield return new WaitForEndOfFrame();
            agent.Warp(position);
            yield return new WaitForEndOfFrame();
            yield return ActionStatus.Success;

            //NavMeshHit hit;
            //if (NavMesh.SamplePosition(position, out hit, 1f, NavMesh.AllAreas))
            //{
            //    // Zatrzymaj agenta przed teleportacj¹
            //    agent.isStopped = true;
            //    yield return new WaitForEndOfFrame();
            //    // Przesuñ agenta do nowej pozycji
            //    //agent.Move(hit.position - caller.transform.position);
            //    agent.Warp(hit.position);

            //    yield return new WaitForEndOfFrame();
            //    // Wznów ruch agenta
            //    agent.isStopped = false;
            //    yield return new WaitForEndOfFrame();
            //    yield return ActionStatus.Success;
            //}
            //else
            //{
            //    Debug.LogError("Teleport destination is not in NavMesh area!");
            //    yield return ActionStatus.Failure;
            //}


            //yield return new WaitForEndOfFrame();
        }
    }
}
