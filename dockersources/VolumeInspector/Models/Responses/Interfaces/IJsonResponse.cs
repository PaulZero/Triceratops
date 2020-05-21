namespace Triceratops.VolumeInspector.Models.Responses.Interfaces
{
    internal interface IJsonResponse : IResponse
    {
        string Json { get; }
    }
}
