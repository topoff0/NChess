using Chess.API.Implementations;
using Chess.API.Interfaces;
using Chess.API.Extensions;
using Chess.API.Persistence.Repositories;
using Chess.API.Persistence.Repositories.Common;
using Chess.Core.Repositories;
using Chess.Core.Repositories.Common;
using Chess.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<GamesDbContext>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
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
