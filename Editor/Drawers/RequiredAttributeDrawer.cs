using UnityEditor;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
	[CustomPropertyDrawer(typeof(RequiredAttribute))]
	public sealed partial class RequiredAttributeDrawer : PropertyDrawer
	{
		private static GUIContent errorIcon;
		private static GUIContent checkIcon;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GetIcons();

			bool hasObject = property.objectReferenceValue != null;
			Rect r = position;
			r.width -= EditorGUIUtility.singleLineHeight;

			bool oEnabled = GUI.enabled;

			GUI.enabled = true;
			EditorGUI.LabelField(new Rect(r.x + r.width, r.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), hasObject ? checkIcon : errorIcon);
			GUI.enabled = oEnabled;
			EditorGUI.PropertyField(r, property, label);
		}

		private static void GetIcons()
		{
			if (errorIcon == null)
			{
				errorIcon = EditorGUIUtility.IconContent("Error@2x", "|This field is required.");
			}

			if (checkIcon == null)
			{
				checkIcon = EditorGUIUtility.IconContent("Installed@2x", "|This field has it's required object.");
			}
		}
	}
}