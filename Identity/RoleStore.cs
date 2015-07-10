using System.Linq;
using System.Threading.Tasks;
using INGA.Framework.NoSqlProviders.Common;
using Microsoft.AspNet.Identity;
using INGA.Framework.DataLayer.Entities.Membership;

namespace INGA.Framework.EnterpriseLibraries.Authentication.Identity
{
    public class RoleStore<TRole> : 
        IRoleStore<TRole>, 
        IQueryableRoleStore<TRole>
        where TRole : UserRole
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
        public RoleStore()
        {
            
        }

        public IQueryable<TRole> Roles
        {
            get
            {
                return Provider.ExecuteQuery<TRole>("UserOperations", "GetRoles").Results.AsQueryable<TRole>();
            }
        }


        public Task CreateAsync(TRole role)
        {
            Provider.Remove<TRole>(role.Id);
            return Task.FromResult(role);
        }

        public Task DeleteAsync(TRole role)
        {
            Provider.Remove<TRole>(role.Id);
            return Task.FromResult(role);
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            return Task.FromResult(Provider.Get<TRole>(roleId).Result);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            var result = Provider.ExecuteQuery<TRole>("UserOperations", "GetRoleByName").Result;
            return Task.FromResult(result);
        }

        public Task UpdateAsync(TRole role)
        {
            return Task.FromResult(Provider.Upsert(role.Id, role));
        }

        public void Dispose()
        {
        }
    }
}
