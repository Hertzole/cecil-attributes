using System;
using UnityEngine;

namespace Hertzole.CecilAttributes
{
    public enum GetComponentTarget { Self = 0, Parent = 1, Children = 2 }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class GetComponentAttribute : PropertyAttribute
    {
        public GetComponentTarget target = GetComponentTarget.Self;
        public bool includeInactive = true;
        [Obsolete("Use 'enableInEditor' instead.")]
        public bool showInInspector { get { return enableInEditor; } set { enableInEditor = value; } }
        public bool enableInEditor = false;
    }
}
