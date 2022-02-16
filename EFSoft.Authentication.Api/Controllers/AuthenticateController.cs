namespace EFSoft.Authentication.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{

    private readonly IJwtAuthenticationManager _jwtAuthenticationManager;

    public AuthenticateController(IJwtAuthenticationManager jwtAuthenticationManager)
    {
        _jwtAuthenticationManager = jwtAuthenticationManager;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserCredentials userCredentials)
    {
        var token = _jwtAuthenticationManager.Authenticate(userCredentials.UserName, userCredentials.Password);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized();
        }

        return Ok(token);
    }
}