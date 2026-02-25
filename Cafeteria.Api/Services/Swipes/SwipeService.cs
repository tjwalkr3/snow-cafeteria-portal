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

    public async Task<SwipeDto?> GetSwipesByUserID(int userId)
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

    public async Task<SwipeDto> GetSwipesByEmail(string email)


    public async Task<List<CustomerSwipeDto>> GetAllCustomers()
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        const string sql = @"
            SELECT 
                c.cust_name AS CustName,
                c.email AS Email,
                c.badger_id AS BadgerId,
                CASE 
                    WHEN latest_swipe.swipe_balance IS NULL THEN NULL
                    WHEN latest_swipe.end_date IS NULL THEN latest_swipe.swipe_balance
                    WHEN latest_swipe.end_date < CURRENT_DATE THEN NULL
                    ELSE latest_swipe.swipe_balance
                END AS SwipeCount,
                CASE 
                    WHEN latest_swipe.swipe_balance IS NULL THEN 'Not Enrolled'
                    WHEN latest_swipe.end_date IS NULL THEN 'Active'
                    WHEN latest_swipe.end_date < CURRENT_DATE THEN 'Expired'
                    ELSE 'Active'
                END AS Status
            FROM cafeteria.customer c
            LEFT JOIN LATERAL (
                SELECT swipe_balance, end_date
                FROM cafeteria.customer_swipe
                WHERE badger_id = c.badger_id
                ORDER BY end_date DESC NULLS FIRST
                LIMIT 1
            ) latest_swipe ON true
            ORDER BY c.cust_name";

        var result = await _dbConnection.QueryAsync<CustomerSwipeDto>(sql);
        return result.ToList();
    }
}