using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(PopUpAppearOnly_UI))]
public class PopUpAppearOnly_UI_Editor : Editor
{
    SerializedProperty appearSequence;
    SerializedProperty playOnStart;

    void OnEnable()
    {
        appearSequence = serializedObject.FindProperty("appearSequence");
        playOnStart = serializedObject.FindProperty("playOnStart");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(playOnStart);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Appear Sequence Settings", EditorStyles.boldLabel);

        for (int i = 0; i < appearSequence.arraySize; i++)
        {
            SerializedProperty element = appearSequence.GetArrayElementAtIndex(i);
            SerializedProperty target = element.FindPropertyRelative("target");
            SerializedProperty delay = element.FindPropertyRelative("delay");
            SerializedProperty duration = element.FindPropertyRelative("duration");
            SerializedProperty animation = element.FindPropertyRelative("animation");

            SerializedProperty moveDirection = element.FindPropertyRelative("moveDirection");
            SerializedProperty moveDistance = element.FindPropertyRelative("moveDistance");
            SerializedProperty fromAlpha = element.FindPropertyRelative("fromAlpha");

            SerializedProperty ScaleUp = element.FindPropertyRelative("ScaleUp");
            SerializedProperty ScaleDown = element.FindPropertyRelative("ScaleDown");
            SerializedProperty UpDuration = element.FindPropertyRelative("UpDuration");
            SerializedProperty DownDuration = element.FindPropertyRelative("DownDuration");
            SerializedProperty SettleDuration = element.FindPropertyRelative("SettleDuration");

            SerializedProperty DropHeight = element.FindPropertyRelative("DropHeight");
            SerializedProperty BounceCount = element.FindPropertyRelative("BounceCount");
            SerializedProperty BounceDamping = element.FindPropertyRelative("BounceDamping");

            SerializedProperty useLaunchForward = element.FindPropertyRelative("useLaunchForward");
            SerializedProperty launchDirection = element.FindPropertyRelative("launchDirection");
            SerializedProperty LaunchDistanceZ = element.FindPropertyRelative("LaunchDistanceZ");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Element " + i, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(target);
            EditorGUILayout.PropertyField(delay);
            EditorGUILayout.PropertyField(duration);
            EditorGUILayout.PropertyField(animation);

            PopUpAppearOnly_UI.AnimationType animType = (PopUpAppearOnly_UI.AnimationType)animation.enumValueIndex;

            if (animType == PopUpAppearOnly_UI.AnimationType.MoveAndFade)
            {
                EditorGUILayout.PropertyField(moveDirection);
                EditorGUILayout.PropertyField(moveDistance);
                EditorGUILayout.PropertyField(fromAlpha);
            }

            if (animType == PopUpAppearOnly_UI.AnimationType.Press)
            {
                EditorGUILayout.LabelField("Press Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(ScaleUp);
                EditorGUILayout.PropertyField(ScaleDown);
                EditorGUILayout.PropertyField(UpDuration);
                EditorGUILayout.PropertyField(DownDuration);
                EditorGUILayout.PropertyField(SettleDuration);
            }
            else if (animType == PopUpAppearOnly_UI.AnimationType.DropBounce)
            {
                EditorGUILayout.LabelField("DropBounce Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(DropHeight);
                EditorGUILayout.PropertyField(BounceCount);
                EditorGUILayout.PropertyField(BounceDamping);
            }
            else if (animType == PopUpAppearOnly_UI.AnimationType.LaunchForward)
            {
                EditorGUILayout.LabelField("LaunchForward Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(useLaunchForward);
                if (useLaunchForward.boolValue)
                {
                    EditorGUILayout.PropertyField(launchDirection);
                    EditorGUILayout.PropertyField(LaunchDistanceZ);
                }
            }

            if (GUILayout.Button("Remove Element"))
            {
                appearSequence.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add New Element"))
        {
            appearSequence.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
