using System.Collections;
using System.Collections.Generic;
using Hertzole.CecilAttributes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Boxes))]
public class BoxesEditor : Editor
{
    [FindProperty]
    private SerializedProperty anims;
    [FindProperty]
    private SerializedProperty ren;
    [FindProperty]
    private SerializedProperty boxes;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        root.Add(new PropertyField(anims));
        root.Add(new PropertyField(ren));
        root.Add(new PropertyField(boxes));
        
        return root;
    }
}
