using BotheringBugs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BotheringBugs.Services.Factories
{
    public class BBUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<BBUser, IdentityRole>
    {
        public BBUserClaimsPrincipalFactory(UserManager<BBUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)   
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(BBUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

            identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));

            return identity;
        }

    }
}
