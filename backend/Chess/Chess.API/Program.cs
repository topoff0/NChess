using Chess.API.Extensions;
using Chess.Application;
using Chess.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var envFile = "dev.env";

DotNetEnv.Env.TraversePath().Load(envFile);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

builder.Services.AddCustomSwagger();
builder.Services.AddCustomCors();

// ======================= APP ======================= 
var app = builder.Build();

app.UseCustomSwagger(app.Environment);

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
