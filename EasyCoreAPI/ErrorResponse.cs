namespace EasyCoreAPI;

public class ErrorResponse
{
    public string? Field { get; set; }
    public string? ErrorMessage { get; set; }

    public ErrorResponse()
    {
        
    }

    public ErrorResponse(string field, string errorMessage)
    {
        field = field;
        errorMessage = errorMessage;
    }
}