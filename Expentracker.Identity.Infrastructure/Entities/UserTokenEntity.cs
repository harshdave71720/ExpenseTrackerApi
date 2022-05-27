using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Identity.Infrastructure.Entities
{
    public class UserTokenEntity : IdentityUserToken<string>
    {
        public bool? Revoked { get; set; }
    }
}
