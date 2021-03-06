﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Discover.DomainModel;
using StructureMap;
using MyMood.Domain;

namespace MyMood.Web
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ReportPassCodeOrAuthenticationRequired : Attribute { }

}