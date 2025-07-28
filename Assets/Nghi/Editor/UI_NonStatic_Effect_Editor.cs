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
            float height = EditorGUIUtility.singleLineHeight * 8f;

            // AnimationType A
            var animType = element.FindPropertyRelative("animationType");
            var type = (AnimationType)animType.enumValueIndex;
            if (type == AnimationType.ShakeHorizontal || type == AnimationType.ShakeVertical)
                height += EditorGUIUtility.singleLineHeight * 2;
            else if (type == AnimationType.PingPongX || type == AnimationType.PingPongY)
                height += EditorGUIUtility.singleLineHeight;
            else if (type == AnimationType.PressScale)
                height += EditorGUIUtility.singleLineHeight * 5; // 1 dòng header + 4 field

            // Target B
            var useB = element.FindPropertyRelative("useTargetB");
            if (useB.boolValue)
            {
                height += EditorGUIUtility.singleLineHeight * 6f;

                var animTypeB = element.FindPropertyRelative("animationTypeB");
                var typeB = (AnimationType)animTypeB.enumValueIndex;
                if (typeB == AnimationType.ShakeHorizontal || typeB == AnimationType.ShakeVertical)
                    height += EditorGUIUtility.singleLineHeight * 2;
                else if (typeB == AnimationType.PingPongX || typeB == AnimationType.PingPongY)
                    height += EditorGUIUtility.singleLineHeight;
                else if (typeB == AnimationType.PressScale)
                    height += EditorGUIUtility.singleLineHeight * 5; // header + 4 field
            }
            return height + 10f;
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = effectItems.GetArrayElementAtIndex(index);
            rect.y += 2;
            float y = rect.y;

            // Target A
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("target"));
            y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("playMode"));
            y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("animationType"));
            y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("duration"));
            y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("delay"));
            y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("loop"));
            y += EditorGUIUtility.singleLineHeight + 2;

            // Extra fields for type A
            var animType = element.FindPropertyRelative("animationType");
            var type = (AnimationType)animType.enumValueIndex;
            if (type == AnimationType.ShakeHorizontal || type == AnimationType.ShakeVertical)
            {
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shakeStrength"));
                y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shakeVibrato"));
                y += EditorGUIUtility.singleLineHeight + 2;
            }
            else if (type == AnimationType.PingPongX || type == AnimationType.PingPongY)
            {
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pingPongDistance"));
                y += EditorGUIUtility.singleLineHeight + 2;
            }
            else if (type == AnimationType.PressScale)
            {
                // Draw header
                EditorGUI.LabelField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), "Press Scale Settings", EditorStyles.boldLabel);
                y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("minScale"));
                y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("maxScale"));
                y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pressLoopCount"));
                y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pressDuration"));
                y += EditorGUIUtility.singleLineHeight + 2;
            }

            // Use Target B
            var useTargetB = element.FindPropertyRelative("useTargetB");
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), useTargetB);
            y += EditorGUIUtility.singleLineHeight + 2;

            if (useTargetB.boolValue)
            {
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("targetB"));
                y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("animationTypeB"));
                y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("durationB"));
                y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("delayB"));
                y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("loopB"));
                y += EditorGUIUtility.singleLineHeight + 2;

                // Extra fields for type B
                var animTypeB = element.FindPropertyRelative("animationTypeB");
                var typeB = (AnimationType)animTypeB.enumValueIndex;
                if (typeB == AnimationType.ShakeHorizontal || typeB == AnimationType.ShakeVertical)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shakeStrengthB"));
                    y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shakeVibratoB"));
                    y += EditorGUIUtility.singleLineHeight + 2;
                }
                else if (typeB == AnimationType.PingPongX || typeB == AnimationType.PingPongY)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pingPongDistanceB"));
                    y += EditorGUIUtility.singleLineHeight + 2;
                }
                else if (typeB == AnimationType.PressScale)
                {
                    EditorGUI.LabelField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), "Press Scale Settings (B)", EditorStyles.boldLabel);
                    y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("minScale"));
                    y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("maxScale"));
                    y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pressLoopCount"));
                    y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pressDuration"));
                    y += EditorGUIUtility.singleLineHeight + 2;
                }
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }



    //private SerializedProperty effectItems;
    //private ReorderableList reorderableList;

    //private void OnEnable()
    //{
    //    effectItems = serializedObject.FindProperty("effectItems");

    //    reorderableList = new ReorderableList(serializedObject, effectItems, true, true, true, true);

    //    reorderableList.drawHeaderCallback = (Rect rect) =>
    //    {
    //        EditorGUI.LabelField(rect, "UI Effect Items");
    //    };

    //    reorderableList.elementHeightCallback = (int index) =>
    //    {
    //        var element = effectItems.GetArrayElementAtIndex(index);
    //        float height = EditorGUIUtility.singleLineHeight * 8f;

    //        var animType = element.FindPropertyRelative("animationType");
    //        var type = (AnimationType)animType.enumValueIndex;
    //        if (type == AnimationType.ShakeHorizontal || type == AnimationType.ShakeVertical)
    //            height += EditorGUIUtility.singleLineHeight * 2;
    //        else if (type == AnimationType.PingPongX || type == AnimationType.PingPongY)
    //            height += EditorGUIUtility.singleLineHeight;

    //        var useB = element.FindPropertyRelative("useTargetB");
    //        if (useB.boolValue)
    //        {
    //            height += EditorGUIUtility.singleLineHeight * 6f;
    //            var animTypeB = element.FindPropertyRelative("animationTypeB");
    //            var typeB = (AnimationType)animTypeB.enumValueIndex;
    //            if (typeB == AnimationType.ShakeHorizontal || typeB == AnimationType.ShakeVertical)
    //                height += EditorGUIUtility.singleLineHeight * 2;
    //            else if (typeB == AnimationType.PingPongX || typeB == AnimationType.PingPongY)
    //                height += EditorGUIUtility.singleLineHeight;
    //        }
    //        return height + 10f;
    //    };

    //    reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
    //    {
    //        var element = effectItems.GetArrayElementAtIndex(index);
    //        rect.y += 2;
    //        float y = rect.y;

    //        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("target"));
    //        y += EditorGUIUtility.singleLineHeight + 2;

    //        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("playMode"));
    //        y += EditorGUIUtility.singleLineHeight + 2;

    //        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("animationType"));
    //        y += EditorGUIUtility.singleLineHeight + 2;

    //        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("duration"));
    //        y += EditorGUIUtility.singleLineHeight + 2;

    //        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("delay"));
    //        y += EditorGUIUtility.singleLineHeight + 2;

    //        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("loop"));
    //        y += EditorGUIUtility.singleLineHeight + 2;

    //        var animType = element.FindPropertyRelative("animationType");
    //        var type = (AnimationType)animType.enumValueIndex;
    //        if (type == AnimationType.ShakeHorizontal || type == AnimationType.ShakeVertical)
    //        {
    //            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shakeStrength"));
    //            y += EditorGUIUtility.singleLineHeight + 2;
    //            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shakeVibrato"));
    //            y += EditorGUIUtility.singleLineHeight + 2;
    //        }
    //        else if (type == AnimationType.PingPongX || type == AnimationType.PingPongY)
    //        {
    //            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pingPongDistance"));
    //            y += EditorGUIUtility.singleLineHeight + 2;
    //        }

    //        var useTargetB = element.FindPropertyRelative("useTargetB");
    //        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), useTargetB);
    //        y += EditorGUIUtility.singleLineHeight + 2;

    //        if (useTargetB.boolValue)
    //        {
    //            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("targetB"));
    //            y += EditorGUIUtility.singleLineHeight + 2;

    //            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("animationTypeB"));
    //            y += EditorGUIUtility.singleLineHeight + 2;

    //            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("durationB"));
    //            y += EditorGUIUtility.singleLineHeight + 2;

    //            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("delayB"));
    //            y += EditorGUIUtility.singleLineHeight + 2;

    //            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("loopB"));
    //            y += EditorGUIUtility.singleLineHeight + 2;

    //            var animTypeB = element.FindPropertyRelative("animationTypeB");
    //            var typeB = (AnimationType)animTypeB.enumValueIndex;
    //            if (typeB == AnimationType.ShakeHorizontal || typeB == AnimationType.ShakeVertical)
    //            {
    //                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shakeStrengthB"));
    //                y += EditorGUIUtility.singleLineHeight + 2;
    //                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shakeVibratoB"));
    //                y += EditorGUIUtility.singleLineHeight + 2;
    //            }
    //            else if (typeB == AnimationType.PingPongX || typeB == AnimationType.PingPongY)
    //            {
    //                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pingPongDistanceB"));
    //                y += EditorGUIUtility.singleLineHeight + 2;
    //            }
    //        }
    //    };
    //}

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();
    //    reorderableList.DoLayoutList();
    //    serializedObject.ApplyModifiedProperties();
    //}
}
