using Game.Systems.AI.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.AI.Actions
{
    public class RandomWeightedActionAction : BaseAction
    {
        [ShowInEditor]
        public Dictionary<BaseAction, int> WeightedActions;


        public override IEnumerator Execute(GameObject caller)
        {
            BaseAction randomAction = WeightedRandomPick();
            MonoBehaviour coroutineStarter = caller.GetComponent<MonoBehaviour>();
            if(randomAction != null && coroutineStarter != null)
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

        private BaseAction WeightedRandomPick()
        {
            int totalWeight = 0;
            List<KeyValuePair<BaseAction, int>> validWeightedActions = new List<KeyValuePair<BaseAction, int>>();

            foreach (KeyValuePair<BaseAction, int> pair  in WeightedActions)
            {
                if(pair.Value > 0)
                {
                    totalWeight += pair.Value;
                    validWeightedActions.Add(pair);
                }
            }

            int randomNumber = Random.Range(0, totalWeight);

            // Iteracja po obiektach i odejmowanie wagi od wylosowanej liczby
            foreach (KeyValuePair<BaseAction, int> pair in validWeightedActions)
            {
                if (randomNumber < pair.Value)
                {
                    return pair.Key;
                }

                randomNumber -= pair.Value;
            }

            // Zwracanie null, jeœli nie uda³o siê wylosowaæ obiektu
            return null;

        }
    }
}

