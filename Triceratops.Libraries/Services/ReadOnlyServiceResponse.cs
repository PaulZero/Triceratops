using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Services.Interfaces;

namespace Triceratops.Libraries.Services
{
    public class ReadOnlyServiceResponse : IReadOnlyServiceResponse
    {
        public static ReadOnlyServiceResponse Successful => new ReadOnlyServiceResponse(true);

        public static ReadOnlyServiceResponse Failed => new ReadOnlyServiceResponse(false);

        public bool Success { get; protected set; }

        public IReadOnlyList<string> Errors => _errors.AsReadOnly();

        public IReadOnlyList<string> Warnings => _warnings.AsReadOnly();

        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _warnings = new List<string>();

        public ReadOnlyServiceResponse()
        {
        }

        public ReadOnlyServiceResponse(bool success, IEnumerable<string> errors = default, IEnumerable<string> warnings = default)
        {
            Success = success;

            if (errors?.Any() ?? false)
            {
                _errors.AddRange(errors);
            }

            if (warnings?.Any() ?? false)
            {
                _warnings.AddRange(warnings);
            }
        }

        public string ToJson()
        {
            return JsonHelper.Serialise(this);
        }

        public static ReadOnlyServiceResponse CreateFromError(string error)
        {
            return new ReadOnlyServiceResponse(false, new[] { error });
        }

        protected void AddError(string error)
        {
            _errors.Add(error);
        }

        protected void AddWarning(string warning)
        {
            _errors.Add(warning);
        }
    }
}
