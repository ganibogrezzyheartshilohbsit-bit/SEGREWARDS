namespace SEGREWARDS_PROJECT.Services
{
    public interface IAuthService
    {
        AuthResult Login(string studentNumber, string password);

        AuthResult Register(string studentNumber, string email, string password, string fullName);
    }
}
