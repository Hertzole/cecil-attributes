using System;

namespace Hertzole.CecilAttributes
{
    public enum GetComponentTarget { Self = 0, Parent = 1, Children = 2 }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class GetComponentAttribute : Attribute
    {
        public GetComponentTarget target = GetComponentTarget.Self;
    }
}
