
namespace Infrastructure.Persistence.SqlConnection;

public class SqlConnectionFactory
{
    private readonly string _connectionString;
    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Microsoft.Data.SqlClient.SqlConnection Create()
    {
        return new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
    }
}
