namespace EFSoft.Authentication.Api.Models;

// Standard response model for API errors
public class ApiErrorResponse
{
    public IEnumerable<string> Errors { get; set; } = new List<string>();

    public ApiErrorResponse(IEnumerable<string> errors)
    {
        Errors = errors;
    }

    public ApiErrorResponse(string error)
    {
        Errors = new List<string> { error };
    }
}
