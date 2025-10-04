using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TechChallenge.Games.Web.Configurations
{
    public static class AuthConfiguration
    {
        public static void AddAuthenticationConfiguration(this WebApplicationBuilder builder)
        {
            var key = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Chave JWT não localizada na configuração.");
            var issuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Issuer JWT não localizado na configuração.");
            
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });
        }

        public static void AddAuthorizationConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrador", p => p.RequireRole("Administrador"));
            });
        }
    }
}
