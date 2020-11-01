using System;

namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class OnChangeAttribute : Attribute
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public OnChangeAttribute(string hook, bool equalCheck = true) { }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
