using Account.API.Extensions;
using Account.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

//TODO: Replace with "scalal"
builder.Services.AddMySwagger();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMyCors();


// ======================= APP ======================= 
var app = builder.Build();

await app.Services.ApplyMigrationAsync();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMySwagger(app.Environment);

app.MapControllers();

app.Run();

