using System;
using Owin;
using Umbraco.Core;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Web;
using Umbraco.Web.Security.Identity;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin;
using Umbraco.Core.Models;

using Umbraco.Web.WebApi;

namespace TwoFactorAuthentication.Middleware
{
    public sealed class TwoFactorEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            UmbracoDefaultOwinStartup.MiddlewareConfigured += ConfigureTwoFactorAuthentication;

           // Section section = applicationContext.Services.SectionService.GetByAlias("2_Step_Verification");
            
            //var adminGroup = applicationContext.Services.UserService.GetAllInGroup(GetUserGroupByAlias("2_Step_Verification");
            //adminGroup.AddAllowedSection(alias);
            //applicationContext.Services.UserService.se //FindByEmail//AddSectionToAllUsers("2_Step_Verification");
            //if (section != null) return;

            // Add a new "Skrift Demo" section
            //applicationContext.Services.SectionService.MakeNew("Skrift Demo", "2_Step_Verification", "icon-newspaper");
        }

        private void ConfigureTwoFactorAuthentication(object sender, OwinMiddlewareConfiguredEventArgs args)
        {
          


            var app = args.AppBuilder;
            var applicationContext = ApplicationContext.Current;
            //netser/////////////////////////
           /* var oAuthServerOptions = new OAuthAuthorizationServerOptions

            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1)
            };
            // Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());*/
            ////////////////////end netser

            app.SetUmbracoLoggerFactory();
            app.UseTwoFactorSignInCookie(Umbraco.Core.Constants.Security.BackOfficeTwoFactorAuthenticationType, TimeSpan.FromMinutes(5));
            
            // app.UseOAuthAuthorizationServer(options);
            // app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            // We need to set these values again after our custom changes. Otherwise preview doesn't work.
            app.UseUmbracoBackOfficeCookieAuthentication(applicationContext)
                .UseUmbracoBackOfficeExternalCookieAuthentication(applicationContext)
                .UseUmbracoPreviewAuthentication(applicationContext); /**/
            
            app.ConfigureUserManagerForUmbracoBackOffice<TwoFactorBackOfficeUserManager, BackOfficeIdentityUser>(
                applicationContext,
                (options, context) =>
                {
                    var membershipProvider = MembershipProviderExtensions.GetUsersMembershipProvider().AsUmbracoMembershipProvider();
                    var userManager = TwoFactorBackOfficeUserManager.Create(options,
                        applicationContext.Services.UserService,
                        applicationContext.Services.EntityService,
                        applicationContext.Services.ExternalLoginService,
                        membershipProvider);
                    return userManager;
                });
        }
    }
}