using System;
using System.Configuration;

namespace SEGREWARDS_PROJECT.Infrastructure
{
    /// <summary>
    /// Reads MySQL connection string from App.config (connectionStrings name: SegrewardsMySql).
    /// </summary>
    public static class DatabaseConfig
    {
        public const string ConnectionStringName = "SegrewardsMySql";

        public static string GetConnectionString()
        {
            var cs = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Missing connection string '" + ConnectionStringName + "'. Add it to App.config under <connectionStrings>.");
            }

            return cs.ConnectionString;
        }
    }
}
