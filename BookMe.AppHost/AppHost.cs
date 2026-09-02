var builder = DistributedApplication.CreateBuilder(args);

// Add the BookMe API
var api = builder.AddProject<Projects.BookMeAPI>("bookme-api")
    .WithHttpsEndpoint(port: 6002, name: "https")
    .WithHttpEndpoint(port: 6003, name: "http");

// Add the BookMe UI (Next.js)
var ui = builder.AddNpmApp("bookme-ui", "../bookme-ui")
    .WithHttpEndpoint(port: 3000, env: "PORT")
    .WithExternalHttpEndpoints()
    .WithReference(api);

builder.Build().Run();
