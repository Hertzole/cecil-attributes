using System;

namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MarkInProfilerAttribute : Attribute { }
}
