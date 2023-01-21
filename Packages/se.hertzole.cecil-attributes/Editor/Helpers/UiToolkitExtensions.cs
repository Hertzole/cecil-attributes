using UnityEngine.UIElements;

namespace Hertzole.CecilAttributes.Editor
{
	internal static class UiToolkitExtensions
	{
		public static void MakeField<T>(this BaseField<T> element)
		{
			element.AddToClassList(BaseField<T>.alignedFieldUssClassName);
		}

		public static void RemoveDecorators(this VisualElement element)
		{
			VisualElement decorators = element.Q<VisualElement>(className: "unity-decorator-drawers-container");
			decorators?.RemoveFromHierarchy();
		}
	}
}