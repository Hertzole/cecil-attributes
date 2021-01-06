using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Hertzole.CecilAttributes.Editor")]
namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class CecilAttributesProcessedAttribute : Attribute { }
}
