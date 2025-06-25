using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(UI_NonStatic_Effect))]
public class UI_NonStatic_EffectEditor : Editor
{
    private SerializedProperty effectItems;
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        effectItems = serializedObject.FindProperty("effectItems");

        reorderableList = new ReorderableList(serializedObject, effectItems, true, true, true, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "UI Effect Items");
        };

        reorderableList.elementHeightCallback = (int index) =>
        {
            var element = effectItems.GetArrayElementAtIndex(index);
            float height = EditorGUIUtility.singleLineHeight * 7f; // Base height

            SerializedProperty animType = element.FindPropertyRelative("animationType");
            AnimationType type = (AnimationType)animType.enumValueIndex;

            switch (type)
            {
                case AnimationType.ShakeHorizontal:
                case AnimationType.ShakeVertical:
                    height += EditorGUIUtility.singleLineHeight * 2f;
                    break;
                case AnimationType.PingPongX:
                case AnimationType.PingPongY:
                    height += EditorGUIUtility.singleLineHeight * 1f;
                    break;
            }
            return height + 10f;
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = effectItems.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("target"), new GUIContent("Target"));

            rect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("playMode"), new GUIContent("Play Mode"));

            rect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("animationType"), new GUIContent("Animation Type"));

            rect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("duration"), new GUIContent("Duration"));

            rect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("delay"), new GUIContent("Delay"));

            rect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("loop"), new GUIContent("Loop"));

            rect.y += EditorGUIUtility.singleLineHeight + 2;

            // Handle specific fields based on Animation Type
            SerializedProperty animType = element.FindPropertyRelative("animationType");
            AnimationType type = (AnimationType)animType.enumValueIndex;

            switch (type)
            {
                case AnimationType.ShakeHorizontal:
                case AnimationType.ShakeVertical:
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("shakeStrength"), new GUIContent("Shake Strength"));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("shakeVibrato"), new GUIContent("Shake Vibrato"));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    break;

                case AnimationType.PingPongX:
                case AnimationType.PingPongY:
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("pingPongDistance"), new GUIContent("Ping Pong Distance"));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    break;
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
