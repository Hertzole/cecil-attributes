using System;

namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class LogCalledAttribute : Attribute
    {
        /// <summary>
        /// If true, a message will be logged when getting this property.<br></br>
        /// <b>Only valid for properties</b>
        /// </summary>
        public bool logPropertyGet = true;
        /// <summary>
        /// If true, a message will be logged when setting this property.<br></br>
        /// <b>Only valid for properties</b>
        /// </summary>
        public bool logPropertySet = true;

        /// <summary>
        /// Will log a message when this method/property is called.
        /// </summary>
        public LogCalledAttribute() { }
    }
}
