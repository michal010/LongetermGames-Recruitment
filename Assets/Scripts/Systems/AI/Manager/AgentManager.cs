using Game.Systems.AI.Actions;
using Game.Systems.AI.Agent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*TODO:
 *  Create custom editor for managing agents
 * 
 * 
 */

namespace Game.Systems.AI.Manager
{
    public class AgentManager : MonoBehaviour
    {
        public GameObject AgentPrefab;
        public Transform SpawnPoint;
        public AgentDataSO data;

        private void Update()
        {
            if (!GameObject.Find("AI Agent(Clone)"))
                SpawnAgent();
        }

        public void SpawnAgent()
        {
            {
                GameObject newAgent = Instantiate(AgentPrefab, SpawnPoint.position, Quaternion.identity);
                newAgent.GetComponent<AgentBrain>().data = data;
            }
        }

    }
}

