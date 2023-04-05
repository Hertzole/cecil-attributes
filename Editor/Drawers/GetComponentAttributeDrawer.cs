using Hertzole.CecilAttributes.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
	[CustomPropertyDrawer(typeof(GetComponentAttribute), true)]
	public sealed partial class GetComponentAttributeDrawer : PropertyDrawer
	{
		private static GUIContent icon;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GetIcon();

			Rect r = position;
			r.width -= 20;

			bool enable = false;

			if (attribute is GetComponentAttribute att)
			{
				enable = att.enableInEditor;
			}

			bool oEnabled = GUI.enabled;
			GUI.enabled = enable;
			EditorGUI.PropertyField(r, property, label);
			GUI.enabled = oEnabled;
			EditorGUI.LabelField(new Rect(r.x + r.width + 2, r.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), icon);

#if !CECIL_ATTRIBUTES_EXPERIMENTAL_GETCOMPONENT
			if (property.objectReferenceValue == null && property.serializedObject.targetObject is IGetComponent comp)
			{
				comp.FetchComponents();
			}
#endif
		}

		private static void GetIcon()
		{
			if (icon == null)
			{
				icon = EditorGUIUtility.IconContent("Refresh@2x", "|This component is automatically fetched in the editor.");
			}
		}
	}
}