using System.Data;
using Dapper;

namespace Cafeteria.Api.Services.Customer;

public class CustomerService : ICustomerService
{
    private readonly IDbConnection _dbConnection;

    public CustomerService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task EnsureCustomerExists(string email, string custName, int badgerId = 0)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        const string checkSql = @"
            SELECT COUNT(*) 
            FROM cafeteria.customer 
            WHERE email = @Email";

        var exists = await _dbConnection.ExecuteScalarAsync<int>(checkSql, new { Email = email });

        if (exists == 0)
        {
            const string insertSql = @"
                INSERT INTO cafeteria.customer (email, badger_id, cust_name)
                VALUES (@Email, @BadgerId, @CustName)";

            await _dbConnection.ExecuteAsync(insertSql, new 
            { 
                Email = email, 
                BadgerId = badgerId, 
                CustName = custName 
            });
        }
    }
}