using Game.Systems.AI.Actions;
using Game.Systems.AI.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.AI.Actions
{
    public class EventAction : BaseAction
    {

        [ShowInEditor]
        public UnityEvent unityAction;
        public override IEnumerator Execute(GameObject caller)
        {
            unityAction.Invoke();
            yield return ActionStatus.Success;
        }
    }
    public class GameObjectEventAction : BaseAction
    {

        [ShowInEditor]
        public UnityEvent<GameObject> unityAction;
        public override IEnumerator Execute(GameObject caller)
        {
            unityAction.Invoke(caller);
            yield return ActionStatus.Success;
        }
    }

}

