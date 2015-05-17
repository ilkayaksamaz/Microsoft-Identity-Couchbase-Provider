using System.Linq;
using System.Threading.Tasks;
using INGA.Framework.DataAccessObjects.Membership;
using INGA.Framework.NoSqlProviders.Common;
using INGA.Framework.NoSqlProviders.Manager;
using Microsoft.AspNet.Identity;

namespace INGA.Framework.EnterpriseLibraries.Authentication.Identity
{
    public class RoleStore<TRole> : IRoleStore<TRole>, IQueryableRoleStore<TRole>
        where TRole : UserRole
    {
        private readonly INoSqlProvider _provider;
        public RoleStore()
        {
            if (_provider == null)
            {
                _provider = NoSqlProviderFactory.Instance;
            }
        }

        public IQueryable<TRole> Roles
        {
            get
            {
                return _provider.GetView<TRole>("UserOperations", "GetRoles");
            }
        }


        public Task CreateAsync(TRole role)
        {
            _provider.Remove(role.Id);
            return Task.FromResult(role);
        }

        public Task DeleteAsync(TRole role)
        {
            _provider.Remove(role.Id);
            return Task.FromResult(role);
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            return Task.FromResult(_provider.Get<TRole>(roleId));
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            var result  = _provider.GetView<TRole>("UserOperations", "GetRoleByName");;
            return Task.FromResult(Enumerable.FirstOrDefault(result.Where(r => r.Name == roleName)));
        }

        public Task UpdateAsync(TRole role)
        {
            return Task.FromResult(_provider.Upsert(role.Id, role));
        }

        public void Dispose()
        {
        }
    }
}
