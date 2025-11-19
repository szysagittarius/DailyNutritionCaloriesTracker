using NutritionTracker.Application;
using NutritionTracker.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDevServer",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Add hexagonal architecture layers
builder.Services.AddApplication();  // Application layer (use cases)
builder.Services.AddSqlServerPersistence(builder.Configuration);  // Output adapter (persistence)

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowViteDevServer");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
