using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using INGA.Framework.DataLayer.Entities.Membership;
using INGA.Framework.NoSqlProviders.Common;
using Microsoft.AspNet.Identity;
using INGA.Framework.Helpers.Security.Encryption;
using INGA.Framework.DataLayer.Entities.Interfaces;

namespace INGA.Framework.EnterpriseLibraries.Authentication.Identity
{
    public class UserStore<TUser> :
        IUserStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserRoleStore<TUser>,
        IUserLoginStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserClaimStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser, string>,
        IUserLockoutStore<TUser, string> where TUser : class, IIdentityUser<string>
    {


        private static INoSqlProvider _provider = null;
        private static INoSqlProvider Provider
        {
            get
            {

                if (_provider == null)
                {
                    _provider = NoSqlManager.ProviderFactory.Instance;
                }
                return _provider;
            }
        }


        public UserStore()
        {
        }


        #region IUserStore<TUser,string> Members

        public Task CreateAsync(TUser user)
        {
            string key = string.Format("user::{0}", user.Email);
            var result = Provider.Set<TUser>(key, user);
            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TUser user)
        {
            return Task.FromResult(Provider.Remove<TUser>(string.Format("user::{0}", user.Email)).Status);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            
            return Task.FromResult(Provider.Get<TUser>(userId).Result);
        }

        public Task<TUser> FindByNameAsync(string Email)
        {
            var user = Provider.Get<TUser>(string.Format("user::{0}", Email)).Result;
            return Task.FromResult((TUser)user);
        }

        public Task UpdateAsync(TUser user)
        {
            return Task.FromResult(Provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user).Status);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

        #region IUserPasswordStore<TUser,string> Members

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<string>(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult<bool>(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.PasswordHash = passwordHash;
            return Task.FromResult<int>(0);
        }

        #endregion

        #region IUserSecurityStampStore<TUser,string> Members

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<string>(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult<int>(0);
        }

        #endregion

        #region IUserLoginStore<TUser,string> Members

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {


            if (user == null)
                throw new ArgumentNullException("user");
            if (login == null)
                throw new ArgumentNullException("login");

            if (user.Logins == null)
            {
                user.Logins = new List<UserLoginInfo>();
            }
            user.Logins.Add(login);
            Provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user);

            return Task.FromResult<int>(0);
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {

            if (login == null)
                throw new ArgumentNullException("login");
            var result = Provider.ExecuteQuery<TUser>("user", "userlogin", string.Format("{0}-{1}", login.LoginProvider, login.ProviderKey)).Results;
            return Task.FromResult(result.FirstOrDefault());
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<IList<UserLoginInfo>>(new List<UserLoginInfo>(user.Logins ?? new List<UserLoginInfo>()));
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {

            if (user == null)
                throw new ArgumentNullException("user");
            if (login == null)
                throw new ArgumentNullException("login");

            var userLogin = user.Logins.First(l => l.ProviderKey == login.ProviderKey && l.LoginProvider == login.LoginProvider);

            if (userLogin == null)
                throw new InvalidOperationException("login does not exist");
            user.Logins.Remove(userLogin);

            Provider.Upsert<TUser>(string.Format("{0}", user.Email), user);
            return Task.FromResult<int>(0);


        }

        #endregion

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            user.Roles.Add(roleName);

            Provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user);
            return Task.FromResult<int>(0);
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.FromResult((IList<string>)user.Roles);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return Task.FromResult(user.Roles.Contains(roleName));
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            user.Roles.Remove(roleName);
            Provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user);
            return Task.FromResult(0);

        }

        #region Email Auth
        public Task<TUser> FindByEmailAsync(string email)
        {
            return Task.FromResult(Provider.Get<TUser>(string.Format("user::{0}", email)).Result);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult<bool>(user.EmailConfirmed);
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;
            Provider.Upsert(string.Format("user::{0}", email), user);
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = true;
            Provider.Upsert(string.Format("user::{0}",user.Email), user);
            return Task.FromResult(0);
        }
        #endregion

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            user.AddClaim(claim);
            Provider.Upsert<TUser>(string.Format("{0}", user.Email), user);
            return Task.FromResult(0);
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return Task.FromResult((IList<Claim>)user.Claims.Select(c => c.ToSecurityClaim()).ToList());
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            user.RemoveClaim(claim);
            Provider.Upsert<TUser>(string.Format("{0}", user.Email), user);

            return Task.FromResult(0);

        }

        public IQueryable<TUser> Users
        {
            get
            {
                return Provider.ExecuteQuery<TUser>("UserOperations", "GetUsers").Results.AsQueryable<TUser>();
            }
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }
    }
}
