using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Services
{
    public sealed class AuthResult
    {
        private AuthResult(bool success, string message, UserRecord user)
        {
            Success = success;
            Message = message;
            User = user;
        }

        public bool Success { get; }
        public string Message { get; }
        public UserRecord User { get; }

        public static AuthResult Ok(UserRecord user)
        {
            return new AuthResult(true, null, user);
        }

        public static AuthResult Fail(string message)
        {
            return new AuthResult(false, message, null);
        }
    }
}
