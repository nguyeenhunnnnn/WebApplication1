using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebApplication1.Models.Entities;
namespace WebApplication1.Services
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<TaiKhoan>
    {
        public AppClaimsPrincipalFactory(
        UserManager<TaiKhoan> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
        {
        }

       
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TaiKhoan user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim("VaiTro", user.VaiTro ?? ""));
            identity.AddClaim(new Claim("Avatar", user.FileAvata ?? "/images/avatar-default.png")); // thêm dòng này
            identity.AddClaim(new Claim("GoiCuoc", user.GoiCuoc ? "true" : "false"));
            return identity;
        }
    }
}
