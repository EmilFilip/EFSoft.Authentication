//namespace EFSoft.Authentication.Api.Controllers;

//[Authorize]
//[Route("api/[controller]")]
//[ApiController]
//public class AuthenticateController(IJwtAuthenticationManager jwtAuthenticationManager) : ControllerBase
//{
//    [AllowAnonymous]
//    [HttpPost("login")]
//    public IActionResult Login([FromBody] UserCredentials userCredentials)
//    {
//        var token = jwtAuthenticationManager.Authenticate(userCredentials.UserName, userCredentials.Password);

//        if (string.IsNullOrWhiteSpace(token))
//        {
//            return Unauthorized();
//        }

//        return Ok(token);
//    }
//}