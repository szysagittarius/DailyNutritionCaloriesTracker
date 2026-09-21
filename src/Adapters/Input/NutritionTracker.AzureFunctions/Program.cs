using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NutritionTracker.Application;
using NutritionTracker.AzureTableStorage.DI;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Use the data connection string for the app's table storage.
// AzureWebJobsStorage is for the Functions runtime itself and does not hold the app's business tables.
var connectionString = builder.Configuration.GetValue<string>("AzureTableStorageConnectionString")
    ?? builder.Configuration.GetValue<string>("AzureWebJobsStorage")
    ?? "UseDevelopmentStorage=true"; // Use Azurite for local development

// Add hexagonal architecture layers
builder.Services.AddApplication();  // Application layer (use cases)
builder.Services.AddAzureTableStorage(connectionString);  // Output adapter (Table Storage)

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
