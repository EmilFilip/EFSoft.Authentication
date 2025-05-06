namespace EFSoft.Authentication.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // Base path will be /api/auth
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IEmailSender _emailSender; // For sending confirmation/reset emails
    private readonly ILogger<AuthController> _logger; // For logging

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenService jwtTokenService,
        IEmailSender emailSender,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _emailSender = emailSender;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost("register")] // POST /api/auth/register
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        // Basic model validation based on data annotations
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponse(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        // Create a new ApplicationUser instance
        var user = new ApplicationUser
        {
            UserName = model.Email, // Often set UserName the same as Email
            Email = model.Email,
            EmailConfirmed = false // Email is not confirmed initially
        };

        // Attempt to create the user using Identity's UserManager
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created successfully: {Email}", user.Email);

            // --- Email Confirmation Flow ---
            // Generate an email confirmation token for the newly created user
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // NOTE: Identity tokens are URL-safe Base64, usually don't *need* encoding,
            // but decoding might be needed when consuming them depending on where they are placed (e.g. URL query).
            // For this simple API, we don't need to encode here, but keep it in mind
            // var encodedToken = HttpUtility.UrlEncode(token); // Example if needed

            // TODO: Implement actual email sending
            // This is a placeholder. In a real app, you'd send an email
            // containing a link with the user ID and token:
            // e.g., "Click here to confirm your email: https://yourapp.com/confirm-email?userId={user.Id}&token={token}"
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking this link: [Link to confirmation endpoint/frontend with userId={user.Id}&token={token}]"); // Construct appropriate URL

            // Return success response with user details
            var response = new RegistrationResponse
            {
                UserId = user.Id,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed
            };
            return Ok(response);
        }
        else
        {
            // If user creation failed, return BadRequest with Identity errors
            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("User creation failed: {Errors}", string.Join("; ", errors));
            return BadRequest(new ApiErrorResponse(errors));
        }
    }

    /// <summary>
    /// Logs in an existing user and returns a JWT.
    /// </summary>
    [HttpPost("login")] // POST /api/auth/login
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        // Basic model validation
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponse(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        // Find the user by email
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            _logger.LogWarning("Login failed for {Email}: A user with this email address was not found.", model.Email);
            return Unauthorized(new ApiErrorResponse("Login failed: Email address not found."));
        }

        // Check if user exists and password is correct using SignInManager
        // This handles password hashing comparison and lockout checks
        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false); // 'false' means don't lock out on failure immediately

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in successfully: {Email}", model.Email);

            // Check if email is confirmed (if required by Identity options)
            if (_userManager.Options.SignIn.RequireConfirmedAccount && !user.EmailConfirmed)
            {
                _logger.LogWarning("Login failed for {Email}: Email not confirmed.", model.Email);
                return Unauthorized(new ApiErrorResponse("Email not confirmed. Please confirm your email address.")); // Or BadRequest, depending on desired response
            }

            // Generate JWT including Id claim
            var tokenString = await _jwtTokenService.GenerateToken(user);
            var expiresAt = _jwtTokenService.GetTokenExpiration();

            // Return the token
            var response = new LoginResponse
            {
                Token = tokenString,
                ExpiresAt = expiresAt
            };
            return Ok(response);
        }
        else if (result.IsNotAllowed)
        {
            _logger.LogWarning("Login failed for {Email}: Account not allowed (e.g., not confirmed).", model.Email);
            return Unauthorized(new ApiErrorResponse("Login failed: Account not allowed (e.g., email not confirmed)."));
        }
        else if (result.IsLockedOut)
        {
            _logger.LogWarning("Login failed for {Email}: Account locked out.", model.Email);
            return Unauthorized(new ApiErrorResponse("Account locked out. Please try again later."));
        }
        else // Failed (invalid password, user not found, etc.)
        {
            _logger.LogWarning("Login failed for {Email}: Invalid credentials or user not found.", model.Email);
            // Return generic error message to prevent user enumeration
            return Unauthorized(new ApiErrorResponse("Invalid credentials."));
        }
    }

    /// <summary>
    /// Initiates the password reset process by sending a token to the user's email.
    /// </summary>
    [HttpPost("forgot-password")] // POST /api/auth/forgot-password
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
    {
        // Basic model validation
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponse(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        // Find the user by email
        var user = await _userManager.FindByEmailAsync(model.Email);

        // IMPORTANT SECURITY NOTE: Always return a generic success message
        // regardless of whether the email exists or not to prevent user enumeration.
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            _logger.LogWarning("Forgot password requested for non-existent or unconfirmed email: {Email}", model.Email);
            // Return generic success to prevent user enumeration
            return Ok(new { message = "If a user with that email exists and is confirmed, a password reset link has been sent." });
        }

        // Generate a password reset token
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // var encodedToken = HttpUtility.UrlEncode(token); // Example if needed for URL

        // TODO: Implement actual email sending
        // Send an email with the reset link (including user ID or email and token)
        // e.g., "Click here to reset your password: https://yourapp.com/reset-password?email={user.Email}&token={token}"
        await _emailSender.SendEmailAsync(model.Email, "Reset your password",
            $"Please reset your password by clicking this link: [Link to reset endpoint/frontend with email={user.Email}&token={token}]"); // Construct appropriate URL

        _logger.LogInformation("Password reset token generated and email sent (placeholder) for {Email}", model.Email);

        // Return generic success message
        return Ok(new { message = "If a user with that email exists and is confirmed, a password reset link has been sent." });
    }

    /// <summary>
    /// Confirms the password reset using the token received via email.
    /// </summary>
    [HttpPost("reset-password")] // POST /api/auth/reset-password
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
    {
        // Basic model validation
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponse(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        // Find the user by email
        var user = await _userManager.FindByEmailAsync(model.Email);

        // Check if user exists
        if (user == null)
        {
            _logger.LogWarning("Password reset attempted for non-existent email: {Email}", model.Email);
            // Return generic error to prevent user enumeration
            return BadRequest(new ApiErrorResponse("Invalid request."));
        }

        // Identity tokens are URL-safe Base64, but if received via query string in a real app,
        // they might be automatically decoded by the framework, or you might need manual decoding here:
        // var decodedToken = HttpUtility.UrlDecode(model.Token); // Example

        // Attempt to reset the password using the user, token, and new password
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

        if (result.Succeeded)
        {
            _logger.LogInformation("Password reset successful for {Email}", model.Email);
            // Optionally, you might want to regenerate security stamps or sign out other sessions
            // await _userManager.UpdateSecurityStampAsync(user);

            return Ok(new { message = "Password reset successful." });
        }
        else
        {
            _logger.LogWarning("Password reset failed for {Email}: {Errors}", model.Email, string.Join("; ", result.Errors.Select(e => e.Description)));
            // Return errors from Identity (e.g., invalid token, password requirements not met)
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new ApiErrorResponse(errors));
        }
    }

    /// <summary>
    /// Confirms the user's email address using the token received via email.
    /// </summary>
    [HttpPost("confirm-email")] // POST /api/auth/confirm-email
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest model)
    {
        // Basic model validation
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponse(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        // Find the user by ID
        var user = await _userManager.FindByIdAsync(model.UserId);

        // Check if user exists
        if (user == null)
        {
            _logger.LogWarning("Email confirmation attempted for non-existent user ID: {UserId}", model.UserId);
            return BadRequest(new ApiErrorResponse("Invalid request."));
        }

        // Identity tokens are URL-safe Base64, but if received via query string in a real app,
        // they might be automatically decoded by the framework, or you might need manual decoding here:
        // var decodedToken = HttpUtility.UrlDecode(model.Token); // Example

        // Attempt to confirm the email using the user and token
        var result = await _userManager.ConfirmEmailAsync(user, model.Token);

        if (result.Succeeded)
        {
            _logger.LogInformation("Email confirmed successfully for user ID: {UserId}", model.UserId);
            return Ok(new { message = "Email confirmed successfully." });
        }
        else
        {
            _logger.LogWarning("Email confirmation failed for user ID: {UserId}: {Errors}", model.UserId, string.Join("; ", result.Errors.Select(e => e.Description)));
            // Return errors from Identity (e.g., invalid token)
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new ApiErrorResponse(errors));
        }
    }
}
