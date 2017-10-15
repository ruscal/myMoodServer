using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Discover.Security
{
    public class ExtendedPrincipal<TExtendedUserInfo> : IPrincipal
    {
        private ExtendedIdentity<TExtendedUserInfo> identity;
        private string[] roles;

        public ExtendedPrincipal(ExtendedIdentity<TExtendedUserInfo> identity, IEnumerable<string> roles)
        {
            this.identity = identity;
            this.roles = roles.ToArray();
        }

        public ExtendedIdentity<TExtendedUserInfo> Identity
        {
            get { return identity; }
        }

        public IEnumerable<string> Roles
        {
            get { return roles; }
        }

        public bool IsInRole(string role)
        {
            return this.roles.Any(r => r == role);
        }

        public bool IsInAnyRole(params string[] roles)
        {
            return this.roles.Any(r => roles.Contains(r));
        }

        IIdentity IPrincipal.Identity
        {
            get { return identity; }
        }

        bool IPrincipal.IsInRole(string role)
        {
            return this.roles.Any(r => r == role);
        }

        #region Serialization

        private static JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        private static BinaryFormatter binarySerializer = new BinaryFormatter();

        public string ToJson()
        {
            return jsonSerializer.Serialize(new UserMemento<TExtendedUserInfo> { UserInfo = this.identity.UserInfo, Roles = this.roles });
        }

        public static UserMemento<TExtendedUserInfo> FromJson(string json)
        {
            return jsonSerializer.Deserialize<UserMemento<TExtendedUserInfo>>(json);
        }

        public byte[] ToBinary()
        {
            using (var memStream = new System.IO.MemoryStream())
            {
                binarySerializer.Serialize(memStream, new UserMemento<TExtendedUserInfo> { UserInfo = this.identity.UserInfo, Roles = this.roles });
                return memStream.ToArray();
            }
        }

        public static UserMemento<TExtendedUserInfo> FromBinary(byte[] bytes)
        {
            using (var memStream = new System.IO.MemoryStream(bytes))
            {
                return (UserMemento<TExtendedUserInfo>)binarySerializer.Deserialize(memStream);
            }
        }

        public class UserMemento<T>
        {
            public T UserInfo { get; set; }
            public string[] Roles { get; set; }
        }

        #endregion
    }
}
