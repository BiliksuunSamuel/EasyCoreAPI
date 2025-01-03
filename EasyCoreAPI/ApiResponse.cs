namespace EasyCoreAPI;

public class ApiResponse<T>
{
    public string? Message { get; set; }
    public T? Data { get; set; }
    public IEnumerable<ErrorResponse> Errors { get; set; }
    public int Code { get; set; }

    public ApiResponse()
    {
        Errors = new List<ErrorResponse>();
    }

    public ApiResponse(string message, T data, int code)
    {
        Message = message;
        Data = data;
        Code = code;
        Errors = new List<ErrorResponse>();
    }

    public ApiResponse(IEnumerable<ErrorResponse> errors, int code, string message)
    {
        Errors = errors;
        Code = code;
        Message = message;
    }
    
}