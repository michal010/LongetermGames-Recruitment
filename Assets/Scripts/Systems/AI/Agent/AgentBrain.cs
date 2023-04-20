using Game.Systems.AI.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.AI.Agent
{
    public class AgentBrain : MonoBehaviour
    {
        public AgentDataSO data;
        private List<IEnumerator> currentActionList;
        private BaseAction currentAction;
        private BaseAction currentSubAction;

        // Start is called before the first frame update
        void Start()
        {
            currentActionList = new List<IEnumerator>();
            StartCoroutine(Run());
        }

        private void OnGUI()
        {
            GUI.TextField(new Rect(10, 10, 300, 50), "Current action: " + currentAction.Name);
            GUI.TextField(new Rect(10, 60, 300, 50), "Current sub-action: " + currentSubAction.Name);
        }

        IEnumerator Run()
        {
            foreach (BaseAction action in data.Actions)
            {
                // Get Actions, execute, get next need
                //SetupNextActions(need);
                currentAction = action;
                yield return StartCoroutine(ExecuteActions());
            }
            Debug.Log("End of actions.");
        }

        IEnumerator ExecuteActions()
        {
            Debug.Log("Executing new action set...");
            if (currentAction.childrenActions.Count > 0)
            {
                foreach (BaseAction action in currentAction.childrenActions)
                {
                    yield return ActionStatus.Running;
                    currentSubAction = action;
                    
                    //Debug.Log("Awaiting for: " + coroutine.ToString());
                    yield return StartCoroutine(action.Execute(gameObject));
                }
                yield return ActionStatus.Success;
            }
            else if (currentAction != null)
            {
                yield return StartCoroutine(currentAction.Execute(gameObject));
                yield return ActionStatus.Success;
            }
            else
            {

                Debug.Log("Empty action found: " + currentAction.name);
                yield return ActionStatus.Failure;
            }

        }

        private void SetupNextActions(BaseAction action)
        {
            currentActionList.Clear();
            foreach (BaseAction a in action.childrenActions)
            {
                currentActionList.Add(a.Execute(this.gameObject));
            }
        }

    }
}

