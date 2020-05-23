using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Api.Services.DockerService.Enums;

namespace Triceratops.Api.Services.DockerService.Models
{
    public class DockerFilterCollection
    {
        private readonly Dictionary<DockerFilterField, string> _filterFieldMap
            = new Dictionary<DockerFilterField, string>
            {
                [DockerFilterField.Label] = "label"
            };

        private readonly Dictionary<DockerFilterField, List<string>> _filters
            = new Dictionary<DockerFilterField, List<string>>();

        public void RequiresFilter(DockerFilterField field, string value)
        {
            if (!ContainersFilter(field, value))
            {
                AddFilter(field, value);
            }
        }

        public void RequiresLabel(string value)
        {
            RequiresFilter(DockerFilterField.Label, value);
        }

        public void AddFilter(DockerFilterField field, string value)
        {
            AddValue(field, value);
        }

        public void AddLabelFilter(string value)
        {
            AddValue(DockerFilterField.Label, value);            
        }

        public bool ContainersFilter(DockerFilterField field, string value)
        {
            if (!_filters.ContainsKey(field))
            {
                return false;
            }

            return _filters[field].Contains(value);
        }

        public void LogContents(ILogger logger)
        {
            foreach (var (field, values) in _filters)
            {
                using var filterScope = logger.BeginScope($"Filter {GetDockerFieldName(field)}");

                if (!values.Any())
                {
                    logger.LogInformation("No values stored for this filter!");
                }
                else
                {
                    foreach (var value in values)
                    {
                        logger.LogInformation($"Value: {value}");
                    }
                }                
            }
        }

        public IDictionary<string, IDictionary<string, bool>> ToDictionary()
        {
            var dictionary = new Dictionary<string, IDictionary<string, bool>>();

            foreach (var filter in _filters)
            {
                var fieldName = GetDockerFieldName(filter.Key);
                var valueDictionary = filter.Value.ToDictionary(v => v, v => true);

                dictionary.Add(fieldName, valueDictionary);

            }

            return dictionary;
        }

        private void AddValue(DockerFilterField field, string searchValue)
        {
            if (!_filters.ContainsKey(field))
            {
                _filters.Add(field, new List<string>(new[]
                {
                    searchValue
                }));

                return;
            }

            if (_filters[field].Contains(searchValue))
            {
                // This value already exists...

                return;
            }

            _filters[field].Add(searchValue);
        }

        private string GetDockerFieldName(DockerFilterField field)
        {
            if (_filterFieldMap.TryGetValue(field, out var dockerField))
            {
                return dockerField;
            }

            throw new Exception($"Unrecognised docker filter field: {field}");
        }

        public static DockerFilterCollection Build(params (DockerFilterField field, string value)[] filters)
        {
            var collection = new DockerFilterCollection();

            foreach (var (field, value) in filters)
            {
                collection.AddFilter(field, value);
            }

            return collection;
        }
    }
}
