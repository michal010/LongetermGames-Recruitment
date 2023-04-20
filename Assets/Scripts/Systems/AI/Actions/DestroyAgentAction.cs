using Game.Systems.AI.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.AI.Actions
{
    public class DestroyAgentAction : BaseAction
    {
        public override IEnumerator Execute(GameObject caller)
        {
            Destroy(caller);
            yield return ActionStatus.Success;
        }
    }
}

