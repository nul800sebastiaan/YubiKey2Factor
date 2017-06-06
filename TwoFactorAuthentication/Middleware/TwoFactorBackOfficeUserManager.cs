using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Security.Identity;

namespace TwoFactorAuthentication.Middleware
{
    /// <summary>
    /// Subclass the default BackOfficeUserManager and extend it to support 2FA
    /// </summary>
    internal class TwoFactorBackOfficeUserManager : BackOfficeUserManager, IUmbracoBackOfficeTwoFactorOptions
    {
        public TwoFactorBackOfficeUserManager(IUserStore<BackOfficeIdentityUser, int> store) : base(store)
        { }

        /// <summary>
        /// Creates a BackOfficeUserManager instance with all default options and the default BackOfficeUserManager 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="userService"></param>
        /// <param name="externalLoginService"></param>
        /// <param name="membershipProvider"></param>
        /// <returns></returns>
        public static TwoFactorBackOfficeUserManager Create(
            IdentityFactoryOptions<TwoFactorBackOfficeUserManager> options,
            IUserService userService,
            IExternalLoginService externalLoginService,
            MembershipProviderBase membershipProvider)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (userService == null) throw new ArgumentNullException("userService");
            if (externalLoginService == null) throw new ArgumentNullException("externalLoginService");

            var manager = new TwoFactorBackOfficeUserManager(new TwoFactorBackOfficeUserStore(userService, externalLoginService, membershipProvider));
            manager.InitUserManager(manager, membershipProvider, options.DataProtectionProvider);
            
            //Here you can specify the 2FA providers that you want to implement
            var dataProtectionProvider = options.DataProtectionProvider;
            manager.RegisterTwoFactorProvider(Constants.YubiKeyProviderName, 
                new TwoFactorValidationProvider(dataProtectionProvider.Create(Constants.YubiKeyProviderName)));
            manager.RegisterTwoFactorProvider(Constants.GoogleAuthenticatorProviderName, 
                new TwoFactorValidationProvider(dataProtectionProvider.Create(Constants.GoogleAuthenticatorProviderName)));

            return manager;
        }

        /// <summary>
        /// Override to return true
        /// </summary>
        public override bool SupportsUserTwoFactor
        {
            get { return true; }
        }

        /// <summary>
        /// Return the view for the 2FA screen
        /// </summary>
        /// <param name="owinContext"></param>
        /// <param name="umbracoContext"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetTwoFactorView(IOwinContext owinContext, UmbracoContext umbracoContext, string username)
        {
            return "../App_Plugins/2FactorAuthentication/2fa-login.html";
        }
    }
}