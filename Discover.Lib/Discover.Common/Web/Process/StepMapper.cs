using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Web.Process
{
    [Serializable]
    public class StepMapper
    {
        public StepMapper()
        {
        }

        public StepMapper(string name, string path)
        {
            this.Name = name;
            this.RoutePath = path;
        }

        public string Name { get; set; }
        public string RoutePath { get; set; }
    }
}
