using Chess.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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


