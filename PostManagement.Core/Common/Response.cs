public class Response<T>
{
    public Response() {

    }
    public Response(T data)
    {
        Data = data;
        Success = true;
    }
    public T Data { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}