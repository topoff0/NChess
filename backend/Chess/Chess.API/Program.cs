using Chess.API.Implementations;
using Chess.API.Interfaces;
using Chess.API.Extensions;
using Chess.Application;
using Chess.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IMovement, Movement>();

builder.Services.AddOpenApi();

builder.Services.AddCustomSwagger();

// ======================= APP ======================= 
var app = builder.Build();

app.UseCustomSwagger(app.Environment);

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
