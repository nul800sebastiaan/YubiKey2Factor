using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace TwoFactorAuthentication.Migrations
{
    [Migration("1.0.0", 1, Constants.ProductName)]
    internal class CreateTwoFactorTable : MigrationBase
    {
        public CreateTwoFactorTable(ISqlSyntaxProvider sqlSyntax, ILogger logger) : base(sqlSyntax, logger)
        { }

        public override void Up()
        {
            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToArray();
            if (tables.InvariantContains(Constants.ProductName)) return;

            Create.Table(Constants.ProductName)
                .WithColumn("userId").AsInt32().NotNullable()
                .WithColumn("key").AsString()
                .WithColumn("value").AsString()
                .WithColumn("confirmed").AsBoolean();
        }

        public override void Down()
        {
            Delete.Table(Constants.ProductName);
        }
    }
}