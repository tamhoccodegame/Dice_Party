using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UI_VFX_Spawner))]
public class UI_VFX_Spawner_Editor : Editor
{
    SerializedProperty modeConfigs;

    void OnEnable()
    {
        modeConfigs = serializedObject.FindProperty("modeConfigs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnArea"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnInterval"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnOnStart"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("destroyDelay"));

        EditorGUILayout.PropertyField(modeConfigs, new GUIContent("Mode Config List"), false);

        if (modeConfigs.isExpanded)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < modeConfigs.arraySize; i++)
            {
                SerializedProperty config = modeConfigs.GetArrayElementAtIndex(i);
                SerializedProperty mode = config.FindPropertyRelative("mode");
                SerializedProperty enabled = config.FindPropertyRelative("enabled");
                SerializedProperty vfxPrefabs = config.FindPropertyRelative("vfxPrefabs");

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(enabled);
                EditorGUILayout.PropertyField(mode);
                EditorGUILayout.PropertyField(vfxPrefabs, true);

                UI_VFX_Spawner.SpawnMode modeVal = (UI_VFX_Spawner.SpawnMode)mode.enumValueIndex;

                switch (modeVal)
                {
                    case UI_VFX_Spawner.SpawnMode.RandomInArea:
                        // random-related settings per VFX here if required
                        break;

                    case UI_VFX_Spawner.SpawnMode.SequentialAtPoints:
                        SerializedProperty spawnPoints = serializedObject.FindProperty("spawnPoints");
                        SerializedProperty playInOrder = serializedObject.FindProperty("playInOrder");
                        SerializedProperty playOnAwakeOnly = serializedObject.FindProperty("playOnAwakeOnly");
                        SerializedProperty simultaneousSpawn = serializedObject.FindProperty("simultaneousSpawnAtAllPoints");
                        EditorGUILayout.PropertyField(spawnPoints, true);
                        EditorGUILayout.PropertyField(playInOrder);
                        EditorGUILayout.PropertyField(playOnAwakeOnly);
                        EditorGUILayout.PropertyField(simultaneousSpawn);
                        break;

                    case UI_VFX_Spawner.SpawnMode.MoveAlongPath:
                        SerializedProperty pathPoints = serializedObject.FindProperty("pathPoints");
                        SerializedProperty moveSpeed = serializedObject.FindProperty("moveSpeed");
                        SerializedProperty waitBetweenMoves = serializedObject.FindProperty("waitBetweenMoves");
                        SerializedProperty loopPath = serializedObject.FindProperty("loopPath");
                        EditorGUILayout.PropertyField(pathPoints, true);
                        EditorGUILayout.PropertyField(moveSpeed);
                        EditorGUILayout.PropertyField(waitBetweenMoves);
                        EditorGUILayout.PropertyField(loopPath);
                        break;
                }

                if (GUILayout.Button("Remove Config"))
                {
                    modeConfigs.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndVertical();
            }
            if (GUILayout.Button("Add New Mode Config"))
            {
                modeConfigs.InsertArrayElementAtIndex(modeConfigs.arraySize);
            }
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
