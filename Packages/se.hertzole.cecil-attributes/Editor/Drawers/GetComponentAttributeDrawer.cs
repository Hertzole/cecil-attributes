using Hertzole.CecilAttributes.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
	[CustomPropertyDrawer(typeof(GetComponentAttribute), true)]
	public class GetComponentAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool show = true;
				
			if (attribute is GetComponentAttribute att)
			{
				show = att.showInInspector;
			}

			if (show)
			{
				bool oEnabled = GUI.enabled;
				GUI.enabled = false;
				EditorGUI.PropertyField(position, property, label);
				GUI.enabled = oEnabled;
			}

			if (property.objectReferenceValue == null && property.serializedObject.targetObject is IGetComponent comp)
			{
				comp.FetchComponents();
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			bool show = true;

			if (attribute is GetComponentAttribute att)
			{
				show = att.showInInspector;
			}

			if (show)
			{
				return base.GetPropertyHeight(property, label);
			}

			return -EditorGUIUtility.standardVerticalSpacing;
		}
	}
}