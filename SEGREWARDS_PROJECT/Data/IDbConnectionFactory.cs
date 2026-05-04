using MySql.Data.MySqlClient;

namespace SEGREWARDS_PROJECT.Data
{
    public interface IDbConnectionFactory
    {
        MySqlConnection CreateOpenConnection();
    }
}
