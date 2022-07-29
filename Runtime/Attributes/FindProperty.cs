using System;

namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class FindProperty : Attribute
    {
        private string customPath;

        public FindProperty() { }

        public FindProperty(string customPath)
        {
            this.customPath = customPath;
        }
    }
}
