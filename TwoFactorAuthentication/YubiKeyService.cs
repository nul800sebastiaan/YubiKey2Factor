using System;
using System.Configuration;
using System.Linq;
using TwoFactorAuthentication.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using YubicoDotNetClient;

namespace TwoFactorAuthentication
{
    internal class YubiKeyService
    {
        internal IYubicoResponse Validate(string token, int userId = -1)
        {
            var clientId = ConfigurationManager.AppSettings["YubiKey.ClientId"];
            var secretKey = ConfigurationManager.AppSettings["YubiKey.SecretKey"];
            var client = new YubicoClient(clientId, secretKey);
            var database = ApplicationContext.Current.DatabaseContext.Database;

            try
            {
                var response = client.Verify(token);
                if (response.Status == YubicoResponseStatus.Ok)
                {
                    //check that this specific user has registered this YubiKey
                    if (userId == -1)
                        return response;

                    var result = database.Fetch<TwoFactor>(string.Format("WHERE [userId] = {0} AND [key] = '{1}' AND [confirmed] = 1",
                        userId, Constants.YubiKeyProviderName));
                    
                    if (result.Any(x => x.Value == response.PublicId))
                        return response;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<YubiKeyService>("Could not validate the provided one-time-password", ex);
            }

            return null;
        }
    }
}