using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.IO;
using INGA.Framework.NoSqlProviders.Common;
using INGA.Framework.NoSqlProviders.Manager;

namespace INGA.Framework.EnterpriseLibraries.Authentication.OAuth
{
    public class OAuthRepository : IDisposable
    {

        private static INoSqlProvider _ctx;

        public OAuthRepository()
        {
            if (_ctx == null)
            {
                _ctx = NoSqlProviderFactory.Instance;
            }
        }


        public void SetClient(Client client)
        {
            _ctx.Save(client.Id, client);
        }

        public Client FindClient(string clientId)
        {
            var client = _ctx.Get<Client>(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var tokens = _ctx.GetView<RefreshToken>("UserOperations", "RefreshTokens");

            var existingToken = tokens.SingleOrDefault(r => r.Subject == token.Subject && r.ClientId == token.ClientId);

            if (existingToken != null)
            {
                await RemoveRefreshToken(existingToken);
            }

            _ctx.Save(token.Id, token);

            return await Task.FromResult<bool>(true);
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var tokens = _ctx.GetView<RefreshToken>("UserOperations", "RefreshTokens", refreshTokenId);

            var refreshToken = tokens.FirstOrDefault(p => p.Id == refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.Remove(refreshToken.Id);
                return await Task.FromResult<bool>(true);
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.Remove(refreshToken.Id);
            return await Task.FromResult<bool>(true);
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var tokens = _ctx.GetView<RefreshToken>("UserOperations", "RefreshTokens", refreshTokenId);
            var refreshToken = await Task.FromResult(tokens.FirstOrDefault(p => p.Id == refreshTokenId));
            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            var tokens = _ctx.GetView<RefreshToken>("UserOperations", "RefreshTokens").ToList();
            return tokens;
        }

        public void Dispose()
        {

        }
    }
}
