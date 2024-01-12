using AutoMapper;
using EnityFrameworkRelationShip.Data;
using EnityFrameworkRelationShip.Extensions;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Repositories;
using EnityFrameworkRelationShip.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepositoryServices();
builder.Services.AddBusinessServices();

//builder.Services.AddControllers().AddJsonOptions(x =>
//{
//    // Handle loops correctly
//    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var myDbContext = services.GetRequiredService<DataContext>();

    // Run the seeding method
    await DatabaseSeeder.SeedDatabaseAsync(myDbContext);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
