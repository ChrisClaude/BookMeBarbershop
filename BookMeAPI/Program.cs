using BookMeAPI.Apis;

var builder = WebApplication.CreateBuilder(args);

// Add secrets configuration in development
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true);
}

// Configure services
var app = builder.ConfigureServices();

// Configure the HTTP request pipeline
app.ConfigureRequestPipeline();

app.Run();
