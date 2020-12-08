using System;

namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class TimedAttribute : Attribute { }
}
