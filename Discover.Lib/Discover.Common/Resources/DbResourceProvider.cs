using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Resources;
using System.Web.Compilation;
using System.Runtime.Caching;
using Discover.Data;

namespace Discover.Resources
{
    public sealed class DbResourceProviderFactory : ResourceProviderFactory
    {
        private DbProviderFactory dbProvider;
        private string connectionString;
        private string tableName;

        public const string ResourceConnectionStringName = "Discover.Resources.DbResourceProvider";
        public const string ResourceTableAppSettingName = "Discover.Resources.DbResourceProvider.TableName";
        public const string ResourceTableDefault = "Resources";

        public DbResourceProviderFactory()
        {
            this.dbProvider = DbProviderFactories.GetFactory(System.Configuration.ConfigurationManager.ConnectionStrings[ResourceConnectionStringName].ProviderName);
            this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[ResourceConnectionStringName].ConnectionString;
            this.tableName = System.Configuration.ConfigurationManager.AppSettings[ResourceTableAppSettingName] ?? ResourceTableDefault;
        }

        public DbResourceProviderFactory(string providerName, string connectionString)
        {
            this.dbProvider = DbProviderFactories.GetFactory(providerName);
            this.connectionString = connectionString;
            this.tableName = System.Configuration.ConfigurationManager.AppSettings[ResourceTableAppSettingName] ?? ResourceTableDefault;
        }

        public override IResourceProvider CreateGlobalResourceProvider(string classKey)
        {
            return new DbResourceProvider(this.dbProvider, this.connectionString, this.tableName, classKey);
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            return new DbResourceProvider(this.dbProvider, this.connectionString, this.tableName, System.IO.Path.GetFileName(virtualPath));
        }

        public static void CreateResourceTable()
        {
            CreateResourceTable(
                System.Configuration.ConfigurationManager.ConnectionStrings[ResourceConnectionStringName].ProviderName,
                System.Configuration.ConfigurationManager.ConnectionStrings[ResourceConnectionStringName].ConnectionString,
                System.Configuration.ConfigurationManager.AppSettings[ResourceTableAppSettingName] ?? ResourceTableDefault
                );
        }

        public static void CreateResourceTable(string providerName, string connectionString, string tableName)
        {
            var provider = DbProviderFactories.GetFactory(providerName);

            using (var connection = provider.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = string.Format(@"
                    IF NOT EXISTS(SELECT 1 FROM [sys].[all_objects] WHERE [type] = 'U' AND [name] = '{0}')
                    BEGIN
                        CREATE TABLE [dbo].[{0}](
	                        [ResourceGroup] [nvarchar](256) NOT NULL,
	                        [ResourceKey] [nvarchar](128) NOT NULL,
	                        [CultureCode] [varchar](8) NOT NULL,
	                        [ResourceValue] [nvarchar](max) NULL,
                         CONSTRAINT [PK_Resources] PRIMARY KEY CLUSTERED 
                        (
	                        [ResourceGroup] ASC,
	                        [ResourceKey] ASC,
	                        [CultureCode] ASC
                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                        ) ON [PRIMARY]
                    END", tableName);

                command.ExecuteNonQuery();
            }
        }
    }

    public class DbResourceProvider : IResourceProvider
    {
        private DbProviderFactory dbProvider;
        private string connectionString;
        private string tableName;
        private string resourceGroupKey;
        private ObjectCache cache = MemoryCache.Default;

        public DbResourceProvider(DbProviderFactory dbProvider, string connectionString, string tableName, string resourceGroupKey)
        {
            this.dbProvider = dbProvider;
            this.connectionString = connectionString;
            this.tableName = tableName;
            this.resourceGroupKey = resourceGroupKey;
        }

        public object GetObject(string resourceKey, CultureInfo culture)
        {
            var resources = FindResourcesFor(culture);
            return resources != null && resources.Contains(resourceKey) ? resources[resourceKey] : null;
        }

        public IResourceReader ResourceReader
        {
            get { return new DbResourceReader(FindResourcesFor(CultureInfo.InvariantCulture)); }
        }

        private IDictionary FindResourcesFor(CultureInfo culture)
        {
            var results = new Dictionary<string, object>();

            var cacheKey = string.Concat(this.GetType().FullName, "|", this.resourceGroupKey, "|", (culture != null && culture != CultureInfo.InvariantCulture ? culture.Name : string.Empty));

            if (cache.Contains(cacheKey))
            {
                results = cache.Get(cacheKey) as Dictionary<string, object>;
            }
            else
            {
                using (var connection = this.dbProvider.CreateConnection())
                {
                    connection.ConnectionString = this.connectionString;
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;

                    var sql = new StringBuilder();

                    sql.AppendFormat("SELECT [ResourceKey], [ResourceValue], [CultureCode] FROM [{0}] ", this.tableName);

                    sql.AppendFormat("WHERE ([ResourceGroup] = {0})", dbProvider.GetParameterPlaceholderFor("resourceGroup"));
                    command.AddParameter(p => { p.ParameterName = dbProvider.GetParameterName("resourceGroup"); p.Value = this.resourceGroupKey; });

                    sql.Append(" AND (LEN([CultureCode]) = 0");
                    if (culture != null && culture != CultureInfo.InvariantCulture)
                    {
                        sql.AppendFormat(" OR [CultureCode] = {0}", dbProvider.GetParameterPlaceholderFor("language"));
                        command.AddParameter(p => { p.ParameterName = dbProvider.GetParameterName("language"); p.Value = culture.TwoLetterISOLanguageName; });

                        if (culture.Name != culture.TwoLetterISOLanguageName)
                        {
                            sql.AppendFormat(" OR [CultureCode] = {0}", dbProvider.GetParameterPlaceholderFor("culture"));
                            command.AddParameter(p => { p.ParameterName = dbProvider.GetParameterName("culture"); p.Value = culture.Name; });
                        }
                    }
                    sql.Append(")");

                    // reverse ordering ensures correct fallback precedence
                    // {lang}-{region} => {lang} => NULL
                    sql.Append(" ORDER BY [CultureCode] DESC");

                    command.CommandText = sql.ToString();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var resourceKey = reader.GetString(0);
                            if (!results.ContainsKey(resourceKey))
                            {
                                results.Add(resourceKey, reader.GetString(1));
                            }
                        }
                    }
                }

                cache.Add(cacheKey, results, new CacheItemPolicy());
            }

            return results;
        }
    }

    public class DbResourceReader : IResourceReader
    {
        private IDictionary resources;

        internal DbResourceReader(IDictionary resources)
        {
            this.resources = resources;
        }

        public void Close()
        {
        }

        public System.Collections.IDictionaryEnumerator GetEnumerator()
        {
            return resources.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return resources.GetEnumerator();
        }

        public void Dispose()
        {
        }
    }
}
