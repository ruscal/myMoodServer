using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Reflection;
using Discover.Security;
using System.ComponentModel.DataAnnotations;
using Discover.DomainModel;
using Discover;

namespace MyMood.Domain
{
    public class User : Entity
    {
        #region Constants and Static Members

        public static int? MaxLoginAttemptsBeforeLockout { get; private set; }
        public static int? NumberOfDaysUntilPasswordChangeRequired { get; private set; }
        public static int? NumberOfPreviousPasswordsToStore { get; private set; }
        public static int? NumberOfHoursUntilPasswordResetTokenExpires { get; private set; }
        public static string[] PasswordGenerationCharacterSets { get; private set; }

        private static System.Text.RegularExpressions.Regex PasswordComplexityRegex;

        static User()
        {
            MaxLoginAttemptsBeforeLockout = 5;
            NumberOfDaysUntilPasswordChangeRequired = 90;
            NumberOfPreviousPasswordsToStore = 5;
            NumberOfHoursUntilPasswordResetTokenExpires = 24;
            PasswordGenerationCharacterSets = new string[] { "23456789", "BCDFGHJKLMNPQRSTVWXYZ", "bcdfghjkmnpqrstvwxyz", "!?$%^&*@#" };
        }

        public static void ConfigurePasswordPolicy(int? maxLoginAttemptsBeforeLockout, int? numberOfDaysUntilPasswordChangeRequired, int? numberOfPreviousPasswordsToStore, int? numberOfHoursUntilPasswordResetTokenExpires, string passwordComplexityRegex)
        {
            MaxLoginAttemptsBeforeLockout = maxLoginAttemptsBeforeLockout;
            NumberOfDaysUntilPasswordChangeRequired = numberOfDaysUntilPasswordChangeRequired;
            NumberOfPreviousPasswordsToStore = numberOfPreviousPasswordsToStore;
            NumberOfHoursUntilPasswordResetTokenExpires = numberOfHoursUntilPasswordResetTokenExpires;
            PasswordComplexityRegex = string.IsNullOrWhiteSpace(passwordComplexityRegex) ? null : new System.Text.RegularExpressions.Regex(passwordComplexityRegex, System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        public static string GeneratePassword()
        {
            return CryptoHelper.GenerateRandomPassword(8, PasswordGenerationCharacterSets);
        }

        #endregion

        public string UserName { get; protected set; }

        public string PasswordHash { get; protected set; }

        public IEnumerable<string> PreviousPasswordHashes { get; protected set; }

        public string EmailAddress { get; protected set; }

        public string DisplayName { get; protected set; }

        public bool IsApproved { get; protected set; }

        public bool IsLockedOut { get; protected set; }

        public int PasswordFailuresSinceLastSuccess { get; protected set; }

        public string PasswordResetToken { get; protected set; }

        public DateTime? LastLoginDate { get; protected set; }

        public DateTime LastPasswordChangedDate { get; protected set; }

        public DateTime? LastPasswordFailureDate { get; protected set; }

        public DateTime? PasswordExpiryDate { get; protected set; }

        public DateTime? PasswordResetTokenExpiryDate { get; protected set; }

        public IEnumerable<string> Roles { get; protected set; }

        protected string PreviousPasswordHashesString
        {
            get { return string.Join("|", PreviousPasswordHashes.ToArray()); }
            set { PreviousPasswordHashes = string.IsNullOrWhiteSpace(value) ? new List<string>() : new List<string>(value.Split('|').Where(r => !string.IsNullOrWhiteSpace(r))); }
        }

        protected string RolesString
        {
            get { return string.Join("|", Roles.ToArray()); }
            set { Roles = string.IsNullOrWhiteSpace(value) ? new List<string>() : new List<string>(value.Split('|').Where(r => !string.IsNullOrWhiteSpace(r))); }
        }

        protected User()
        {
            PreviousPasswordHashes = new List<string>();
            Roles = new List<string>();
        }

        public User(string userName, string password, string email, string displayName, bool isApproved, params string[] roleNames)
            : this()
        {
            UserName = userName;
            SetPassword(password);
            EmailAddress = email;
            DisplayName = displayName;
            IsApproved = isApproved;

            AddToRoles(roleNames);
        }

        public bool VerifyLoginAttempt(string attemptedPassword)
        {
            var success = false;

            if (!MaxLoginAttemptsBeforeLockout.HasValue || this.PasswordFailuresSinceLastSuccess < MaxLoginAttemptsBeforeLockout)
            {
                success = CryptoHelper.VerifyHash(attemptedPassword, PasswordHash);

                if (success)
                {
                    this.PasswordFailuresSinceLastSuccess = 0;
                    this.LastLoginDate = DateTime.UtcNow;
                }
                else
                {
                    this.PasswordFailuresSinceLastSuccess += 1;
                    this.LastPasswordFailureDate = DateTime.UtcNow;
                }
            }

            if (MaxLoginAttemptsBeforeLockout.HasValue && this.PasswordFailuresSinceLastSuccess >= MaxLoginAttemptsBeforeLockout)
            {
                this.IsLockedOut = true;
            }

            return success;
        }

        public void Unlock()
        {
            var previouslyLockedOut = IsLockedOut;
            this.IsLockedOut = false;

            if (previouslyLockedOut)
            {
                DomainEvents.Raise(new Events.UserLockoutStatusChanged(this));
            }
        }

        public void SetApprovalStatus(bool approved)
        {
            var previousStatus = IsApproved;
            this.IsApproved = approved;

            if (approved != previousStatus)
            {
                DomainEvents.Raise(new Events.UserApprovalStatusChanged(this));
            }
        }

        public void SetUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentException("User Name cannot be empty");
            if (userName.Any(c => char.IsWhiteSpace(c))) throw new ArgumentException("User Name cannot contain white-space characters");

            var previousUserName = UserName;
            UserName = userName;

            if (!string.Equals(userName, previousUserName, StringComparison.InvariantCultureIgnoreCase))
            {
                DomainEvents.Raise(new Events.UserNameChanged(this, previousUserName));
            }
        }

        public void SetEmailAddress(string emailAddress)
        {
            var previousEmailAddress = EmailAddress;
            EmailAddress = emailAddress;

            if (!string.Equals(emailAddress, previousEmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                DomainEvents.Raise(new Events.UserEmailAddressChanged(this, previousEmailAddress));
            }
        }

        public void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        public IEnumerable<ValidationResult> ValidateNewPassword(string password)
        {
            return ValidateNewPassword(password, "NewPassword");
        }

        public IEnumerable<ValidationResult> ValidateNewPassword(string password, string memberName)
        {
            if (string.IsNullOrEmpty(password))
            {
                yield return new ValidationResult("Password cannot be empty", Enumerable.Repeat(memberName, 1));
            }
            else if (PasswordComplexityRegex != null && !PasswordComplexityRegex.IsMatch(password))
            {
                yield return new ValidationResult("Password does not meet complexity requirements", Enumerable.Repeat(memberName, 1));
            }
            else if (NumberOfPreviousPasswordsToStore.HasValue && this.PreviousPasswordHashes.Take(NumberOfPreviousPasswordsToStore.Value).Any(p => CryptoHelper.VerifyHash(password, p)))
            {
                yield return new ValidationResult("Password cannot be the same as any of the previous " + NumberOfPreviousPasswordsToStore.ToString(), Enumerable.Repeat(memberName, 1));
            }

            yield break;
        }

        public void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be empty");
            if (PasswordComplexityRegex != null && !PasswordComplexityRegex.IsMatch(password)) throw new ArgumentException("Password does not meet complexity requirements");
            if (NumberOfPreviousPasswordsToStore.HasValue && this.PreviousPasswordHashes.Take(NumberOfPreviousPasswordsToStore.Value).Any(p => CryptoHelper.VerifyHash(password, p))) throw new ArgumentException("Password cannot be the same as any of the previous " + NumberOfPreviousPasswordsToStore.ToString());

            this.PasswordHash = CryptoHelper.ComputeHash(password);
            this.PreviousPasswordHashes = Enumerable.Repeat(this.PasswordHash, 1).Concat(this.PreviousPasswordHashes).Take(NumberOfPreviousPasswordsToStore ?? 0).ToArray();
            this.LastPasswordChangedDate = DateTime.UtcNow;
            this.PasswordExpiryDate = NumberOfDaysUntilPasswordChangeRequired.HasValue ? this.LastPasswordChangedDate.AddDays(NumberOfDaysUntilPasswordChangeRequired.Value) : (DateTime?)null;
        }

        public bool PasswordEquals(string password)
        {
            return CryptoHelper.VerifyHash(password, PasswordHash);
        }

        public string IssuePasswordResetToken()
        {
            this.PasswordResetToken = Guid.NewGuid().ToString("N");
            this.PasswordResetTokenExpiryDate = NumberOfHoursUntilPasswordResetTokenExpires.HasValue ? DateTime.UtcNow.AddHours(NumberOfHoursUntilPasswordResetTokenExpires.Value) : DateTime.MaxValue;
            DomainEvents.Raise(new Events.UserPasswordResetTokenIssued(this, this.PasswordResetToken, this.PasswordResetTokenExpiryDate));
            return this.PasswordResetToken;
        }

        public string ResetPassword()
        {
            var password = CryptoHelper.GenerateRandomPassword(8, PasswordGenerationCharacterSets);
            SetPassword(password);
            this.PasswordResetToken = null;
            this.PasswordResetTokenExpiryDate = null;
            DomainEvents.Raise(new Events.UserPasswordReset(this, password));
            return password;
        }

        public void ExpirePassword()
        {
            this.PasswordExpiryDate = DateTime.UtcNow;
        }

        public bool IsInRole(string roleName)
        {
            return Roles.Any(r => r.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsInAnyRole(params string[] roleNames)
        {
            return roleNames.Any(r => IsInRole(r));
        }

        public void AddToRoles(params string[] roleNames)
        {
            foreach (var roleName in roleNames.Where(r => !string.IsNullOrWhiteSpace(r) && !IsInRole(r)))
            {
                ((ICollection<string>)Roles).Add(roleName);
            }
        }

        public void RemoveFromRoles(params string[] roleNames)
        {
            foreach (var roleName in roleNames.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                ((ICollection<string>)Roles).Remove(roleName);
            }
        }

        public static User RegisterNewUser(IDomainDataContext db, string userName, string password, string emailAddress, string displayName, bool isApproved, params string[] roleNames)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("User Name cannot be blank/empty");
            if (db.Get<User>().Any(u => u.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase))) throw new ArgumentException("User Name is already in use");
            if (!string.IsNullOrWhiteSpace(emailAddress) && !emailAddress.IsValidEmailAddress()) throw new ArgumentException("Email Address does not appear to be valid");

            var user = new User(userName, password, emailAddress, displayName, isApproved);

            db.Add(user);

            DomainEvents.Raise(new Events.UserRegistered(user, password));

            return user;
        }
    }

    /// <summary>
    /// Defines fixed security roles recognized by the system (for use within application/ui code)
    /// </summary>
    public class UserRole
    {
        [Display(Name = "System Admin")]
        public const string SystemAdmin = "SystemAdmin";

        public static IEnumerable<KeyValuePair<string, string>> GetRoles()
        {
            return typeof(UserRole).GetFields().Where(f => f.IsLiteral).Select(f => new KeyValuePair<string, string>(f.Name, f.DisplayName()));
        }
    }
}
