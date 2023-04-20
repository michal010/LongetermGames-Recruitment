using Game.Systems.AI.Actions;
using Game.Systems.AI.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.AI.Actions
{
    public class SpawGameObjectAction : BaseAction
    {
        [ShowInEditor]
        public GameObject Prefab;
        [ShowInEditor]
        public bool DestroyAfterTime;
        [ShowInEditor]
        public float DestroyTime;
        [ShowInEditor]
        public Vector3 Point;
        public override IEnumerator Execute(GameObject caller)
        {
            GameObject go = Instantiate(Prefab, Point, Quaternion.identity);
            if (DestroyAfterTime)
                Destroy(go, DestroyTime);
            yield return ActionStatus.Success;
        }
    }
}

