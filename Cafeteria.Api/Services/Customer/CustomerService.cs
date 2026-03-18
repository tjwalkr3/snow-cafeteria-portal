using System.Data;
using Cafeteria.Shared.DTOs.Customer;
using Dapper;

namespace Cafeteria.Api.Services.Customer;

public class CustomerService : ICustomerService
{
    private readonly IDbConnection _dbConnection;

    public CustomerService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task EnsureCustomerExists(string email, string custName)
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
                INSERT INTO cafeteria.customer (email, cust_name)
                VALUES (@Email, @CustName)";

            await _dbConnection.ExecuteAsync(insertSql, new
            {
                Email = email,
                CustName = custName
            });
        }
    }

    public async Task<List<CustomerRoleDto>> GetAllCustomersWithRoles(string? search = null)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        const string sql = @"
            SELECT email AS Email, cust_name AS CustName, badger_id AS BadgerId, user_role AS UserRole
            FROM cafeteria.customer
            WHERE (@Search IS NULL
                OR cust_name ILIKE '%' || @Search || '%'
                OR email ILIKE '%' || @Search || '%')
            ORDER BY cust_name ASC";

        var results = await _dbConnection.QueryAsync<CustomerRoleDto>(sql, new { Search = string.IsNullOrWhiteSpace(search) ? null : search });
        return results.ToList();
    }

    public async Task<bool> ToggleFoodServiceRole(string email)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        const string getRoleSql = @"
            SELECT user_role FROM cafeteria.customer WHERE email = @Email LIMIT 1";

        var currentRole = await _dbConnection.QuerySingleOrDefaultAsync<string?>(getRoleSql, new { Email = email });

        if (string.Equals(currentRole, "admin", StringComparison.OrdinalIgnoreCase))
            return false;

        var newRole = string.Equals(currentRole, "food-service", StringComparison.OrdinalIgnoreCase)
            ? null
            : "food-service";

        const string updateSql = @"
            UPDATE cafeteria.customer SET user_role = @NewRole WHERE email = @Email";

        await _dbConnection.ExecuteAsync(updateSql, new { NewRole = newRole, Email = email });
        return true;
    }
}