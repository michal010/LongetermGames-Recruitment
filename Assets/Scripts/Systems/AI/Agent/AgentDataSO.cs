using Game.Systems.AI.Actions;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "AgentData", menuName = "ScriptableObjects/AI/Data", order = 1)]
[Serializable]
public class AgentDataSO : ScriptableObject
{
    public string AgentName; //name for an agent, optional
    [SerializeReference] public List<BaseAction> Actions;

    public void AddAction()
    {
        #if UNITY_EDITOR
        Actions.Add(CreateInstance<BaseAction>());
        AssetDatabase.AddObjectToAsset(Actions[Actions.Count - 1], this);
        AssetDatabase.SaveAssets();
        #endif
    }
}
