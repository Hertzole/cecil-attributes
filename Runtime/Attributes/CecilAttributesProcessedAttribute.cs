﻿using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Hertzole.CecilAttributes.Editor")]
[assembly: InternalsVisibleTo("Unity.Hertzole.CecilAttributes.CodeGen")]
namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class CecilAttributesProcessedAttribute : Attribute { }
}
