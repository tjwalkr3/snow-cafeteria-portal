using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cafeteria.Shared.DTOs.Swipe;
using Dapper;

namespace Cafeteria.Api.Services.Swipes;

public class SwipeService : ISwipeService
{
    private readonly IDbConnection _dbConnection;

    public SwipeService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<SwipeDto> GetSwipesByUserID(int userId)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        const string sql = @"
            SELECT badger_id AS BadgerId, swipe_balance AS SwipeBalance
            FROM cafeteria.customer_swipe
            WHERE badger_id = @UserId";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<SwipeDto>(sql, new { UserId = userId });
        if (result == null)
        {
            throw new KeyNotFoundException($"No swipe data found for user ID {userId}.");
        }
        return result;
    }
}