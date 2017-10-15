using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Discover.Web.Mvc
{
    public static class RoutingHelper
    {
        public static RouteValueDictionary Append(this RouteValueDictionary routeValues, string key, object value)
        {
            routeValues.Add(key, value);
            return routeValues;
        }

        public static RouteValueDictionary MergeWith(this RouteValueDictionary routeValues, object values)
        {
            var result = new RouteValueDictionary(values);
            foreach (var pair in routeValues)
            {
                result.Add(pair.Key, pair.Value);
            }
            return result;
        }
    }
}
