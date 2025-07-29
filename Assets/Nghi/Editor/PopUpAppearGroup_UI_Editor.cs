using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
[CustomEditor(typeof(PopUpAppearGroup_UI))]
public class PopUpAppearGroup_UI_Editor : Editor
{
    private SerializedProperty appearGroups;
    private ReorderableList groupList;
    private Dictionary<int, ReorderableList> elementLists = new Dictionary<int, ReorderableList>();

    private void OnEnable()
    {
        appearGroups = serializedObject.FindProperty("appearGroups");

        // ReorderableList cho Groups
        groupList = new ReorderableList(serializedObject, appearGroups, true, true, true, true);
        groupList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "UI Groups");
        };

        groupList.elementHeightCallback = (int index) =>
        {
            SerializedProperty group = appearGroups.GetArrayElementAtIndex(index);
            SerializedProperty elements = group.FindPropertyRelative("elements");
            float height = EditorGUIUtility.singleLineHeight * 2 + 10; // Group name + elements label
            height += GetElementListHeight(index, elements);           // Dynamic height cho elements list
            return height + 10f;
        };

        groupList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty group = appearGroups.GetArrayElementAtIndex(index);
            rect.y += 2;
            float y = rect.y;

            // Group name
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                group.FindPropertyRelative("groupName"));
            y += EditorGUIUtility.singleLineHeight + 4;

            // Elements list
            SerializedProperty elements = group.FindPropertyRelative("elements");
            DrawElementList(elements, index, rect.x, y, rect.width);
        };
    }

    private void DrawElementList(SerializedProperty elements, int groupIndex, float x, float y, float width)
    {
        if (!elementLists.ContainsKey(groupIndex))
        {
            // Tạo ReorderableList cho elements trong group
            elementLists[groupIndex] = new ReorderableList(serializedObject, elements, true, true, true, true);

            elementLists[groupIndex].drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "UI Elements");
            };

            elementLists[groupIndex].elementHeightCallback = (int elementIndex) =>
            {
                SerializedProperty element = elements.GetArrayElementAtIndex(elementIndex);
                return CalculateElementHeight(element) + 10f;
            };

            elementLists[groupIndex].drawElementCallback = (Rect rect, int elementIndex, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = elements.GetArrayElementAtIndex(elementIndex);
                DrawElementFields(element, rect);
            };
        }

        // Vẽ ReorderableList element
        Rect listRect = new Rect(x, y, width, elementLists[groupIndex].GetHeight());
        elementLists[groupIndex].DoList(listRect);
    }

    private float CalculateElementHeight(SerializedProperty element)
    {
        float line = EditorGUIUtility.singleLineHeight + 4; // tăng khoảng cách 4px
        float height = line * 6; // target, delay, duration, animation, moveDir, moveDistance

        var animProp = element.FindPropertyRelative("animation");
        var animType = (PopUpAppearGroup_UI.AnimationType)animProp.enumValueIndex;

        if (animType == PopUpAppearGroup_UI.AnimationType.MoveAndFade ||
            animType == PopUpAppearGroup_UI.AnimationType.Move)
        {
            height += line; // fromAlpha
        }

        if (animType == PopUpAppearGroup_UI.AnimationType.Press)
        {
            height += line; // tiêu đề Press Settings
            height += line * 5; // ScaleUp, ScaleDown, UpDuration, DownDuration, SettleDuration
        }

        if (animType == PopUpAppearGroup_UI.AnimationType.DropBounce)
        {
            height += line; // tiêu đề DropBounce
            height += line * 3; // DropHeight, BounceCount, BounceDamping
        }

        if (animType == PopUpAppearGroup_UI.AnimationType.LaunchForward)
        {
            height += line; // tiêu đề LaunchForward
            height += line * 3; // useLaunchForward, launchDirection, LaunchDistanceZ
        }

        return height;
    }

    private void DrawElementFields(SerializedProperty element, Rect rect)
    {
        float line = EditorGUIUtility.singleLineHeight + 4;
        float y = rect.y + 2;

        // target
        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("target"));
        y += line;

        // delay
        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("delay"));
        y += line;

        // duration
        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("duration"));
        y += line;

        // animation type
        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("animation"));
        y += line;

        // move direction
        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("moveDirection"));
        y += line;

        // move distance
        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("moveDistance"));
        y += line;

        // from alpha (move/moveFade)
        var animType = (PopUpAppearGroup_UI.AnimationType)element.FindPropertyRelative("animation").enumValueIndex;
        if (animType == PopUpAppearGroup_UI.AnimationType.MoveAndFade ||
            animType == PopUpAppearGroup_UI.AnimationType.Move)
        {
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("fromAlpha"));
            y += line;
        }

        // Press settings
        if (animType == PopUpAppearGroup_UI.AnimationType.Press)
        {
            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                "Press Settings", EditorStyles.boldLabel);
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("ScaleUp"));
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("ScaleDown"));
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("UpDuration"));
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("DownDuration"));
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("SettleDuration"));
            y += line;
        }

        // DropBounce settings
        if (animType == PopUpAppearGroup_UI.AnimationType.DropBounce)
        {
            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                "DropBounce Settings", EditorStyles.boldLabel);
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("DropHeight"));
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("BounceCount"));
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("BounceDamping"));
            y += line;
        }

        // LaunchForward settings
        if (animType == PopUpAppearGroup_UI.AnimationType.LaunchForward)
        {
            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                "LaunchForward Settings", EditorStyles.boldLabel);
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("useLaunchForward"));
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("launchDirection"));
            y += line;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("LaunchDistanceZ"));
            y += line;
        }
    }

    private float GetElementListHeight(int groupIndex, SerializedProperty elements)
    {
        if (!elementLists.ContainsKey(groupIndex))
            return EditorGUIUtility.singleLineHeight;

        return elementLists[groupIndex].GetHeight();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // playOnStart toggle
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playOnStart"));

        // Groups list
        groupList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
