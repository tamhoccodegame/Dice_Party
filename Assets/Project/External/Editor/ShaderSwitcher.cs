#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public class ShaderSwitcher : Editor
{
    [MenuItem("Tools/Switch Shader of Selected Material(s) to NewToonShader")]
    static void ChangeShader()
    {
        // Lấy material đang được chọn trong tab Project
        Object[] selectedObjects = Selection.objects;

        foreach (Object obj in selectedObjects)
        {
            Material mat = obj as Material;

            if (mat == null)
            {
                Debug.LogWarning("Bạn phải chọn 1 Material trong Project!");
                return;
            }

            Texture baseMap = null;

            if (mat.HasProperty("_BaseMap"))
                baseMap = mat.GetTexture("_BaseMap");

            // Ví dụ đổi sang Standard Shader
            Shader newShader = Shader.Find("Shader Graphs/NewToonShader");

            if (newShader == null)
            {
                Debug.LogWarning("Không tìm thấy shader!");
                return;
            }

            mat.shader = newShader;

            if (mat.HasProperty("_BaseMap"))
                mat.SetTexture("_BaseMap", baseMap);

            Debug.Log("Đã đổi shader cho: " + mat.name);
        }
    }

    [MenuItem("Tools/Switch Shader of Selected Material(s) to URP Lit Shader")]
    static void ChangeShader2()
    {
        // Lấy material đang được chọn trong tab Project
        Object[] selectedObjects = Selection.objects;

        foreach (Object obj in selectedObjects)
        {
            Material mat = obj as Material;

            if (mat == null)
            {
                Debug.LogWarning("Bạn phải chọn 1 Material trong Project!");
                return;
            }

            Texture baseMap = null;

            if (mat.HasProperty("_BaseMap"))
                baseMap = mat.GetTexture("_BaseMap");

            // Ví dụ đổi sang Standard Shader
            Shader newShader = Shader.Find("Universal Render Pipline/Lit");

            if (newShader == null)
            {
                Debug.LogWarning("Không tìm thấy shader!");
                return;
            }

            mat.shader = newShader;

           
        }
    }
}

#endif