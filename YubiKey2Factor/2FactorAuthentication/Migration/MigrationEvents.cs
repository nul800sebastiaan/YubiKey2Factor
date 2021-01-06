using System;
using System.Linq;
using Semver;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;

namespace TwoFactorAuthentication.Migrations
{
    public class MigrationEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            HandleTwoFactorMigration(applicationContext);
        }

        private static void HandleTwoFactorMigration(ApplicationContext applicationContext)
        {
            var currentVersion = new SemVersion(0, 0, 0);

            // get all migrations for "TwoFactor" already executed
            var migrations = applicationContext.Services.MigrationEntryService.GetAll(Constants.ProductName);

            // get the latest migration for "TwoFactor" executed
            var latestMigration = migrations.OrderByDescending(x => x.Version).FirstOrDefault();

            if (latestMigration != null)
                currentVersion = latestMigration.Version;

            var targetVersion = new SemVersion(1, 0, 0);
            if (targetVersion == currentVersion)
                return;

            var migrationsRunner = new MigrationRunner(
                applicationContext.Services.MigrationEntryService,
                applicationContext.ProfilingLogger.Logger,
                currentVersion,
                targetVersion,
                Constants.ProductName);

            try
            {
                migrationsRunner.Execute(applicationContext.DatabaseContext.Database);
            }
            catch (Exception e)
            {
                LogHelper.Error<MigrationEvents>("Error running TwoFactor migration", e);
            }
        }
    }
}