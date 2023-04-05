using System;
using UnityEngine;

namespace Hertzole.CecilAttributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class RequiredAttribute : PropertyAttribute { }
}