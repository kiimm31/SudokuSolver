using Microsoft.OpenApi.Models;
using SudokuSolver.Api.Middleware;
using SudokuSolver.Api.Services;
using SudokuSolver.Core.Factories;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS for React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sudoku Solver API",
        Version = "v1",
        Description = "A comprehensive API for solving, validating, and generating Sudoku puzzles",
        Contact = new OpenApiContact
        {
            Name = "Sudoku Solver Team",
            Email = "support@sudokusolver.com"
        }
    });

    // Add XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register Sudoku services
builder.Services.AddScoped<ISudokuSolverFactory, SudokuSolver.Api.Services.SudokuSolverFactory>();
builder.Services.AddScoped<PuzzleGenerationService>();

// Add logging
builder.Services.AddLogging();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sudoku Solver API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Use global exception handler
app.UseGlobalExceptionHandler();

app.UseCors("AllowReactApp");

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapHealthChecks("/health");

app.Run();
