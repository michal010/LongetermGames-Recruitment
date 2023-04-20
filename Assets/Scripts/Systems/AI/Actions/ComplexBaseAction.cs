using Game.Systems.AI.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexBaseAction : BaseAction
{
    public override IEnumerator Execute(GameObject caller)
    {
        MonoBehaviour coroutineStarter = caller.GetComponent<MonoBehaviour>();
        foreach (var a in childrenActions)
        {
            yield return ActionStatus.Running;
            yield return coroutineStarter.StartCoroutine(a.Execute(caller));
        }
        yield return ActionStatus.Success;
    }
}
