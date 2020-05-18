namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ApiModelResponse<T>
        where T : class, new()
    {
        public T Model { get; set; }

        public ApiModelResponse(T model)
        {
            Model = model;
        }
    }
}
