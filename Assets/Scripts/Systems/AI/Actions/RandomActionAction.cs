using Game.Systems.AI.Actions;
using Game.Systems.AI.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.AI.Actions
{
    public class RandomActionAction : ComplexBaseAction
    {
        public override IEnumerator Execute(GameObject caller)
        {
            BaseAction randomAction = SelectRandomAction();
            MonoBehaviour coroutineStarter = caller.GetComponent<MonoBehaviour>();
            if (randomAction != null && coroutineStarter != null)
            {
                yield return ActionStatus.Running;
                yield return coroutineStarter.StartCoroutine(randomAction.Execute(caller));
                yield return ActionStatus.Success;
            }
            else
            {
                Debug.Log("Couldn't pick random action or there is no monobehaviour attached to passed gameobject.");
                yield return ActionStatus.Failure;
            }
        }

        public BaseAction SelectRandomAction()
        {
            int actionIndex = Random.Range(0, childrenActions.Count);
            if (childrenActions[actionIndex])
                return childrenActions[actionIndex];
            else
                return null;
        }
    }
}
