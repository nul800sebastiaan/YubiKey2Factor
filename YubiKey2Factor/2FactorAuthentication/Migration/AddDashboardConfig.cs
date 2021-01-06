using System.Xml;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace TwoFactorAuthentication.Migrations
{
    [Migration("1.0.0", 1, Constants.ProductName)]
    internal class AddDashboardConfig : MigrationBase
    {
        public AddDashboardConfig(ISqlSyntaxProvider sqlSyntax, ILogger logger) : base(sqlSyntax, logger)
        { }

        public override void Up()
        {
            const string dashboardConfigFilePath = "~/Config/Dashboard.config";
            var configFile = OpenAsXmlDocument(dashboardConfigFilePath);
            var rootNode = configFile.DocumentElement;
            if (rootNode == null)
                return;

            var existingNode = rootNode.SelectSingleNode("//section[@alias='TwoFactorManagement']");
            if (existingNode != null)
                return;

            const string xmlContent = "<section alias=\"TwoFactorManagement\"><areas><area>content</area></areas><tab caption=\"Two Step Verification\"><control>/App_Plugins/2FactorAuthentication/managementdashboard.html</control></tab></section>";
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlContent);

            if (xmlDocument.DocumentElement == null)
                return;

            var importNode = configFile.ImportNode(xmlDocument.DocumentElement, true);
            rootNode.AppendChild(importNode);
            var dashboardConfigFile = IOHelper.MapPath(dashboardConfigFilePath);
            configFile.Save(dashboardConfigFile);
        }

        public override void Down()
        {
            const string dashboardConfigFilePath = "~/Config/Dashboard.config";
            var configFile = OpenAsXmlDocument(dashboardConfigFilePath);
            var rootNode = configFile.DocumentElement;
            if (rootNode == null)
                return;

            var existingNode = rootNode.SelectSingleNode("//section[@alias='TwoFactorManagement']");
            if (existingNode == null || existingNode.ParentNode == null)
                return;

            existingNode.ParentNode.RemoveChild(existingNode);
            var dashboardConfigFile = IOHelper.MapPath(dashboardConfigFilePath);
            configFile.Save(dashboardConfigFile);
        }

        public static XmlDocument OpenAsXmlDocument(string filePath)
        {
            XmlDocument xmlDocument;
            using (var reader = new XmlTextReader(IOHelper.MapPath(filePath)) { WhitespaceHandling = WhitespaceHandling.All })
            {
                xmlDocument = new XmlDocument();
                xmlDocument.Load(reader);
            }

            return xmlDocument;
        }
    }
}