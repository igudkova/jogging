using Jogging.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jogging.Authentication
{
    public class AuthRepository : IDisposable
    {
        private DatabaseContext databaseContext;
        private UserManager<IdentityUser> userManager;

        public AuthRepository()
        {
            databaseContext = new DatabaseContext();
            userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(databaseContext));
        }

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            return await userManager.FindAsync(loginInfo);
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            return await userManager.CreateAsync(user);
        }

        public async Task<IdentityResult> AddToRoleAsync(string userId, string role)
        {
            return await userManager.AddToRoleAsync(userId, role);
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            return await userManager.AddLoginAsync(userId, login);
        }

        public async Task<IList<string>> GetRolesAsync(string userId)
        {
            return await userManager.GetRolesAsync(userId);
        }

        public IdentityUser FindByName(string name)
        {
            return userManager.FindByName(name);
        }

        public bool IsInRole(string userId, string role)
        {
            return userManager.IsInRole(userId, role);
        }

        public bool IsLogged(string userId)
        {
            var logins = userManager.GetLogins(userId);

            return logins.Count > 0;
        }

        public IdentityResult RemoveFromRole(string userId, string role)
        {
            return userManager.RemoveFromRole(userId, role);
        }

        public IdentityResult AddToRole(string userId, string role)
        {
            return userManager.AddToRole(userId, role);
        }

        public IdentityResult Delete(IdentityUser user)
        {
            return userManager.Delete(user);
        }

        public void Dispose()
        {
            databaseContext.Dispose();
            userManager.Dispose();
        }
    }
}