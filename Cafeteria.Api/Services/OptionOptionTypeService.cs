using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public class OptionOptionTypeService : IOptionOptionTypeService
{
    private readonly IDbConnection _dbConnection;

    public OptionOptionTypeService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<OptionOptionTypeDto> CreateOptionOptionType(OptionOptionTypeDto optionOptionTypeDto)
    {
        const string sql = @"
            INSERT INTO cafeteria.option_option_type (food_option_id, food_option_type_id)
            VALUES (@FoodOptionId, @FoodOptionTypeId)
            RETURNING id AS Id, food_option_id AS FoodOptionId, food_option_type_id AS FoodOptionTypeId;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<OptionOptionTypeDto>(sql, optionOptionTypeDto);
        return result ?? throw new InvalidOperationException("Failed to create option-option-type mapping");
    }

    public async Task<OptionOptionTypeDto?> GetOptionOptionTypeByID(int id)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                food_option_id AS FoodOptionId, 
                food_option_type_id AS FoodOptionTypeId
            FROM cafeteria.option_option_type
            WHERE id = @id;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<OptionOptionTypeDto>(sql, new { id });
        return result;
    }

    public async Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes()
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                food_option_id AS FoodOptionId, 
                food_option_type_id AS FoodOptionTypeId
            FROM cafeteria.option_option_type
            ORDER BY food_option_type_id, food_option_id;";

        var result = await _dbConnection.QueryAsync<OptionOptionTypeDto>(sql);
        return result.ToList();
    }

    public async Task<OptionOptionTypeDto?> UpdateOptionOptionTypeById(int id, OptionOptionTypeDto optionOptionTypeDto)
    {
        const string sql = @"
            UPDATE cafeteria.option_option_type
            SET food_option_id = @FoodOptionId,
                food_option_type_id = @FoodOptionTypeId
            WHERE id = @id
            RETURNING id AS Id, food_option_id AS FoodOptionId, food_option_type_id AS FoodOptionTypeId;";

        var parameters = new
        {
            id,
            optionOptionTypeDto.FoodOptionId,
            optionOptionTypeDto.FoodOptionTypeId
        };

        var result = await _dbConnection.QuerySingleOrDefaultAsync<OptionOptionTypeDto>(sql, parameters);
        return result;
    }

    public async Task<bool> DeleteOptionOptionTypeById(int id)
    {
        const string sql = @"
            DELETE FROM cafeteria.option_option_type
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }
}
