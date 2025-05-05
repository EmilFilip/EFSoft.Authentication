using EFSoft.Authentication.Api;
using EFSoft.Authentication.Api.Database;
using EFSoft.Authentication.Api.Extentsions;
using EFSoft.Authentication.Api.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllers();
var secret = builder.Configuration["JwtSettings:Secret"]!;

var key = Encoding.ASCII.GetBytes(secret);
builder.Services.AddAuthorization();
//builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme)
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme)
//(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddIdentityCore<User>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddApiEndpoints();
builder.Services.AddDbContext<ApplicationDbContext>(
options =>
{
    _ = options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnectionString"), sqlServeroptions =>
    {
        _ = sqlServeroptions.EnableRetryOnFailure();
    });
});
builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomUserClaimsPrincipalFactory>();
//builder.Services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManager(secret));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapIdentityApi<User>();
app.Run();