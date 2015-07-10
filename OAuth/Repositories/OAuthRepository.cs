using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using INGA.Framework.EnterpriseLibraries.Authentication.OAuth.Entities;
using INGA.Framework.NoSqlProviders.Common;

namespace INGA.Framework.EnterpriseLibraries.Authentication.OAuth.Repositories
{
    public class OAuthRepository : IDisposable
    {

        private static INoSqlProvider _ctx = NoSqlManager.ProviderFactory.Instance;

        public OAuthRepository()
        {
           
        }


        public void SetClient(Client client)
        {
            _ctx.Set(client.Id, client);
        }

        public Client FindClient(string clientId)
        {
            var client = _ctx.Get<Client>(clientId).Result;

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var tokens = _ctx.ExecuteQuery<RefreshToken>("UserOperations", "RefreshTokens").Results;
            

            var existingToken = tokens.SingleOrDefault(r => r.Subject == token.Subject && r.ClientId == token.ClientId);

            if (existingToken != null)
            {
                await RemoveRefreshToken(existingToken);
            }

            _ctx.Set(token.Id, token);

            return await Task.FromResult<bool>(true);
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var tokens = _ctx.ExecuteQuery<RefreshToken>("UserOperations", "RefreshTokens", refreshTokenId).Results;

            var refreshToken = tokens.FirstOrDefault(p => p.Id == refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.Remove<RefreshToken>(refreshToken.Id);
                return await Task.FromResult<bool>(true);
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.Remove<RefreshToken>(refreshToken.Id);
            return await Task.FromResult<bool>(true);
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var tokens = _ctx.ExecuteQuery<RefreshToken>("UserOperations", "RefreshTokens", refreshTokenId).Results;
            var refreshToken = await Task.FromResult(tokens.FirstOrDefault(p => p.Id == refreshTokenId));
            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            var tokens = _ctx.ExecuteQuery<RefreshToken>("UserOperations", "RefreshTokens").Results.ToList();
            return tokens;
        }

        public void Dispose()
        {

        }
    }
}
