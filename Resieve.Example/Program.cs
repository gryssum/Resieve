using Microsoft.EntityFrameworkCore;
using Resieve;
using Resieve.Example.Data;
using Resieve.Example.Entities;
using Resieve.Example.Repository;
using Resieve.Mappings.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Swagger and Resieve services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddTransient<ProductRepository>();
builder.Services.AddTransient<ProductAdvancedRepository>();

builder.Services.AddResieve();
builder.Services.AddTransient<IResieveCustomFilter<Product>, CustomTagFilter>(); 
builder.Services.AddResieveMappingsFromAssembly(typeof(ResieveMappingForProduct).Assembly);

var app = builder.Build();

// Set up database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Get the database context
        var context = services.GetRequiredService<AppDbContext>();
        
        // Apply migrations at startup
        context.Database.Migrate();
        
        // Seed data after migrations have been applied
        await DbSeeder.SeedDatabaseAsync(context);
        
        Console.WriteLine("Database migrations applied and seeding completed.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();