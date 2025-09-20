using TechChallenge.Games.Endpoints;
using TechChallenge.Games.Web.Configurations;
using TechChallenge.Games.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configurações customizadas
builder.AddLogConfiguration();
builder.AddApiConfiguration();
builder.AddInfrastructureConfiguration();
builder.AddApplicationConfiguration();
builder.AddAuthenticationConfiguration();
builder.AddAuthorizationConfiguration();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

var app = builder.Build();

app.UseErrorLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapJogoEndpoints();

app.Run();