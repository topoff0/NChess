using Account.API.Extensions;
using Account.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


var envFile = "dev.env";

DotNetEnv.Env.TraversePath().Load(envFile);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllersWithFilters();

builder.Services.AddOpenApi();

builder.Services.AddMySwagger();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMyCors();

// ======================= APP ======================= 
var app = builder.Build();

app.UseGlobalExceptionHandler();

app.UseMySwagger(app.Environment);

app.UseStaticFiles();

await app.Services.ApplyMigrationAsync();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();

