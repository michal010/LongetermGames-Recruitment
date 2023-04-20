using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Systems.AI.Actions;
using System.Linq;

public class AgentManagerWindow : EditorWindow
{
    private static readonly string AGENT_DATA_PATH = "Assets/Resources/Data/AI/Agents";

    private static readonly string AGENT_DATA_EXTENSION = ".asset";
    private string newAgentName = "New Agent";


    private AgentDataSO selectedAgent;
    private List<BaseAction> selectedAgentActionList;
    private List<AgentDataSO> agents;
    private Vector2 scrollPosition;
    private string searchQuery = "";

    //int currentTab = 0;


    [MenuItem("Window/AI/Agent Editor")]
    public static void ShowWindow()
    {
        GetWindow<AgentManagerWindow>("Agent manager window");
    }

    private void OnEnable()
    {
        // Initalize agent and needs list
        agents = new List<AgentDataSO>();
        LoadAgents();
    }

    private void OnDisable()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        //currentTab = GUILayout.Toolbar(currentTab, new string[] { "Agent Editor", "Needs Editor" });

        ShowAgentEditor();
        //switch (currentTab)
        //{
        //    case 0:
        //        // First tab content
        //        break;

        //    case 1:
        //        // Second tab content
        //        // ShowNeedsEditor();
        //        ActionEditor.ShowWindow();
        //        break;
        //}
        if (GUILayout.Button("Save all Data"))
        {
            foreach (var agent in agents)
            {
                SaveAgent(agent);
            }
        }
    }

    private void ShowAgentEditor()
    {
        GUILayout.BeginHorizontal();
        // Show search bar for agents
        GUILayout.BeginVertical(GUILayout.Width(150));
        GUILayout.Label("Search:");
        searchQuery = GUILayout.TextField(searchQuery);

        // Show a list of agents
        GUILayout.BeginVertical(GUILayout.Width(150));
        GUILayout.Label("Agents:");
        GUILayout.EndVertical();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
        foreach (var agent in agents.Where(a => a.AgentName.ToLower().Contains(searchQuery.ToLower())))
        {
            if (GUILayout.Button(agent.AgentName))
            {
                selectedAgent = agent;
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        // Show selected agent data
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        if (selectedAgent != null)
        {
            GUILayout.Label("Selected agent: " + selectedAgent.AgentName);
            GUILayout.Space(10);

            // Edit agent name
            GUILayout.BeginHorizontal();
            GUILayout.Label("Agent Name: ");
            selectedAgent.AgentName = GUILayout.TextField(selectedAgent.AgentName);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Edit agent needs
            GUILayout.Label("Agent actions:");
            selectedAgentActionList = selectedAgent.Actions;
            for (int i = 0; i < selectedAgentActionList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Action " + (i + 1) + ":");
                EditorGUILayout.LabelField(selectedAgentActionList[i].GetType().Name);
                GUILayout.Label(selectedAgentActionList[i].Name);
                selectedAgentActionList[i] = (BaseAction)EditorGUILayout.ObjectField(selectedAgentActionList[i], typeof(BaseAction), false);
                GUILayout.EndHorizontal();
            }

            // Save selected agent data
            GUILayout.Space(20);
            if (GUILayout.Button("Save Agent"))
            {
                SaveAgent(selectedAgent);
            }
            if (GUILayout.Button("Add Action"))
            {
                //Add need to agent
                selectedAgent.AddAction();
            }
            GUILayout.Space(20);
        }
        else
        {
            GUILayout.Label("No agent selected");
        }
        if (GUILayout.Button("Create new agent"))
        {
            CreateNewAgent();
        }
        if (GUILayout.Button("Save all agents"))
        {
            foreach (var agent in agents)
            {
                SaveAgent(agent);
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private void LoadAgents()
    {
        agents.Clear();
        var agentDataGuids = AssetDatabase.FindAssets("t:AgentDataSO", new[] { AGENT_DATA_PATH });
        foreach (var guid in agentDataGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var agent = AssetDatabase.LoadAssetAtPath<AgentDataSO>(path);
            if (agent != null)
            {
                agents.Add(agent);
            }
        }
    }

    private void CreateNewAgent()
    {
        // Create new agent data asset
        AgentDataSO newAgent = ScriptableObject.CreateInstance<AgentDataSO>();
        newAgent.AgentName = newAgentName;
        newAgent.Actions = new List<BaseAction>();
        // Save new agent data asset
        AssetDatabase.CreateAsset(newAgent, AGENT_DATA_PATH + "/" + newAgentName + AGENT_DATA_EXTENSION);
        AssetDatabase.SaveAssets();

        // Add new agent to the list
        agents.Add(newAgent);

        // Select new agent
        selectedAgent = newAgent;

        AssetDatabase.Refresh();
    }

    private void SaveAgent(AgentDataSO agent)
    {
        // Save selected agent data asset
        string oldFilePath = AssetDatabase.GetAssetPath(agent);

        EditorUtility.SetDirty(agent);
        AssetDatabase.RenameAsset(oldFilePath, agent.AgentName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Repaint();
    }
}

