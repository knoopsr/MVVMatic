using Microsoft.Data.SqlClient;
using System.IO;

namespace MVVMatic
{
    public static class SettingsReader
    {
        public static string FindConnectionString(string solutionPath, string settingNaam = "ConnectionDB")
        {
            // Zoek de DAL-projectfolder
            string dalFolder = Directory.GetDirectories(solutionPath)
                .FirstOrDefault(d => d.EndsWith(".DAL"));

            if (dalFolder == null)
                throw new DirectoryNotFoundException("Geen DAL-projectmap gevonden.");

            string settingsPath = Path.Combine(dalFolder, "Properties", "Connection.settings");

            if (!File.Exists(settingsPath))
                throw new FileNotFoundException("Settingsbestand niet gevonden.", settingsPath);

            var doc = new System.Xml.XmlDocument();
            doc.Load(settingsPath);

            // XML namespace voor settingsbestand
            var nsmgr = new System.Xml.XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("s", "http://schemas.microsoft.com/VisualStudio/2004/01/settings");

            var node = doc.SelectSingleNode($"/s:SettingsFile/s:Settings/s:Setting[@Name='{settingNaam}']/s:Value", nsmgr);

            return node?.InnerText ?? "";
        }

        public static string ExtractDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog; // dit is de database naam
        }
    }

}