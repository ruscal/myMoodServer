using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Logging
{
    public class LogItem
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string User { get; set; }
        public string SessionId { get; set; }
    }
}
