using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models.Validation;

namespace Triceratops.Libraries.Models.ServerConfiguration
{
    public abstract class AbstractServerConfiguration
    {
        public const string DefaultGroupName = "General";

        [Required]
        [Display(Name = "Server Name", Order = -1000)]
        [ConfigField(DefaultGroupName)]
        public string ServerName { get; set; }

        [Required]
        [Port]
        [Display(Name = "Host Port", Description = "The port exposed by the server that players will use to connect.", Order = -999)]
        [ConfigField(DefaultGroupName)]
        public ushort HostPort { get; set; }

        [Range(1, 5)]
        public int Test { get; set; }
        
        [JsonIgnore]
        public abstract ushort ContainerPort { get; }

        [JsonIgnore]
        public abstract ServerType ServerType { get; }

        public abstract bool IsValid();

        public Dictionary<string, List<string>> GetEditableFields()
        {
            var fieldDictionary = new Dictionary<string, List<string>>();

            var properties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(p => p.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? 0);

            foreach (var property in properties)
            {
                var fieldAttribute = property.GetCustomAttribute<ConfigFieldAttribute>();

                if (fieldAttribute == null)
                {
                    continue;
                }

                if (!fieldDictionary.ContainsKey(fieldAttribute.GroupName))
                {
                    fieldDictionary[fieldAttribute.GroupName] = new List<string>();
                }

                fieldDictionary[fieldAttribute.GroupName].Add(property.Name);
            }

            return fieldDictionary;
        }
            
    }
}
