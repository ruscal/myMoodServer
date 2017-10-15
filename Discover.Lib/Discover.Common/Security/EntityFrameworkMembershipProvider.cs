using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Web.Security;

namespace Discover.Security
{
    public class EntityFrameworkMembershipProvider<TUserEntity> : MembershipProvider
        where TUserEntity : class, IMembershipUser, new()
    {
        protected Func<ObjectContext> getObjectContext = null;
        protected bool disposeObjectContextAfterUse = true;

        public void SetObjectContextSource(Func<ObjectContext> objectContextSourceFunc, bool disposeAfterUse)
        {
            this.getObjectContext = objectContextSourceFunc;
            this.disposeObjectContextAfterUse = disposeAfterUse;
        }

        #region Properties

        protected string applicationName = "/";

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 5; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 0; }
        }

        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return String.Empty; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return false; }
        }

        #endregion

        #region Functions

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (string.IsNullOrEmpty(username))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (string.IsNullOrEmpty(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (string.IsNullOrEmpty(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }
            if (string.IsNullOrEmpty(password) || password.Length < this.MinRequiredPasswordLength)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            var context = this.getObjectContext();
            try
            {
                var users = context.CreateObjectSet<TUserEntity>();

                if (users.Where(Usr => Usr.UserName == username).Any())
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }

                if (this.RequiresUniqueEmail && users.Where(Usr => Usr.Email == email).Any())
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }

                var now = DateTime.UtcNow;

                var newUser = new TUserEntity
                {
                    Id = SequentialGuid.NewCombGuid(),
                    UserName = username,
                    Password = CryptoHelper.ComputeHash(password),
                    IsApproved = isApproved,
                    Email = email,
                    CreationDate = now,
                    LastPasswordChangedDate = now,
                    LastPasswordFailureDate = now,
                    PasswordFailuresSinceLastSuccess = 0,
                    LastLoginDate = now,
                    LastActivityDate = now,
                    LastLockoutDate = now,
                    IsLockedOut = false
                };

                users.AddObject(newUser);
                context.SaveChanges();
                status = MembershipCreateStatus.Success;
                return new MembershipUser(Membership.Provider.Name, newUser.UserName, newUser.Id, newUser.Email, null, null, newUser.IsApproved, newUser.IsLockedOut, newUser.CreationDate, newUser.LastLoginDate, newUser.LastActivityDate, newUser.LastPasswordChangedDate, newUser.LastLockoutDate);
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            var context = this.getObjectContext();
            try
            {
                var user = context.CreateObjectSet<TUserEntity>().FirstOrDefault(u => u.UserName == username);
                
                if (user == null)
                {
                    return false;
                }
                if (!user.IsApproved)
                {
                    return false;
                }
                if (user.IsLockedOut)
                {
                    return false;
                }

                var verificationSucceeded = user.Password != null && CryptoHelper.VerifyHash(password, user.Password);

                if (verificationSucceeded)
                {
                    user.PasswordFailuresSinceLastSuccess = 0;
                    user.LastLoginDate = DateTime.UtcNow;
                    user.LastActivityDate = DateTime.UtcNow;
                }
                else
                {
                    if (user.PasswordFailuresSinceLastSuccess < MaxInvalidPasswordAttempts)
                    {
                        user.PasswordFailuresSinceLastSuccess += 1;
                        user.LastPasswordFailureDate = DateTime.UtcNow;
                    }
                    else
                    {
                        user.LastPasswordFailureDate = DateTime.UtcNow;
                        user.LastLockoutDate = DateTime.UtcNow;
                        user.IsLockedOut = true;
                    }
                }

                context.SaveChanges();

                return verificationSucceeded;
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            var context = this.getObjectContext();
            try
            {
                var user = context.CreateObjectSet<TUserEntity>().FirstOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    if (userIsOnline)
                    {
                        user.LastActivityDate = DateTime.UtcNow;
                        context.SaveChanges();
                    }
                    return new MembershipUser(Membership.Provider.Name, user.UserName, user.Id, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate);
                }
                
                return null;
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (!(providerUserKey is Guid)) return null;

            var context = this.getObjectContext();
            try
            {
                var user = context.CreateObjectSet<TUserEntity>().FirstOrDefault(u => u.Id == (Guid)providerUserKey);

                if (user != null)
                {
                    if (userIsOnline)
                    {
                        user.LastActivityDate = DateTime.UtcNow;
                        context.SaveChanges();
                    }
                    return new MembershipUser(Membership.Provider.Name, user.UserName, user.Id, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate); 
                }

                return null;
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                return false;
            }
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < this.MinRequiredPasswordLength)
            {
                return false;
            }

            var context = this.getObjectContext();
            try
            {
                var user = context.CreateObjectSet<TUserEntity>().FirstOrDefault(u => u.UserName == username);

                if (user == null) return false;

                var verificationSucceeded = user.Password != null && CryptoHelper.VerifyHash(oldPassword, user.Password);

                if (verificationSucceeded)
                {
                    user.PasswordFailuresSinceLastSuccess = 0;
                    user.Password = CryptoHelper.ComputeHash(newPassword);
                    user.LastPasswordChangedDate = DateTime.UtcNow;
                }
                else
                {
                    if (user.PasswordFailuresSinceLastSuccess < MaxInvalidPasswordAttempts)
                    {
                        user.PasswordFailuresSinceLastSuccess += 1;
                        user.LastPasswordFailureDate = DateTime.UtcNow;
                    }
                    else
                    {
                        user.LastPasswordFailureDate = DateTime.UtcNow;
                        user.LastLockoutDate = DateTime.UtcNow;
                        user.IsLockedOut = true;
                    }
                }

                context.SaveChanges();

                return verificationSucceeded;
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            
            var context = this.getObjectContext();
            try
            {
                var user = context.CreateObjectSet<TUserEntity>().FirstOrDefault(u => u.UserName == username);

                if (user == null) return null;

                var newPassword = CryptoHelper.GenerateRandomPassword(Math.Max(8, this.MinRequiredPasswordLength));
                
                user.PasswordFailuresSinceLastSuccess = 0;
                user.Password = CryptoHelper.ComputeHash(newPassword);
                user.LastPasswordChangedDate = DateTime.UtcNow;
                
                context.SaveChanges();

                return newPassword;
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override bool UnlockUser(string username)
        {
            var context = this.getObjectContext();
            try
            {
                var user = context.CreateObjectSet<TUserEntity>().FirstOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    user.IsLockedOut = false;
                    user.PasswordFailuresSinceLastSuccess = 0;
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            var dateActive = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(Convert.ToDouble(Membership.UserIsOnlineTimeWindow)));

            var context = this.getObjectContext();
            try
            {
                return context.CreateObjectSet<TUserEntity>().Where(u => u.LastActivityDate > dateActive).Count();
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            var context = this.getObjectContext();
            try
            {
                var users = context.CreateObjectSet<TUserEntity>();
                var userEntity = users.FirstOrDefault(u => u.Id == (Guid)user.ProviderUserKey);

                if (userEntity != null)
                {
                    if (this.RequiresUniqueEmail && users.Where(u => u.Email == user.Email && u.Id != (Guid)user.ProviderUserKey).Any())
                    {
                        throw new ArgumentException("Email address \"" + user.Email + "\" is already in use");
                    }

                    userEntity.Email = user.Email;
                    userEntity.IsApproved = user.IsApproved;

                    context.SaveChanges();
                }
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            var context = this.getObjectContext();
            try
            {
                var users = context.CreateObjectSet<TUserEntity>();
                var user = users.FirstOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    users.DeleteObject(user);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            var context = this.getObjectContext();
            try
            {
                var user = context.CreateObjectSet<TUserEntity>().FirstOrDefault(u => u.Email == email);

                return (user != null) ? user.UserName : string.Empty;
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();

            var context = this.getObjectContext();
            try
            {
                var users = context.CreateObjectSet<TUserEntity>();

                totalRecords = users.Where(u => u.Email == emailToMatch).Count();

                foreach (var user in users.Where(u => u.Email == emailToMatch).OrderBy(u => u.UserName).Skip(pageIndex * pageSize).Take(pageSize))
                {
                    membershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.Id, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate));
                }
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }

            return membershipUsers;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();

            var context = this.getObjectContext();
            try
            {
                var users = context.CreateObjectSet<TUserEntity>();

                totalRecords = users.Where(u => u.UserName == usernameToMatch).Count();

                foreach (var user in users.Where(u => u.UserName == usernameToMatch).OrderBy(u => u.UserName).Skip(pageIndex * pageSize).Take(pageSize))
                {
                    membershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.Id, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate));
                }
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }

            return membershipUsers;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();

            var context = this.getObjectContext();
            try
            {
                var users = context.CreateObjectSet<TUserEntity>();

                totalRecords = users.Count();

                foreach (var user in users.OrderBy(u => u.UserName).Skip(pageIndex * pageSize).Take(pageSize))
                {
                    membershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.Id, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate));
                }
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }

            return membershipUsers;
        }

        #endregion

        #region Not Supported

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public interface IMembershipUser
    {
        Guid Id { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        bool IsApproved { get; set; }
        string Email { get; set; }
        DateTime CreationDate { get; set; }
        DateTime LastPasswordChangedDate { get; set; }
        DateTime LastPasswordFailureDate { get; set; }
        int PasswordFailuresSinceLastSuccess { get; set; }
        DateTime LastLoginDate { get; set; }
        DateTime LastActivityDate { get; set; }
        DateTime LastLockoutDate { get; set; }
        bool IsLockedOut { get; set; }
    }
}
