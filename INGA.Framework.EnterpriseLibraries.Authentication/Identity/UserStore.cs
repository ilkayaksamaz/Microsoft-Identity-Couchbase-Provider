using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using INGA.Framework.DataAccessObjects.Membership;
using INGA.Framework.NoSqlProviders.Common;
using INGA.Framework.NoSqlProviders.Manager;
using Microsoft.AspNet.Identity;

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
        IUserLockoutStore<TUser, string> where TUser : UserModel
    {
        private static INoSqlProvider _provider;

        public UserStore()
        {
            if (_provider == null)
            {
                _provider = NoSqlProviderFactory.Instance;
            }

        }


        #region IUserStore<TUser,string> Members

        public Task CreateAsync(TUser user)
        {
            string key = string.Format("user::{0}", user.Email);
            _provider.Save<TUser>(key, user);
            return Task.FromResult(user);
        }

        public Task DeleteAsync(TUser user)
        {
            _provider.Remove(string.Format("user::{0}", user.Email));
            return Task.FromResult(user);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            return Task.FromResult(_provider.Get<TUser>(userId));
        }

        public Task<TUser> FindByNameAsync(string Email)
        {
            return Task.FromResult(_provider.Get<TUser>(string.Format("user::{0}", Email)));
        }

        public Task UpdateAsync(TUser user)
        {
            return Task.FromResult(_provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user));
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
            _provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user);

            return Task.FromResult<int>(0);
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {

            if (login == null)
                throw new ArgumentNullException("login");

            var result = _provider.QueryResult<TUser>(
                string.Format(
                    "SELECT login FROM UserModel AS login WHERE login.LoginProvider == {0} && login.ProviderKey == {1}",
                    login.LoginProvider, login.ProviderKey));

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

            _provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user);
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

            _provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user);
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
            _provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user);
            return Task.FromResult(0);

        }

        #region Email Auth
        public Task<TUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }
        #endregion

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            user.AddClaim(claim);
            _provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user);
            return Task.FromResult(0);
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return Task.FromResult((IList<Claim>)user.Claims.Select(c => c.ToSecurityClaim()).ToList());
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            user.RemoveClaim(claim);
            _provider.Upsert<TUser>(string.Format("user::{0}", user.Email), user);

            return Task.FromResult(0);

        }

        public IQueryable<TUser> Users
        {
            get
            {
                return _provider.GetView<TUser>("UserOperations", "GetUsers");
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
