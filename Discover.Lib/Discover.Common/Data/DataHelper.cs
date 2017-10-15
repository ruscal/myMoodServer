using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Discover.Data
{
    public static class DataHelper
    {
        public static string GetParameterPlaceholderFor(this DbProviderFactory provider, string parameterName)
        {
            return provider is System.Data.SqlClient.SqlClientFactory ? "@" + parameterName :
                provider.GetType().Name.Contains("Oracle") ? ":" + parameterName :
                "?";
        }

        public static string GetParameterName(this DbProviderFactory provider, string parameterName)
        {
            return provider is System.Data.SqlClient.SqlClientFactory ? "@" + parameterName :
                provider.GetType().Name.Contains("Oracle") ? ":" + parameterName :
                null;
        }

        public static DbParameter AddParameter(this DbCommand command, Action<DbParameter> configure)
        {
            var parameter = command.CreateParameter();
            configure(parameter);
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}
