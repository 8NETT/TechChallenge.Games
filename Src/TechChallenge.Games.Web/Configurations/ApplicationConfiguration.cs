using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Application.Security;
using TechChallenge.Games.Application.Services;

namespace TechChallenge.Games.Web.Configurations
{
    public static class ApplicationConfiguration
    {
        public static void AddApplicationConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddScoped<IJogoService, JogoService>();
        }
    }
}
