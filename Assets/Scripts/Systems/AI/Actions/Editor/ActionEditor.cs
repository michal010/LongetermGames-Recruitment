using Game.Systems.AI.Actions;
using Game.Systems.AI.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Game.Systems.AI.Actions.Editor
{
    public class ActionEditor : EditorWindow
    {
        private static readonly string ACTIONS_DATA_PATH = "Assets/Resources/Data/AI/Actions";
        private static readonly string ACTIONS_DATA_EXTENSION = ".asset";
        private Type[] actionTypes;
        private Type selectedSubtype;
        private Vector2 subActionListScrollPosition;
        private Vector2 actionListScrollPosition;

        private BaseAction selectedAction;
        private BaseAction selectedSubAction;
        private List<BaseAction> selectedActionSubActionsList;
        private List<BaseAction> actions;
        
        private string newActionName = "New action";
        private string searchQuery = "";


        [MenuItem("Window/AI/AI Action Editor")]
        public static void ShowWindow()
        {
            ActionEditor window = GetWindow<ActionEditor>();
            window.titleContent = new GUIContent("AI Action Editor");
            window.Show();
        }

        private void OnEnable()
        {
            // Find all types that inherit from BaseAction
            actionTypes = GetAllSubtypes(typeof(BaseAction));
            actions = new List<BaseAction>();
            LoadActions();
        }



        private void OnGUI()
        {
            // Display selection dropdown and "Create" button
            EditorGUILayout.BeginHorizontal();

            // Show search bar for needs
            GUILayout.BeginVertical(GUILayout.Width(150));
            GUILayout.Label("Search:");
            searchQuery = GUILayout.TextField(searchQuery);

            // Show list of needs
            GUILayout.BeginVertical(GUILayout.Width(150));
            GUILayout.Label("Actions:");
            GUILayout.EndVertical();
            actionListScrollPosition = GUILayout.BeginScrollView(actionListScrollPosition, GUILayout.ExpandHeight(true));
            foreach (var action in actions.Where(n => n.Name.ToLower().Contains(searchQuery.ToLower())))
            {
                if (GUILayout.Button(action.Name))
                {
                    selectedAction = action;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();




            // Show selected need data
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (selectedAction != null)
            {
                GUILayout.Label("Selected action: " + selectedAction.Name);
                GUILayout.Space(10);

                // Edit need name
                GUILayout.BeginHorizontal(GUILayout.Width(300));
                GUILayout.Label("Action name: ");
                selectedAction.Name = GUILayout.TextField(selectedAction.Name);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                // Show data of selected action
                ShowObjectData(selectedAction);


                // Show sub-actions
                GUILayout.Label("Action sub-actions:");
                selectedActionSubActionsList = selectedAction.childrenActions;
                subActionListScrollPosition = GUILayout.BeginScrollView(subActionListScrollPosition, GUILayout.ExpandWidth(true));
                if (selectedActionSubActionsList != null || selectedActionSubActionsList.Count > 0)
                {
                    GUILayout.BeginVertical();
                    // Make a scrollable list.
                    for (int i = 0; i < selectedActionSubActionsList.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField(selectedActionSubActionsList[i].GetType().Name);

                        selectedActionSubActionsList[i] = (BaseAction)EditorGUILayout.ObjectField("Data", selectedActionSubActionsList[i], typeof(BaseAction), true);

                        // Show visible fields of class, so we can enter our input
                        GUILayout.BeginVertical();

                        ShowObjectData(selectedActionSubActionsList[i]);

                        EditorGUILayout.EndVertical();

                        if (GUILayout.Button("Remove"))
                        {
                            selectedAction.childrenActions.Remove(selectedActionSubActionsList[i]);
                            DestroyImmediate(selectedActionSubActionsList[i], true);
                            EditorUtility.SetDirty(selectedAction);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            Repaint();
                        }

                        if (GUILayout.Button("Save action data separately"))
                        {
                            // Rename asset so it's name won't be blank
                            SaveSubAction(selectedActionSubActionsList[i], i);
                            EditorUtility.SetDirty(selectedAction);
                            AssetDatabase.Refresh();
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                        GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }


                // Save selected need data
                GUILayout.Space(20);
                if (GUILayout.Button("Save Action"))
                {
                    SaveAction(selectedAction);
                }
                GUILayout.Space(20);
            }
            else
            {
                GUILayout.Label("No Action selected");
            }




            // Display dropdown with available subtypes
            int index = Array.IndexOf(actionTypes, selectedSubtype);
            index = EditorGUILayout.Popup(index, GetSubtypeNames(actionTypes));
            if(index != -1)
                selectedSubtype = actionTypes[index];

            if (GUILayout.Button("Add action"))
            {
                if(selectedSubtype == null)
                {
                    Debug.Log("Selected action type is null.");
                    return;
                }    

                // Create new ScriptableObject instance of selected subtype
                var newAction = ScriptableObject.CreateInstance(selectedSubtype) as BaseAction;

                // Add asset to existing asset
                AssetDatabase.AddObjectToAsset(newAction, selectedAction);

                selectedAction.childrenActions.Add(newAction);
                EditorUtility.SetDirty(selectedAction);
                
            }
            if (GUILayout.Button("Create new action"))
            {
                CreateNewAction(selectedSubtype);
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();


        }

        private void ShowObjectData(UnityEngine.ScriptableObject obj)
        {
            if(obj.GetType() == typeof(ComplexBaseAction))
            {
                return;
            }
            else
            {
                SerializedObject serializedObject = new SerializedObject(obj);
                SerializedProperty serializedProperty = serializedObject.GetIterator();
                while (serializedProperty.NextVisible(true))
                {
                    FieldInfo fieldInfo = obj.GetType().GetField(serializedProperty.name);

                    if (fieldInfo != null)
                    {
                        ShowInEditor[] attributes = (ShowInEditor[])fieldInfo.GetCustomAttributes(typeof(ShowInEditor), true);

                        if (attributes.Length > 0)
                        {
                            // 
                            EditorGUILayout.PropertyField(serializedProperty, true);
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private Type[] GetAllSubtypes(Type parentType)
        {
            List<Type> subtypes = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(parentType) && !type.IsAbstract)
                    {
                        subtypes.Add(type);
                    }
                }
            }

            return subtypes.ToArray();
        }

        private string[] GetSubtypeNames(Type[] types)
        {
            string[] names = new string[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                names[i] = types[i].Name;
            }

            return names;
        }


        private void CreateNewAction(Type fileType)
        {
            // Create new need data asset
            var newAction = ScriptableObject.CreateInstance(fileType) as BaseAction;
            newAction.Name = GenerateUniqueName(ACTIONS_DATA_PATH, newActionName, ACTIONS_DATA_EXTENSION);
            newAction.childrenActions = new List<BaseAction>();
            // Save new need data asset
            string filePath = ACTIONS_DATA_PATH + "/" + newAction.Name + ACTIONS_DATA_EXTENSION;
            AssetDatabase.CreateAsset(newAction, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Repaint();

            // Add new need to the list
            actions.Add(newAction);

            // Select new agent
            selectedAction = newAction;

        }

        private void LoadActions()
        {
            actions.Clear();
            var needDataGuids = AssetDatabase.FindAssets("t:BaseAction", new[] { ACTIONS_DATA_PATH });
            foreach (var guid in needDataGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var action = AssetDatabase.LoadAssetAtPath<BaseAction>(path);
                if (action != null)
                {
                    actions.Add(action);
                }
            }
        }

        //Change to save sub-action
        private void SaveSubAction(BaseAction action, int i)
        {
            // Create new action data asset
            var newAction = (BaseAction)ScriptableObject.CreateInstance(action.GetType());
            EditorUtility.CopySerialized(action, newAction);
            if (newAction.childrenActions == null)
                newAction.childrenActions = new List<BaseAction>();
            if (String.IsNullOrEmpty(newAction.name))
            {
                newAction.Name = GenerateUniqueName(ACTIONS_DATA_PATH, newActionName, ACTIONS_DATA_EXTENSION);
            }
            // Pass the reference to correct item in list
            selectedActionSubActionsList[i] = newAction;

            // Remove embeded scriptable object from parent
            AssetDatabase.RemoveObjectFromAsset(action);

            string filePath = ACTIONS_DATA_PATH + "/" + newAction.Name + ACTIONS_DATA_EXTENSION;

            // Save new need data asset
            AssetDatabase.CreateAsset(newAction, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Repaint();
        }

        private void SaveAction<T>(T action) where T : BaseAction
        {
            // Save selected agent data asset
            string oldFilePath = AssetDatabase.GetAssetPath(action);

            EditorUtility.SetDirty(action);
            AssetDatabase.RenameAsset(oldFilePath, action.Name);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Repaint();
        }

        private string GenerateUniqueName(string DATA_PATH, string fileName, string DATA_EXTENSION)
        {
            string filePath = DATA_PATH + "/" + fileName + DATA_EXTENSION;
            int count = 0;
            while(File.Exists(filePath))
            {
                count ++;
                filePath = DATA_PATH + "/" + fileName + "_" + count + ACTIONS_DATA_EXTENSION;
            }
            return fileName + "_" + count;
        }

    }
}
