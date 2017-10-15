using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.DomainModel;

namespace MyMood.Domain.Events
{
    public abstract class UserEvent : IDomainEvent
    {
        public User User { get; protected set; }
    }

    public class UserRegistered : UserEvent
    {
        public string Password { get; protected set; }

        public UserRegistered(User user, string password)
        {
            this.User = user;
            this.Password = password;
        }
    }

    public class UserNameChanged : UserEvent
    {
        public string PreviousUserName { get; protected set; }

        public UserNameChanged(User user, string previousUserName)
        {
            this.User = user;
            this.PreviousUserName = previousUserName;
        }
    }

    public class UserEmailAddressChanged : UserEvent
    {
        public string PreviousEmailAddress { get; protected set; }

        public UserEmailAddressChanged(User user, string previousEmailAddress)
        {
            this.User = user;
            this.PreviousEmailAddress = previousEmailAddress;
        }
    }

    public class UserPasswordResetTokenIssued : UserEvent
    {
        public string Token { get; protected set; }

        public DateTime? ExpiryDate { get; protected set; }

        public UserPasswordResetTokenIssued(User user, string token, DateTime? expiryDate)
        {
            this.User = user;
            this.Token = token;
            this.ExpiryDate = expiryDate;
        }
    }

    public class UserPasswordReset : UserEvent
    {
        public string NewPassword { get; protected set; }

        public UserPasswordReset(User user, string newPassword)
        {
            this.User = user;
            this.NewPassword = newPassword;
        }
    }

    public class UserApprovalStatusChanged : UserEvent
    {
        public UserApprovalStatusChanged(User user)
        {
            this.User = user;
        }
    }

    public class UserLockoutStatusChanged : UserEvent
    {
        public UserLockoutStatusChanged(User user)
        {
            this.User = user;
        }
    }
}
