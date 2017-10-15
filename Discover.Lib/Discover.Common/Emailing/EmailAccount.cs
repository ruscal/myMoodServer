using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Emailing
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class EmailAccount : EmailContact
    {
        public EmailAccount()
            :base()
        {
        }

        public EmailAccount(string name, string emailDisplayName, string emailAddress, string server, int port, string serverUsername, string serverPassword)
            :base(emailAddress, emailDisplayName)
        {
            Name = name;
            Server = server;
            Port = port;
            ServerUsername = serverUsername;
            ServerPassword = serverPassword;
        }

        public EmailAccount(string name, string emailDisplayName, string emailAddress, string server, int port, string serverUsername, string serverPassword, string certificatePath, string certificatePassword)
            : base(emailAddress, emailDisplayName)
        {
            Name = name;
            Server = server;
            Port = port;
            ServerUsername = serverUsername;
            ServerPassword = serverPassword;
            CertificateFilePath = certificatePath;
            CertificatePassword = certificatePassword;
        }

        public string Name { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string ServerUsername { get; set; }
        public string ServerPassword { get; set; }
        public string CertificateFilePath { get; set; }
        public string CertificatePassword { get; set; }
    }
}
