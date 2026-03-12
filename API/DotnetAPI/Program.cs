using DotnetAPI.Controllers;
using DotnetAPI.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<ProgramDbContext>(options =>
options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("InMemoryDb"))
);
builder.Services.AddSwaggerGen(c =>
{
    var XMLPath = AppDomain.CurrentDomain.BaseDirectory + "DotnetAPI.xml";
    if (File.Exists(XMLPath))
    {
        c.IncludeXmlComments(XMLPath);
    }
});

var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "DotnetAPI");
        config.RoutePrefix = string.Empty;
    });
}

app.MapSwagger();

app.UseHttpsRedirection();

app.Run();