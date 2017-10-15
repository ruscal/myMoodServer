using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Security
{
    /// <summary>
    /// This attibute is used to mark MVC action methods which may be accessed by anonymous (i.e. unauthenticated) users 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AllowAnonymousAttribute : Attribute { }
}
