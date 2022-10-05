using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.CecilAttributes.Editor
{
	[CustomPropertyDrawer(typeof(RequiredAttribute))]
	public class RequiredAttributeDrawer : PropertyDrawer
	{
		private static GUIContent errorIcon;
		private static GUIContent checkIcon;

		private VisualElement iconElement;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GetIcons();

			bool hasObject = property.objectReferenceValue != null;
			Rect r = position;
			r.width -= EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(new Rect(r.x + r.width, r.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), hasObject ? checkIcon : errorIcon);
			EditorGUI.PropertyField(r, property, label);
		}

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