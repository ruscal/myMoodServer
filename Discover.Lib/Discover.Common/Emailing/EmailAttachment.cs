using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Emailing
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class EmailAttachment
    {
        public EmailAttachment()
        {
        }

        public EmailAttachment(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public EmailAttachment(int id, string name, string path)
        {
            Id = id;
            Name = name;
            Path = path;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
