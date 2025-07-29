using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
[CustomEditor(typeof(PopUpDisappearGroup_UI))]
public class PopUpDisappearGroup_UI_Editor : Editor
{
    private SerializedProperty triggerModeProp;
    private SerializedProperty appearScriptProp;
    private SerializedProperty disappearGroupsProp;
    private ReorderableList groupList;

    void OnEnable()
    {
        triggerModeProp = serializedObject.FindProperty("triggerMode");
        appearScriptProp = serializedObject.FindProperty("appearScript");
        disappearGroupsProp = serializedObject.FindProperty("disappearGroups");

        groupList = new ReorderableList(serializedObject, disappearGroupsProp, true, true, true, true);
        groupList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "UI Groups", EditorStyles.boldLabel);
        };

        groupList.elementHeightCallback = index =>
        {
            var group = disappearGroupsProp.GetArrayElementAtIndex(index);
            var elementsProp = group.FindPropertyRelative("elements");
            return EditorGUIUtility.singleLineHeight * 2 + 10 + // GroupName + Elements list header
                   (elementsProp.isExpanded ? CalculateElementsHeight(elementsProp) : 0);
        };

        groupList.drawElementCallback = (rect, index, active, focused) =>
        {
            var group = disappearGroupsProp.GetArrayElementAtIndex(index);
            var groupNameProp = group.FindPropertyRelative("groupName");
            var elementsProp = group.FindPropertyRelative("elements");

            float y = rect.y + 2;

            // Group Name
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                groupNameProp, new GUIContent("Group Name"));
            y += EditorGUIUtility.singleLineHeight + 4;

            // Elements (nested list)
            elementsProp.isExpanded = EditorGUI.Foldout(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                elementsProp.isExpanded, "UI Elements", true);
            y += EditorGUIUtility.singleLineHeight + 2;

            if (elementsProp.isExpanded)
            {
                DrawUIElementsList(elementsProp, rect.x + 10, y, rect.width - 10);
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(triggerModeProp);
        EditorGUILayout.PropertyField(appearScriptProp);

        GUILayout.Space(10);
        groupList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    // ==================== ELEMENTS ======================

    private void DrawUIElementsList(SerializedProperty elementsProp, float x, float y, float width)
    {
        for (int i = 0; i < elementsProp.arraySize; i++)
        {
            var element = elementsProp.GetArrayElementAtIndex(i);
            float height = CalculateElementHeight(element);

            Rect boxRect = new Rect(x, y, width, height);
            GUI.Box(boxRect, GUIContent.none); // background box

            DrawElementFields(element, new Rect(x + 4, y + 4, width - 8, height - 8));

            y += height + 6; // spacing between elements
        }

        // Add/remove buttons
        Rect addRect = new Rect(x, y, 60, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(addRect, "+"))
            elementsProp.InsertArrayElementAtIndex(elementsProp.arraySize);
        Rect removeRect = new Rect(x + 65, y, 60, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(removeRect, "-") && elementsProp.arraySize > 0)
            elementsProp.DeleteArrayElementAtIndex(elementsProp.arraySize - 1);
    }

    private float CalculateElementsHeight(SerializedProperty elementsProp)
    {
        float total = 0;
        for (int i = 0; i < elementsProp.arraySize; i++)
        {
            total += CalculateElementHeight(elementsProp.GetArrayElementAtIndex(i)) + 6;
        }
        return total + EditorGUIUtility.singleLineHeight;
    }

    private float CalculateElementHeight(SerializedProperty element)
    {
        float line = EditorGUIUtility.singleLineHeight + 4;
        float height = line * 6; // base fields: target, delay, duration, animation, moveDirection, moveDistance

        var animProp = element.FindPropertyRelative("animation");
        var animType = (PopUpAppearGroup_UI.AnimationType)animProp.enumValueIndex;

        if (animType == PopUpAppearGroup_UI.AnimationType.MoveAndFade ||
            animType == PopUpAppearGroup_UI.AnimationType.Move)
        {
            height += line; // fromAlpha
        }

        if (animType == PopUpAppearGroup_UI.AnimationType.Press)
        {
            height += line; // label
            height += line * 5; // ScaleUp, ScaleDown, UpDuration, DownDuration, SettleDuration
        }

        if (animType == PopUpAppearGroup_UI.AnimationType.DropBounce)
        {
            height += line; // label
            height += line * 3; // DropHeight, BounceCount, BounceDamping
        }

        if (animType == PopUpAppearGroup_UI.AnimationType.LaunchForward)
        {
            height += line; // label
            height += line * 3; // useLaunchForward, launchDirection, LaunchDistanceZ
        }

        return height + 10;
    }

    private void DrawElementFields(SerializedProperty element, Rect rect)
    {
        float line = EditorGUIUtility.singleLineHeight + 4;
        float y = rect.y;

        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("target"));
        y += line;

        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("delay"));
        y += line;

        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("duration"));
        y += line;

        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("animation"));
        y += line;

        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("moveDirection"));
        y += line;

        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("moveDistance"));
        y += line;

        var animType = (PopUpAppearGroup_UI.AnimationType)element.FindPropertyRelative("animation").enumValueIndex;

        if (animType == PopUpAppearGroup_UI.AnimationType.MoveAndFade ||
            animType == PopUpAppearGroup_UI.AnimationType.Move)
        {
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("fromAlpha"));
            y += line;
        }

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
}
