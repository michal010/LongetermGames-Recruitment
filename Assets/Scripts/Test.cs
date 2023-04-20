using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    public NavMeshAgent agent;
    public Vector3 position = Vector3.zero;
    
    [ContextMenu("Test")]
    public void TestMove()
    {
        agent.destination = position;
    }
}
