using Newtonsoft.Json.Linq;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace ITPortal.Core
{
    /// <summary>
    /// Specifies a description for a property or event.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class WereTypeInfoAttribute : Attribute
    {
        public string Type { get; private set; }

        public string Key { get; private set; }

        public string KeyName { get; private set; }

        public string Desc { get; private set; }

        public WereTypeInfoAttribute(string type, string key, string keyName, string desc)
        {
            Type = type;
            Key = key;
            KeyName = keyName;
            Desc = desc;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is WereTypeInfoAttribute other && other.Type == Type;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
