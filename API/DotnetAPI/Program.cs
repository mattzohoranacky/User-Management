using DotnetAPI.Controllers;
using DotnetAPI.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(jwtOptions =>
{
    jwtOptions.RequireHttpsMetadata=false;
	jwtOptions.Authority = "http://localhost:7210";
	jwtOptions.Audience = "https://localhost:5152";
});

builder.Services.AddAuthentication()
.AddJwtBearer("some-scheme", jwtOptions =>
{
	jwtOptions.MetadataAddress = builder.Configuration["Api:MetadataAddress"];
	// Optional if the MetadataAddress is specified
	jwtOptions.Authority = builder.Configuration["Api:Authority"];
	jwtOptions.Audience = builder.Configuration["Api:Audience"];
	jwtOptions.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateIssuerSigningKey = true,
		ValidAudiences = builder.Configuration.GetSection("Api:ValidAudiences").Get<string[]>(),
		ValidIssuers = builder.Configuration.GetSection("Api:ValidIssuers").Get<string[]>()
	};

	jwtOptions.MapInboundClaims = false;
});

var requireAuthPolicy = new AuthorizationPolicyBuilder()
	.RequireAuthenticatedUser()
	.Build();

builder.Services.AddAuthorizationBuilder()
	.SetFallbackPolicy(requireAuthPolicy); // require authorization by default

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(jwtOptions =>
{
    jwtOptions.RequireHttpsMetadata=false;
	jwtOptions.Authority = "http://localhost:7210";
	jwtOptions.Audience = "https://localhost:5152";
});

builder.Services.AddAuthentication()
.AddJwtBearer("some-scheme", jwtOptions =>
{
	jwtOptions.MetadataAddress = builder.Configuration["Api:MetadataAddress"];
	// Optional if the MetadataAddress is specified
	jwtOptions.Authority = builder.Configuration["Api:Authority"];
	jwtOptions.Audience = builder.Configuration["Api:Audience"];
	jwtOptions.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateIssuerSigningKey = true,
		ValidAudiences = builder.Configuration.GetSection("Api:ValidAudiences").Get<string[]>(),
		ValidIssuers = builder.Configuration.GetSection("Api:ValidIssuers").Get<string[]>()
	};

	jwtOptions.MapInboundClaims = false;
});

var requireAuthPolicy = new AuthorizationPolicyBuilder()
	.RequireAuthenticatedUser()
	.Build();

builder.Services.AddAuthorizationBuilder()
	.SetFallbackPolicy(requireAuthPolicy); // require authorization by default

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