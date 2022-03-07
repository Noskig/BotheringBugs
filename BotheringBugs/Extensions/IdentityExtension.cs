using System.Security.Claims;
using System.Security.Principal;

namespace BotheringBugs.Extensions
{
    public static class IdentityExtension
    {
        public static int? GetCompanyId(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst("CompanyId");
            //Ternary operatr(if/else)
            return (claim != null) ? int.Parse(claim.Value) : null;

        }
    }
}
