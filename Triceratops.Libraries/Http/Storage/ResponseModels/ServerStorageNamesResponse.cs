namespace Triceratops.Libraries.Http.Storage.ResponseModels
{
    public class ServerStorageNamesResponse
    {
        public string[] ServerNames { get; set; }

        public static ServerStorageNamesResponse Empty => new ServerStorageNamesResponse { ServerNames = new string[0] };
    }
}
