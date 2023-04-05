#if UNITY_2020_3_OR_NEWER
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.CecilAttributes.Editor
{
	partial class RequiredAttributeDrawer
	{
		private ImageElement iconElement;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			GetIcons();

			VisualElement root = new VisualElement
			{
				name = $"required-attribute-drawer-{property.name}",
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

			root.Add(field);
			
			bool hasObj = property.objectReferenceValue != null;
			iconElement = new ImageElement((Texture2D) (hasObj ? checkIcon.image : errorIcon.image))
			{
				name = "icon",
				style =
				{
					marginLeft = 3,
					marginTop = 2
				}
			};

			root.Add(iconElement);

			field.RegisterValueChangeCallback(ctx =>
			{
				if (iconElement == null)
				{
					return;
				}
			
				bool hasNewValue = ctx.changedProperty.objectReferenceValue != null;
			
				iconElement.Image = (Texture2D) (hasNewValue ? checkIcon.image : errorIcon.image);
				iconElement.tooltip = hasNewValue ? checkIcon.tooltip : errorIcon.tooltip;
			});
			
			field.RegisterCallback<GeometryChangedEvent, VisualElement>((evt, args) =>
			{
				args.RemoveDecorators();
			}, field);

			iconElement.style.opacity = 1;

			root.RegisterCallback<AttachToPanelEvent, VisualElement>(OnRootAttached, root);

			return root;
		}

		private void OnRootAttached(AttachToPanelEvent evt, VisualElement root)
		{
			UpdateIconState(IsParentDisabled(root));
			root.UnregisterCallback<AttachToPanelEvent, VisualElement>(OnRootAttached);
		}

		private void UpdateIconState(bool disabled)
		{
			if (!disabled)
			{
				iconElement.style.opacity = 1;
				return;
			}
			
			// If the parent is disabled, set the opacity of the icon to 2 to make it fully visible again.
			// But we need to wait for it to execute because of some UI toolkit stuff.
			iconElement.schedule.Execute(() => { iconElement.style.opacity = 2; });
		}

		private static bool IsParentDisabled(VisualElement target)
		{
			VisualElement parent = target;

			while (parent != null)
			{
				if (parent.ClassListContains("unity-disabled"))
				{
					return true;
				}

				parent = parent.parent;
			}

			return false;
		}
	}
}
#endif