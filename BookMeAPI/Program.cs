var builder = WebApplication.CreateBuilder(args);

// Add secrets configuration in development
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true);
}

// Configure services
builder.ConfigureServices();

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigureRequestPipeline();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
