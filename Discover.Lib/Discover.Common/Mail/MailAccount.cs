using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Mail
{
    [Serializable]
    public class MailAccount
    {
        public string Name { get; set; }

        public string SenderEmailAddress { get; set; }

        public string SenderDisplayName { get; set; }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public string ServerUsername { get; set; }

        public string ServerPassword { get; set; }

        public string CertificateFilePath { get; set; }

        public string CertificatePassword { get; set; }

        public string ReplyTo { get; set; }

        public MailAccount()
        {
        }

        public MailAccount(string name, string senderEmailAddress, string senderDisplayName, string server, int port, string serverUsername, string serverPassword)
        {
            Name = name;
            SenderEmailAddress = senderEmailAddress;
            SenderDisplayName = senderDisplayName;
            ServerAddress = server;
            ServerPort = port;
            ServerUsername = serverUsername;
            ServerPassword = serverPassword;
        }

        public MailAccount(string name, string senderEmailAddress, string senderDisplayName, string server, int port, string serverUsername, string serverPassword, string certificatePath, string certificatePassword, string replyTo)
        {
            Name = name;
            SenderEmailAddress = senderEmailAddress;
            SenderDisplayName = senderDisplayName;
            ServerAddress = server;
            ServerPort = port;
            ServerUsername = serverUsername;
            ServerPassword = serverPassword;
            CertificateFilePath = certificatePath;
            CertificatePassword = certificatePassword;
            ReplyTo = replyTo;
        }

        public MailAccount(Config.MailAccount accountConfig)
        {
            Name = accountConfig.Name;
            SenderEmailAddress = accountConfig.SenderEmailAddress;
            SenderDisplayName = accountConfig.SenderDisplayName;
            ServerAddress = accountConfig.ServerAddress;
            ServerPort = accountConfig.ServerPort;
            ServerUsername = accountConfig.ServerUsername;
            ServerPassword = accountConfig.ServerPassword;
            CertificateFilePath = accountConfig.CertificateFilePath;
            CertificatePassword = accountConfig.CertificatePassword;
            ReplyTo = accountConfig.ReplyTo;
        }
    }
}
