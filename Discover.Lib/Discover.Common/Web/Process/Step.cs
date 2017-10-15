using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Logic;
using System.Runtime.Serialization;
using System.Web.Routing;

namespace Discover.Web.Process
{
    [Serializable]
    public class Step : ISerializable
    {
        public string Name { get; set; }

        //public string BaseName { get; set; }

        public string FriendlyName { get; set; }

        public int Index { get; set; }

        public string RouteName { get; set; }

        public RouteValueDictionary RouteData { get; set; }

        public string Url { get; set; }
       
        public string SectionName { get; set; }

        public string SectionUrl { get; set; }

        public IDictionary<string, object> Attributes { get; set; }

        public Step() 
        {
            Attributes = new Dictionary<string, object>();
        }

        #region Serialization

        protected Step(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            FriendlyName = info.GetString("FriendlyName");
            RouteName = info.GetString("RouteName");
            Url = info.GetString("Url");
            SectionName = info.GetString("SectionName");
            SectionUrl = info.GetString("SectionUrl");

            RouteData = new RouteValueDictionary();

            foreach (var pair in info.GetValue("RouteData", typeof(RouteDataValue[])) as RouteDataValue[])
            {
                RouteData.Add(pair.Key, pair.Value);
            }

            Attributes = new Dictionary<string, object>();

            foreach (var attrib in info.GetValue("Attributes", typeof(CustomAttribute[])) as CustomAttribute[])
            {
                Attributes.Add(attrib.Key, attrib.Value);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("FriendlyName", FriendlyName);
            info.AddValue("RouteName", RouteName);
            info.AddValue("RouteData", RouteData.Select(pair => new RouteDataValue() { Key = pair.Key, Value = pair.Value }).ToArray());
            info.AddValue("Url", Url);
            info.AddValue("SectionName", SectionName);
            info.AddValue("SectionUrl", SectionUrl);
            info.AddValue("Attributes", Attributes.Select(pair => new CustomAttribute() { Key = pair.Key, Value = pair.Value }).ToArray());
        }

        [Serializable]
        private struct CustomAttribute
        {
            public string Key { get; set; }
            public object Value { get; set; }
        }

        [Serializable]
        private struct RouteDataValue
        {
            public string Key { get; set; }
            public object Value { get; set; }
        }

        #endregion
    }
}
