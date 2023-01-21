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
				name = $"get-component-attribute-drawer-{property.name}",
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
					flexShrink = 1
				}
			};

			field.Bind(property.serializedObject);

			field.SetEnabled(enable);

			root.Add(field);

			ImageElement iconElement = new ImageElement((Texture2D) icon.image)
			{
				style =
				{
					marginLeft = 5,
					marginTop = 2
				},
				tooltip = icon.tooltip
			};

			root.Add(iconElement);

			field.RegisterCallback<GeometryChangedEvent, VisualElement>((evt, args) => { args.RemoveDecorators(); }, field);

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