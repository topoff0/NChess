using Account.Data;
using Account.JWT.Services;
using Account.JWT.Configuration;
using Account.Services.Interfaces;
using Account.Services.Implementations;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add jwt configuration
builder.Services.AddJwtAuthentication(builder.Configuration);

// REGISTER DEPENDENCIES:
{
    // Jwt service
    builder.Services.AddScoped<IJwtService, JwtService>();

    // For actions with authentication
    builder.Services.AddScoped<IAccountService, AccountService>();

    // Email Service
    builder.Services.AddScoped<IEmailService, EmailService>();

    // Encryption service
    builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
}

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Account_Chess API", Version = "v1.0.0" });
});

var app = builder.Build();


var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (!Directory.Exists(wwwrootPath))
{
    Directory.CreateDirectory(wwwrootPath);
}

var imagesPath = Path.Combine(wwwrootPath, "images");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

// For users accounts images
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images")),
    RequestPath = "/images"
});



app.MapControllers();
app.Run();
