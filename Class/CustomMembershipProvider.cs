using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using vercom.Models;

namespace vercom.Class
{
    public class CustomMembershipProvider : MembershipProvider
    {
        VERCOMEntities db = new VERCOMEntities();
        private string applicationName;
        public override bool EnablePasswordRetrieval => true;

        public override bool EnablePasswordReset => throw new NotImplementedException();

        public override bool RequiresQuestionAndAnswer => throw new NotImplementedException();

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }
        public override int MaxInvalidPasswordAttempts => throw new NotImplementedException();

        public override int PasswordAttemptWindow => throw new NotImplementedException();

        public override bool RequiresUniqueEmail => throw new NotImplementedException();

        public override MembershipPasswordFormat PasswordFormat => throw new NotImplementedException();

        public override int MinRequiredPasswordLength => throw new NotImplementedException();

        public override int MinRequiredNonAlphanumericCharacters => throw new NotImplementedException();

        public override string PasswordStrengthRegularExpression => throw new NotImplementedException();

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            if (user != null)
            {
                // Verify the old password
                if (user.Password == HashPassword(oldPassword))
                {
                    // Update the password to the new hashed password
                    user.Password = HashPassword(newPassword);
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    throw new MembershipPasswordException("Old password is incorrect.");
                }
            }
            else
            {
                throw new MembershipPasswordException("User not found.");
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (db.Users.Any(u => u.UserName == username))
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            if (db.Users.Any(u => u.Email == email))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            if (password != passwordQuestion)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            // Hash the password
            string passwordHash = HashPassword(password);

            // Create the user
            var user = new Users
            {
                UserName = username,
                Email = email,
                Password = passwordHash,
                IsApproved = isApproved,
                CreationDate = DateTime.Now,
            };

            db.Users.Add(user);
            db.SaveChanges();

            status = MembershipCreateStatus.Success;
            return new MembershipUser("CustomMembershipProvider", username, providerUserKey,
                email, passwordQuestion, null, isApproved, false,
                DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);

        }


        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            return db.Users.Where(x => x.UserName == username && x.Password == hashedPassword).Any();
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}