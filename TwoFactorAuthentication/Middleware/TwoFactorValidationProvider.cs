using System.Linq;
using System.Threading.Tasks;
using Google.Authenticator;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using TwoFactorAuthentication.Models;
using Umbraco.Core;
using Umbraco.Core.Models.Identity;
using YubicoDotNetClient;

namespace TwoFactorAuthentication.Middleware
{
    internal class TwoFactorValidationProvider : DataProtectorTokenProvider<BackOfficeIdentityUser, int>, IUserTokenProvider<BackOfficeIdentityUser, int>
    {
        public TwoFactorValidationProvider(IDataProtector protector) : base(protector)
        { }

        /// <summary>
        /// Explicitly implement this interface method - which overrides the base class's implementation
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> IUserTokenProvider<BackOfficeIdentityUser, int>.ValidateAsync(string purpose, string token, UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            if (purpose == Constants.GoogleAuthenticatorProviderName)
            {
                var twoFactorAuthenticator = new TwoFactorAuthenticator();
                var database = ApplicationContext.Current.DatabaseContext.Database;
                var result = database.Fetch<TwoFactor>(string.Format("WHERE [userId] = {0} AND [key] = '{1}' AND [confirmed] = 1",
                    user.Id, Constants.GoogleAuthenticatorProviderName));
                if (result.Any() == false)
                    return Task.FromResult(false);

                var key = result.First().Value;
                var validToken = twoFactorAuthenticator.ValidateTwoFactorPIN(key, token);
                return Task.FromResult(validToken);
            }

            if (purpose == Constants.YubiKeyProviderName)
            {
                var yubiKeyService = new YubiKeyService();
                var response = yubiKeyService.Validate(token, user.Id);
                return Task.FromResult(response != null && response.Status == YubicoResponseStatus.Ok);
            }

            return Task.FromResult(false);
        }
    }
}