using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Discover.Mail
{
    [Serializable]
    public class MailMessage : Discover.DomainModel.IEntity
    {
        public Guid Id { get; protected set; }

        public string BatchId { get; set; }

        public virtual MailAddress From { get; set; }

        public virtual ICollection<MailAddress> To { get; set; }

        public virtual ICollection<MailAddress> ReplyTo { get; set; }

        public virtual ICollection<MailAddress> CC { get; set; }

        public virtual ICollection<MailAddress> BCC { get; set; }

        public virtual ICollection<MailAttachment> Attachments { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }

        public MailMessage()
        {
            Id = SequentialGuid.NewCombGuid();
            From = new MailAddress();
            To = new List<MailAddress>();
            CC = new List<MailAddress>();
            BCC = new List<MailAddress>();
            ReplyTo = new List<MailAddress>();
            Attachments = new List<MailAttachment>();
            this.IsBodyHtml = true;
        }
    }

    [Serializable]
    public class SentMailMessage : MailMessage
    {
        public bool IsSigned { get; set; }

        public bool IsEncrypted { get; set; }

        public DeliveryStatus DeliveryStatus
        {
            get { return (DeliveryStatus)DeliveryStatusEnumValue; }
            set { DeliveryStatusEnumValue = (int)value; }
        }
        protected int DeliveryStatusEnumValue { get; set; }

        public DateTime? DelayUntil { get; set; }

        public DateTime? SentOn { get; set; }

        public DateTime? FailedOn { get; set; }

        public int FailureCount { get; set; }

        public string FailureMessage { get; set; }

        protected SentMailMessage()
        {
        }

        public SentMailMessage(MailMessage message)
        {
            Id = message.Id;
            BatchId = message.BatchId;
            From = message.From;
            To = message.To;
            CC = message.CC;
            BCC = message.BCC;
            Attachments = message.Attachments;
            Subject = message.Subject;
            Body = message.Body;
            IsBodyHtml = IsBodyHtml;
            ReplyTo = message.ReplyTo;
        }

        public MailDeliveryInfo GetMailDeliveryInfo()
        {
            return new MailDeliveryInfo
            {
                MessageId = Id,
                DeliveryStatus = DeliveryStatus,
                SentOn = SentOn,
                FailedOn = FailedOn,
                FailureCount = FailureCount,
                FailureMessage = FailureMessage
            };
        }
    }
    
    [Serializable]
    public class MailAddress : Discover.DomainModel.IEntity
    {
        public Guid Id { get; protected set; }

        public string Address { get; set; }

        public string DisplayName { get; set; }

        public MailAddress()
        {
            Id = SequentialGuid.NewCombGuid();
        }
    }

    [Serializable]
    public class MailAttachment : Discover.DomainModel.IEntity
    {
        public Guid Id { get; protected set; }

        public string Name { get; set; }

        public string ContentType { get; set; }
        
        public byte[] Content { get; set; }

        public string Path { get; set; }

        public MailAttachment()
        {
            Id = SequentialGuid.NewCombGuid();
        }
    }
}
