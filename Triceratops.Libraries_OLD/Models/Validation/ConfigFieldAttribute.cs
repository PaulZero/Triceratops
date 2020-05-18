using System;

namespace Triceratops.Libraries.Models.Validation
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ConfigFieldAttribute : Attribute
    {
        public string GroupName { get; }

        public ConfigFieldAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
