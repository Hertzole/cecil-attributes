using System;

namespace Hertzole.CecilAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class LogCalledAttribute : Attribute
    {
        private bool logPropertyGet;
        private bool logPropertySet;

        /// <summary>
        /// Will log a message when this method/property is called.
        /// </summary>
        public LogCalledAttribute()
        {
            logPropertyGet = false;
            logPropertySet = false;
        }

        /// <summary>
        /// Will log a message when this method/property is called.
        /// <br/>
        /// These values only apply when putting the attribute on a property.
        /// </summary>
        /// <param name="logPropertyGet">If true, a message will be logged when getting this property.</param>
        /// <param name="logPropertySet">If true, a message will be logged when setting this property.</param>
        public LogCalledAttribute(bool logPropertyGet, bool logPropertySet)
        {
            this.logPropertyGet = logPropertyGet;
            this.logPropertySet = logPropertySet;
        }
    }
}
