using System;
using SEGREWARDS_PROJECT.Data.Repositories;
using SEGREWARDS_PROJECT.Infrastructure;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IAuditLogRepository _audit;

        public AuthService(IUserRepository users, IAuditLogRepository audit)
        {
            _users = users;
            _audit = audit;
        }

        public AuthResult Login(string studentNumber, string password)
        {
            if (string.IsNullOrWhiteSpace(studentNumber)) return AuthResult.Fail("Student number is required.");
            if (string.IsNullOrEmpty(password)) return AuthResult.Fail("Password is required.");

            var user = _users.GetByStudentNumber(studentNumber.Trim());
            if (user == null || !user.IsActive)
            {
                return AuthResult.Fail("Invalid student number or password.");
            }

            if (!PasswordHasher.Verify(password, user.PasswordSalt, user.PasswordHash))
            {
                return AuthResult.Fail("Invalid student number or password.");
            }

            _audit.Insert(user.Id, "login", "user", user.Id.ToString(), null);
            return AuthResult.Ok(user);
        }

        public AuthResult Register(string studentNumber, string email, string password, string fullName)
        {
            if (string.IsNullOrWhiteSpace(studentNumber)) return AuthResult.Fail("Student number is required.");
            if (string.IsNullOrWhiteSpace(password)) return AuthResult.Fail("Password is required.");
            if (string.IsNullOrWhiteSpace(fullName)) return AuthResult.Fail("Full name is required.");

            if (_users.StudentNumberExists(studentNumber.Trim()))
            {
                return AuthResult.Fail("That student number is already registered.");
            }

            if (!string.IsNullOrWhiteSpace(email) && _users.EmailExists(email))
            {
                return AuthResult.Fail("That email is already in use.");
            }

            var salt = PasswordHasher.CreateSalt();
            var hash = PasswordHasher.Hash(password, salt);

            var user = new UserRecord
            {
                StudentNumber = studentNumber.Trim(),
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                PasswordHash = hash,
                PasswordSalt = salt,
                FullName = fullName.Trim(),
                YearLevel = null,
                Course = null,
                RoleId = 1,
                EcoPointsBalance = 0,
                IsActive = true
            };

            var id = _users.Insert(user);
            user.Id = id;

            _audit.Insert(id, "register", "user", id.ToString(), null);
            return AuthResult.Ok(user);
        }
    }
}
