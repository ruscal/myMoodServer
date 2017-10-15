using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Emailing.Config;

namespace Discover.Emailing
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class EmailAccountHelper
    {
        public static EmailAccount ToEmailAccountDTO(Account configAccount)
        {
            EmailAccount account = new EmailAccount(configAccount.Name, configAccount.EmailFromName, configAccount.EmailAddress, configAccount.Server, configAccount.Port, configAccount.ServerUsername, configAccount.ServerPassword, configAccount.CertificateFilePath, configAccount.CertificatePassword);
            return account;
        }
    }
}
