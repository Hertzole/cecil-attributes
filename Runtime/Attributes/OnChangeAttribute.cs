using System;

namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class OnChangeAttribute : Attribute
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public string hook;
        public bool equalCheck = true;

        //public OnChangeAttribute(string hook, bool equalCheck = true)
        //{
        //    this.hook = hook;
        //}
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
