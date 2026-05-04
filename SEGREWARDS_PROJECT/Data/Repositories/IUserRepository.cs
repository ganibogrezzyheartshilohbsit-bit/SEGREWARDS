using System.Collections.Generic;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public interface IUserRepository
    {
        UserRecord GetByStudentNumber(string studentNumber);
        UserRecord GetById(long userId);
        bool StudentNumberExists(string studentNumber);
        bool EmailExists(string email);
        long Insert(UserRecord user);
        void UpdateEcoPoints(long userId, int newBalance);
        void UpdateProfile(long userId, string fullName, string yearLevel, string course);
    }
}
