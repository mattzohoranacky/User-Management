using DotnetAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

var key = Encoding.ASCII.GetBytes("a_totally_legitimate_key_to_use_here");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Should be true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true, // Set to true in production
        ValidIssuer = "http://localhost:5152/auth/token",
        ValidateAudience = false,
        ValidAudience = "API user",
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
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

app.MapSwagger().AllowAnonymous();

app.UseHttpsRedirection();

app.Run();