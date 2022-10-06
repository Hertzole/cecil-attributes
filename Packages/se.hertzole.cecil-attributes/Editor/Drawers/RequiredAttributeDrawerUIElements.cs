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
				iconElement = new VisualElement
				{
					style =
					{
						width = EditorGUIUtility.singleLineHeight - 2,
						height = EditorGUIUtility.singleLineHeight - 2,
						backgroundImage = new StyleBackground((Texture2D) (property.objectReferenceValue != null ? checkIcon.image : errorIcon.image)),
						unityBackgroundScaleMode = ScaleMode.ScaleAndCrop,
						marginLeft = 3,
						marginTop = 2
					}
				};

				root.Add(iconElement);
			}

			field.RegisterValueChangeCallback(ctx =>
			{
				if (iconElement == null)
				{
					return;
				}

				iconElement.style.backgroundImage = new StyleBackground((Texture2D) (ctx.changedProperty.objectReferenceValue != null ? checkIcon.image : errorIcon.image));
			});

			return root;
		}
	}
}
#endif