namespace EFSoft.Authentication.Api.Services;

public interface IJwtAuthenticationManager
{
    string Authenticate(
        string username,
        string password);
}

