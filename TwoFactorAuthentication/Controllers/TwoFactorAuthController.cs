using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Google.Authenticator;
using TwoFactorAuthentication.Models;
using Umbraco.Core.Logging;
using Umbraco.Web.WebApi;
using YubicoDotNetClient;

namespace TwoFactorAuthentication.Controllers
{
    public class TwoFactorAuthController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public List<TwoFactorAuthInfo> TwoFactorEnabled()
        {
            var database = DatabaseContext.Database;
            var user = Security.CurrentUser;
            var result = database.Fetch<TwoFactor>("WHERE [userId] = @userId AND [confirmed] = 1", new { userId = user.Id });
            var twoFactorAuthInfo = new List<TwoFactorAuthInfo>();
            foreach (var factor in result)
            {
                var authInfo = new TwoFactorAuthInfo { ApplicationName = factor.Key };
                twoFactorAuthInfo.Add(authInfo);
            }
            return twoFactorAuthInfo;
        }

        [HttpGet]
        public TwoFactorAuthInfo GoogleAuthenticatorSetupCode()
        {
            var tfa = new TwoFactorAuthenticator();
            var user = Security.CurrentUser;
            var accountSecretKey = Guid.NewGuid().ToString();
            var setupInfo = tfa.GenerateSetupCode("TestApp", user.Email, accountSecretKey, 300, 300);

            var database = DatabaseContext.Database;
            var twoFactorAuthInfo = new TwoFactorAuthInfo();
            var existingAccount = database.Fetch<TwoFactor>(string.Format("WHERE userId = {0} AND [key] = '{1}'", 
                user.Id, Constants.GoogleAuthenticatorProviderName));
            if (existingAccount.Any())
            {
                var account = existingAccount.First();
                if (account.Confirmed)
                    return twoFactorAuthInfo;
                
                var tf = new TwoFactor { Value = accountSecretKey, UserId = user.Id, Key =  Constants.GoogleAuthenticatorProviderName };
                var update = database.Update(tf);

                if (update == 0)
                    return twoFactorAuthInfo;
            }
            else
            {
                var result = database.Insert(new TwoFactor { UserId = user.Id, Key = Constants.GoogleAuthenticatorProviderName, Value = accountSecretKey, Confirmed = false });
                if (result is bool == false)
                    return twoFactorAuthInfo;

                var insertSucces = (bool)result;
                if (insertSucces == false)
                    return twoFactorAuthInfo;

            }
            
            twoFactorAuthInfo.Secret = setupInfo.ManualEntryKey;
            twoFactorAuthInfo.Email = user.Email;
            twoFactorAuthInfo.ApplicationName = "TestApp";

            return twoFactorAuthInfo;
        }

        [HttpPost]
        public bool ValidateAndSave(string code)
        {
            var database = DatabaseContext.Database;
            var user = Security.CurrentUser;
            var insertSucces = false;
            try
            {
                var yubiKeyService = new YubiKeyService();
                var response = yubiKeyService.Validate(code);
                if (response != null && response.Status == YubicoResponseStatus.Ok)
                {
                    var result = database.Insert(new TwoFactor { UserId = user.Id, Key = Constants.YubiKeyProviderName, Value = response.PublicId, Confirmed = true });
                    if (result is bool)
                        insertSucces = (bool)result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<TwoFactorAuthController>("Could not log in with the provided one-time-password", ex);
            }
            return insertSucces;
        }

        [HttpPost]
        public bool ValidateAndSaveGoogleAuth(string code)
        {
            var database = DatabaseContext.Database;
            var user = Security.CurrentUser;
            try
            {
                var twoFactorAuthenticator = new TwoFactorAuthenticator();
                var all = database.Fetch<TwoFactor>("WHERE userId = @userId", 
                    new { userId = user.Id });

                var result = all.LastOrDefault(t => t.Key == Constants.GoogleAuthenticatorProviderName);
                if (result != null)
                {
                    var isValid = twoFactorAuthenticator.ValidateTwoFactorPIN(result.Value, code);
                    if (isValid == false)
                        return false;

                    var tf = new TwoFactor { Confirmed = true, Value = result.Value, UserId = user.Id, Key = Constants.GoogleAuthenticatorProviderName };
                    var update = database.Update(tf);
                    isValid = update > 0;
                    return isValid;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<TwoFactorAuthController>("Could not log in with the provided one-time-password", ex);
            }
            return false;
        }

        [HttpPost]
        public bool Disable()
        {
            var database = DatabaseContext.Database;
            var user = Security.CurrentUser;
            var result = database.Delete<TwoFactor>("WHERE [userId] = @userId", new { userId = user.Id });
            //if more than 0 rows have been deleted, the query ran successfully
            return result != 0;
        }
    }
}