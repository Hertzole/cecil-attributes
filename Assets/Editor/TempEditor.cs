using Hertzole.CecilAttributes;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(Temp))]
public class TempEditor : Editor
{
    [FindProperty]
    private SerializedProperty test;
    [FindProperty]
    private SerializedProperty lel;
    [FindProperty("cantRememberMe/lol")]
    private SerializedProperty test2;
    [FindProperty("customName")]
    private SerializedProperty thatName;

    [FindProperty("test")]
    private SerializedProperty Test { get; set; }

    private SerializedProperty template1;
    private SerializedProperty template2;
    private SerializedProperty template3;
    private SerializedProperty TemplateProperty { get; set; }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(test);
        EditorGUILayout.PropertyField(lel);
        EditorGUILayout.PropertyField(test2);
        EditorGUILayout.PropertyField(thatName);

        serializedObject.ApplyModifiedProperties();
    }

    //private void OnEnable()
    //{

    //}

    private void OnEnable()
    {
        Debug.Log("I'm a farthead");
    }

    //private void OnEnable()
    //{
    //    if (serializedObject.FindProperty("fuck") == null)
    //    {
    //        Debug.LogError($"There's no property called 'fuck' on {target}.");
    //        return;
    //    }

    //    if (serializedObject.FindProperty("fuck").FindPropertyRelative("fuck2") == null)
    //    {
    //        Debug.LogError($"There's no property called 'fuck2' on {target}.");
    //        return;
    //    }
    //    template1 = serializedObject.FindProperty("test");
    //    template2 = serializedObject.FindProperty("lel");
    //    template3 = serializedObject.FindProperty("lel").FindPropertyRelative("lol");
    //    TemplateProperty = serializedObject.FindProperty("test");
    //}
}
