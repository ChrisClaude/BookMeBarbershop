using BookMeAPI;

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
