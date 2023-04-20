using Game.Systems.AI.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.AI.Actions
{
    public enum ActionStatus { Success, Failure, Running }

    public class BaseAction : ScriptableObject
    {
        [ShowInEditor]
        public string Name;
        [ShowInEditor]
        [SerializeReference] public List<BaseAction> childrenActions;
        public virtual IEnumerator Execute(GameObject caller)
        {
            Debug.Log("Action not implemented.");
            yield return ActionStatus.Failure;
        }
    }

}