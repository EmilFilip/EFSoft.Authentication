//namespace EFSoft.Authentication.Api.Services;

//public class JwtAuthenticationManager : IJwtAuthenticationManager
//{
//    private IDictionary<string, string> _users;
//    private readonly string _key;

//    public JwtAuthenticationManager(string key)
//    {
//        _key = key;
//        _users = new Dictionary<string, string>
//                {
//                    { "user1", "password1" },
//                    { "user2", "password2" }
//                };
//    }

//    public string Authenticate(string username, string password)
//    {
//        if (!_users.Any(u => u.Key == username && u.Value == password))
//        {
//            return string.Empty;
//        }

//        var tokenKey = Encoding.ASCII.GetBytes(_key);

//        var claims = new List<Claim>
//        {
//           new Claim(ClaimTypes.Name, username)
//        };

//        var signingCredentials = new SigningCredentials(
//                new SymmetricSecurityKey(tokenKey),
//                SecurityAlgorithms.HmacSha256Signature);

//        var jwt = new JwtSecurityToken(
//            claims: claims,
//            expires: DateTime.UtcNow.AddMinutes(15),
//            signingCredentials: signingCredentials);

//        return new JwtSecurityTokenHandler().WriteToken(jwt);
//    }
//}
