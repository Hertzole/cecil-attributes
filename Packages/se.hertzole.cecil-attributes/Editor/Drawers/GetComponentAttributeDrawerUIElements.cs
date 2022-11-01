#if UNITY_2020_3_OR_NEWER
using Hertzole.CecilAttributes.Interfaces;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.CecilAttributes.Editor
{
	partial class GetComponentAttributeDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			GetIcon();
			
			bool enable = false;

			if (attribute is GetComponentAttribute att)
			{
				enable = att.enableInEditor;
			}

			VisualElement root = new VisualElement
			{
				style =
				{
					flexDirection = FlexDirection.Row
				}
			};

			PropertyField field = new PropertyField(property)
			{
				style =
				{
					flexGrow = 1,
					flexShrink = 0
				}
			};

			field.Bind(property.serializedObject);

			field.SetEnabled(enable);

			root.Add(field);

			VisualElement iconElement = new VisualElement
			{
				style =
				{
					width = EditorGUIUtility.singleLineHeight - 2,
					height = EditorGUIUtility.singleLineHeight - 2,
					backgroundImage = new StyleBackground((Texture2D) icon.image),
					unityBackgroundScaleMode = ScaleMode.ScaleAndCrop,
					marginLeft = 5,
					marginTop = 2
				},
				tooltip = icon.tooltip
			};

			root.Add(iconElement);
			
#if !CECIL_ATTRIBUTES_EXPERIMENTAL_GETCOMPONENT
			field.RegisterValueChangeCallback(evt =>
			{
				if (evt.changedProperty.serializedObject.targetObject is IGetComponent comp)
				{
					comp.FetchComponents();
				}
			});
#endif
			
			return root;
		}
	}
}
#endif