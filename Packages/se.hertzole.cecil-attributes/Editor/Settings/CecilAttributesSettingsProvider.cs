using UnityEditor;

namespace Hertzole.CecilAttributes.Editor
{
    internal static class CecilAttributesSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new SettingsProvider("Hertzole/Cecil Attributes", SettingsScope.Project)
            {
                label = "Cecil Attributes",
                guiHandler = ctx => CecilAttributesSettingsGUI.OnGUI(),
                keywords = new string[] { "hertzole", "cecil", "attribute", "attributes", "mono", "reset", "static", "log", "called" }
            };
        }
    }
}
