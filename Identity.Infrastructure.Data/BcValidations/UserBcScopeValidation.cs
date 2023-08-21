using System.Data;
using Dapper;
using Identity.Domain.Validations.Users;
using Identity.Infrastructure.Dapper;

namespace Identity.Infrastructure.ORM.BcValidations;

public class UserBcScopeValidation : IUserBcScopeValidation
{
    private readonly IDbConnection dapperConnection;

    public UserBcScopeValidation(
        DapperContext dapperContext
        )
    {
        dapperConnection = dapperContext.CreateConnection();
    }
    
    public bool IsExistEmail(string email)
    {
        var isExist = dapperConnection.QueryFirstOrDefault<bool>($"select " +
                                    $"case when exists (select * from users where users.email = '{email}')" +
                                    "then 1 else 0 " +
                                    "end");
        
        return isExist;
    }

    public bool IsExistUserName(string userName)
    {
        var isExist = dapperConnection.QueryFirstOrDefault<bool>($"select " +
                                                                 $"case when exists (select * from users where users.username = '{userName}')" +
                                                                 "then 1 else 0 " +
                                                                 "end");
        
        return isExist;
    }
}