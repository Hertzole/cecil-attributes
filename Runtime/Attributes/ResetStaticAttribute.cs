using System;

namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class ResetStaticAttribute : Attribute
    {
        public ResetStaticAttribute() { }
    }
}
