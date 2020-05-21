using System.Text.Json;

namespace Triceratops.VolumeInspector.Models.ResponseModels
{
    public class VolumeInspectorResponseModel
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public string ObjectJson { get; set; }

        public VolumeInspectorResponseModel()
        {
        }

        protected VolumeInspectorResponseModel(bool success, string error, object responseObject = null)
        {
            Success = success;
            Message = error;

            if (responseObject != null)
            {
                ObjectJson = JsonSerializer.Serialize(responseObject);
            }
        }

        public T DeserialiseData<T>()
        {
            if (ObjectJson == null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(ObjectJson);
        }

        public static VolumeInspectorResponseModel CreateForObject(object responseObject)
        {
            return new VolumeInspectorResponseModel(true, null, responseObject);
        }

        public static VolumeInspectorResponseModel CreateForError(string error)
        {
            return new VolumeInspectorResponseModel(false, error);
        }

        public static VolumeInspectorResponseModel CreateWithMessage(string message)
        {
            return new VolumeInspectorResponseModel(true, message);
        }
    }
}
