using System.Security.Claims;

namespace TechChallenge.Games.Web.Security;

public static class SecurityExtensions
{
    public static int GetId(this ClaimsPrincipal principal)
    {
        var claim = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (claim == null)
            throw new InvalidOperationException("Usuário não possui uma claim de identificação.");

        return int.Parse(claim.Value);
    }
}