using Game.Systems.AI.Attributes;
using System.Collections;
using UnityEngine;

namespace Game.Systems.AI.Actions
{
    public class WaitSecondsAction : BaseAction
    {
        [ShowInEditor]
        public uint Amount;
        public override IEnumerator Execute(GameObject caller)
        {
            yield return new WaitForSeconds(Amount);
        }
    }
}