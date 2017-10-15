using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Discover.Security
{
    public class ExtendedIdentity<TExtendedUserInfo> : IIdentity
    {
        private TExtendedUserInfo extendedInfo;
        private Func<TExtendedUserInfo, string> namePropertyResolver;

        public ExtendedIdentity(TExtendedUserInfo extendedInfo, Func<TExtendedUserInfo, string> namePropertyResolver)
        {
            this.extendedInfo = extendedInfo;
            this.namePropertyResolver = namePropertyResolver;
        }

        public TExtendedUserInfo UserInfo
        {
            get { return extendedInfo; }
        }

        string IIdentity.AuthenticationType
        {
            get { return "Custom"; }
        }

        bool IIdentity.IsAuthenticated
        {
            get { return true; }
        }

        string IIdentity.Name
        {
            get { return namePropertyResolver(extendedInfo); }
        }
    }
}
