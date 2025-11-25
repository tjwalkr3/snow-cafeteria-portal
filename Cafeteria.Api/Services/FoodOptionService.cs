using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public class FoodOptionService : IFoodOptionService
{
    private readonly IDbConnection _dbConnection;

    public FoodOptionService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<FoodOptionDto> CreateFoodOption(FoodOptionDto foodOptionDto)
    {
        const string sql = @"
            INSERT INTO cafeteria.food_option (food_option_name, in_stock, image_url)
            VALUES (@FoodOptionName, @InStock, @ImageUrl)
            RETURNING id AS Id, food_option_name AS FoodOptionName, in_stock AS InStock, image_url AS ImageUrl;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<FoodOptionDto>(sql, foodOptionDto);
        return result ?? throw new InvalidOperationException("Failed to create food option");
    }

    public async Task<FoodOptionDto?> GetFoodOptionByID(int id)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                food_option_name AS FoodOptionName, 
                in_stock AS InStock, 
                image_url AS ImageUrl
            FROM cafeteria.food_option
            WHERE id = @id;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<FoodOptionDto>(sql, new { id });
        return result;
    }

    public async Task<List<FoodOptionDto>> GetAllFoodOptions()
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                food_option_name AS FoodOptionName, 
                in_stock AS InStock, 
                image_url AS ImageUrl
            FROM cafeteria.food_option
            ORDER BY food_option_name;";

        var result = await _dbConnection.QueryAsync<FoodOptionDto>(sql);
        return result.ToList();
    }

    public async Task<FoodOptionDto?> UpdateFoodOption(int id, FoodOptionDto foodOptionDto)
    {
        const string sql = @"
            UPDATE cafeteria.food_option
            SET food_option_name = @FoodOptionName,
                in_stock = @InStock,
                image_url = @ImageUrl
            WHERE id = @id
            RETURNING id AS Id, food_option_name AS FoodOptionName, in_stock AS InStock, image_url AS ImageUrl;";

        var parameters = new
        {
            id,
            foodOptionDto.FoodOptionName,
            foodOptionDto.InStock,
            foodOptionDto.ImageUrl
        };

        var result = await _dbConnection.QuerySingleOrDefaultAsync<FoodOptionDto>(sql, parameters);
        return result;
    }

    public async Task<bool> DeleteFoodOption(int id)
    {
        const string sql = @"
            DELETE FROM cafeteria.food_option
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }
}
