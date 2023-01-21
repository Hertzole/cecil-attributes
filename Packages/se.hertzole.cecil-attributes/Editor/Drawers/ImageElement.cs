using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.CecilAttributes.Editor
{
	internal sealed class ImageElement : VisualElement
	{
		private Texture2D image;

		public Texture2D Image
		{
			get { return image; }
			set
			{
				image = value;
				style.backgroundImage = value;
			}
		}

		public ImageElement(Texture2D image)
		{
			Image = image;
			Setup();
		}

		private void Setup()
		{
			style.width = EditorGUIUtility.singleLineHeight - 2;
			style.height = EditorGUIUtility.singleLineHeight - 2;
			style.minWidth = EditorGUIUtility.singleLineHeight - 2;
#if UNITY_2022_2_OR_NEWER
			style.backgroundRepeat = new StyleBackgroundRepeat(new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat));
			style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Center));
			style.backgroundPositionY = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Center));
			style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(BackgroundSizeType.Cover));
#else
			style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
#endif
		}
	}
}