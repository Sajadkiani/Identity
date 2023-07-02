using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Dapper;

public class DapperContext
{
    private readonly string connectionString;
    public DapperContext(string connectionString)
    {
        this.connectionString = connectionString;
    }
    public IDbConnection CreateConnection()
        => new SqlConnection(connectionString);
}