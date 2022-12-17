#if UNITY_2020_3_OR_NEWER
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.CecilAttributes.Editor
{
	partial class RequiredAttributeDrawer
	{
		private VisualElement iconElement;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			GetIcons();

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

			root.Add(field);

			if (iconElement == null)
			{
				bool hasObj = property.objectReferenceValue != null;

				iconElement = new VisualElement
				{
					style =
					{
						width = EditorGUIUtility.singleLineHeight - 2,
						height = EditorGUIUtility.singleLineHeight - 2,
						backgroundImage = new StyleBackground((Texture2D) (hasObj ? checkIcon.image : errorIcon.image)),
						unityBackgroundScaleMode = ScaleMode.ScaleAndCrop,
						marginLeft = 3,
						marginTop = 2
					},
					tooltip = hasObj ? checkIcon.tooltip : errorIcon.tooltip
				};

				root.Add(iconElement);
			}

			field.RegisterValueChangeCallback(ctx =>
			{
				if (iconElement == null)
				{
					return;
				}

				bool hasObj = ctx.changedProperty.objectReferenceValue != null;

				iconElement.style.backgroundImage = new StyleBackground((Texture2D) (hasObj ? checkIcon.image : errorIcon.image));
				iconElement.tooltip = hasObj ? checkIcon.tooltip : errorIcon.tooltip;
			});

			iconElement.style.opacity = 0;

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