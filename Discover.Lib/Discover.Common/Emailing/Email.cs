using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Discover.Emailing
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class Email
    {
        public Email()
        {
            //Guid = System.Guid.NewGuid().ToString().Replace("-", "");
            Guid = System.Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateLastEdited = DateTime.Now;
            IsHtml = true;
            IsSigned = false;
            IsEncrypted = false;
            SendingStatus = EmailSendStatus.PENDING;
        }

        public Email(
            EmailAccount from,
            EmailContact to,
            string subject,
            string message,
            bool isHtml,
            string batchId,
            DateTime initialSendDate,
            bool isSigned,
            bool isEncrypted
            )
        {
            From = from;
            To.Add(to);
            Subject = subject;
            Message = message;
            IsHtml = isHtml;
            BatchId = batchId;
            SendDate = initialSendDate;
            IsSigned = isSigned;
            IsEncrypted = isEncrypted;
            Guid = System.Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateLastEdited = DateTime.Now;
            SendingStatus = EmailSendStatus.PENDING;
        }

        public Email(
            EmailAccount from,
            List<EmailContact> tos,
            string subject,
            string message,
            bool isHtml,
            string batchId,
            DateTime initialSendDate,
            bool isSigned,
            bool isEncrypted
            )
        {
            From = from;
            To.AddRange(tos);
            Subject = subject;
            Message = message;
            IsHtml = isHtml;
            BatchId = batchId;
            SendDate = initialSendDate;
            IsSigned = isSigned;
            IsEncrypted = isEncrypted;
            Guid = System.Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateLastEdited = DateTime.Now;
            SendingStatus = EmailSendStatus.PENDING;
        }

        public Email(int id,
            string guid,
            EmailAccount from,
            EmailContact to,
            string subject,
            string message,
            bool isHtml,
            string batchId,
            DateTime initialSendDate,
            bool isSigned,
            bool isEncrypted,
            EmailSendStatus sendingStatus,
            DateTime dateCreated,
            string createdBy,
            DateTime dateLastEdited,
            string lastEditedBy)
        {
            Id = id;
            Guid = guid;
            From = from;
            To.Add(to);
            Subject = subject;
            Message = message;
            IsHtml = isHtml;
            BatchId = batchId;
            SendDate = initialSendDate;
            IsSigned = isSigned;
            IsEncrypted = isEncrypted;
            SendingStatus = sendingStatus;
            DateCreated = dateCreated;
            CreatedBy = createdBy;
            DateLastEdited = dateLastEdited;
            LastEditedBy = lastEditedBy;
        }

        public Email(int id,
            string guid,
            EmailAccount from,
            List<EmailContact> tos,
            string subject,
            string message,
            bool isHtml,
            string batchId,
            DateTime initialSendDate,
            bool isSigned,
            bool isEncrypted,
            EmailSendStatus sendingStatus,
            DateTime dateCreated,
            string createdBy,
            DateTime dateLastEdited,
            string lastEditedBy)
        {
            Id = id;
            Guid = guid;
            From = from;
            To.AddRange(tos);
            Subject = subject;
            Message = message;
            IsHtml = isHtml;
            BatchId = batchId;
            SendDate = initialSendDate;
            IsSigned = isSigned;
            IsEncrypted = isEncrypted;
            SendingStatus = sendingStatus;
            DateCreated = dateCreated;
            CreatedBy = createdBy;
            DateLastEdited = dateLastEdited;
            LastEditedBy = lastEditedBy;
        }

        private List<EmailContact> _to = new List<EmailContact>();
        private List<EmailAttachment> _attachments = new List<EmailAttachment>();

        public List<EmailAttachment> Attachments
        {
            get { return _attachments; }
            set { _attachments = value; }
        }


        public List<EmailContact> To
        {
            get { return _to; }
        }

        public String ToField
        {
            get { return String.Join(",", (from t in To select t.ToString()).ToArray<string>()); }
        }

        public int Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateLastEdited { get; set; }
        public string LastEditedBy { get; set; }
        public string Guid { get; set; }
        public EmailAccount From { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }

        [DefaultValue(true)]
        public bool IsHtml { get; set; }
        public string FailedException { get; set; }
        public DateTime? FailedDate { get; set; }
        public int FailAttempts { get; set; }
        public EmailSendStatus SendingStatus { get; set; }
        public string BatchId { get; set; }
        public DateTime SendDate { get; set; }

        [DefaultValue(false)]
        public bool IsSigned { get; set; }
        [DefaultValue(false)]
        public bool IsEncrypted { get; set; }


    }
}
