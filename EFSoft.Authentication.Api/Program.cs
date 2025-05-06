var builder = WebApplication.CreateBuilder(args);

// --- Configuration ---
// Configure Identity database connection string
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnectionString");
if (string.IsNullOrEmpty(identityConnectionString))
{
    throw new InvalidOperationException("Connection string 'IdentityConnectionString' not found.");
}

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
{
    throw new InvalidOperationException("JWT Settings 'Secret' not found or configured incorrectly.");
}

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();
// Optional: Basic validation for email settings
if (string.IsNullOrEmpty(emailSettings?.SmtpHost) || string.IsNullOrEmpty(emailSettings.SmtpUser) || string.IsNullOrEmpty(emailSettings.SmtpPass) || string.IsNullOrEmpty(emailSettings.FromAddress))
{
    // Log a warning or throw if email sending is critical from startup
    builder.Services.AddSingleton<EFSoft.Authentication.Api.Services.IEmailSender, PlaceholderEmailSender>(); // Fallback to placeholder
    Console.WriteLine("WARNING: EmailSettings not fully configured in appsettings. Email sending will use a placeholder.");
}
else
{
    // --- Register the actual SMTP Email Sender ---
    builder.Services.AddScoped<EFSoft.Authentication.Api.Services.IEmailSender, SmtpEmailSender>();
    Console.WriteLine("EmailSettings configured. Using SmtpEmailSender.");
}

// Add DbContext for Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(identityConnectionString));

// Add Identity
// Configure Identity options if needed (e.g., password strength, lockout)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // Require email confirmation
                                                   // Add other password, lockout, user options here as needed
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true; // Example: relax constraints slightly
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Configures Identity to use EF Core
.AddDefaultTokenProviders(); // Adds token providers for password reset, email confirmation, etc.

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Validate the signing key
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)), // The secret key
        ValidateIssuer = true, // Validate the issuer
        ValidIssuer = jwtSettings.Issuer, // The valid issuer
        ValidateAudience = true, // Validate the audience
        ValidAudience = jwtSettings.Audience, // The valid audience
        ValidateLifetime = true, // Validate the token's lifetime (expiration)
        ClockSkew = TimeSpan.Zero // Remove default 5 minute skew
    };
});

// Add Authorization (needed to use [Authorize] attributes later if you expand scope)
builder.Services.AddAuthorization();

// Add Controllers for Web API endpoints
builder.Services.AddControllers();

// Add Swagger/OpenAPI for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddScoped<JwtTokenService>();

// Add a placeholder for email sending service
//builder.Services.AddScoped<EFSoft.Authentication.Api.Services.IEmailSender, PlaceholderEmailSender>();

var app = builder.Build();

// --- HTTP Request Pipeline Configuration ---

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use Swagger in Development for easy testing
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ApplyMigrations();

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Enable routing
app.UseRouting();

// Enable Authentication and Authorization middleware
app.UseAuthentication(); // This middleware checks for a valid token/cookie
app.UseAuthorization();  // This middleware checks if the authenticated user has permission

// Map controller endpoints
app.MapControllers();

// Run the application
app.Run();