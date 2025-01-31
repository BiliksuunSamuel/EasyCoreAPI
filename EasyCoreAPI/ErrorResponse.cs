namespace EasyCoreAPI;

public class ErrorResponse
{
    public string? Field { get; set; }
    public string? ErrorMessage { get; set; }

    public ErrorResponse(string field, string errorMessage)
    {
        Field = field;
        ErrorMessage = errorMessage;
    }
}