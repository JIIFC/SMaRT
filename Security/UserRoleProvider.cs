using JT.AspNetBaseRoleProvider;

using Microsoft.EntityFrameworkCore;

using SMARTV3.Models;

namespace SMARTV3.Security
{
    public class UserRoleProvider : IBaseRoleProvider
    {
        private readonly IServiceScopeFactory scopeFactory;

        public UserRoleProvider(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public Task<ICollection<string>> GetUserRolesAsync(string userName)
        {
            var result = new List<string>();
            userName = RemoveDomainFromUsername(userName).ToLower();
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SMARTV3DbContext>();

            // Finds only enabled users matching the username
            User? user = db.Users.Where(i => i.Enabled == true && i.UserName.ToLower() == userName)
                                 .Include(i => i.Roles).ToList().FirstOrDefault();
            if (user != null && user.Roles.Count > 0)
            {
                Role? role = user.Roles.FirstOrDefault();
                if (role != null && role.RoleName != null)
                {
                    result.Add(role.RoleName);
                }
            }

            return Task.FromResult((ICollection<string>)result);
        }

        public static string RemoveDomainFromUsername(string username)
        {
            if (username.Contains('\\'))
            {
                return username.Split('\\')[1];
            }
            return username;
        }
    }
}