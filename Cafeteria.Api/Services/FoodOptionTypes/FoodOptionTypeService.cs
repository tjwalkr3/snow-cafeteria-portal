using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.FoodOptionTypes;

public class FoodOptionTypeService : IFoodOptionTypeService
{
    private readonly IDbConnection _dbConnection;

    public FoodOptionTypeService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<FoodOptionTypeDto> CreateFoodType(FoodOptionTypeDto foodTypeDto)
    {
        const string sql = @"
            INSERT INTO cafeteria.food_option_type (food_option_type_name, num_included, max_amount, food_option_price, entree_id, side_id)
            VALUES (@FoodOptionTypeName, @NumIncluded, @MaxAmount, @FoodOptionPrice, @EntreeId, @SideId)
            RETURNING id AS Id, food_option_type_name AS FoodOptionTypeName, num_included AS NumIncluded, max_amount AS MaxAmount, food_option_price AS FoodOptionPrice, entree_id AS EntreeId, side_id AS SideId;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<FoodOptionTypeDto>(sql, foodTypeDto);
        return result ?? throw new InvalidOperationException("Failed to create food type");
    }

    public async Task<FoodOptionTypeDto?> GetFoodTypeByID(int id)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                food_option_type_name AS FoodOptionTypeName, 
                num_included AS NumIncluded, 
                max_amount AS MaxAmount, 
                food_option_price AS FoodOptionPrice, 
                entree_id AS EntreeId, 
                side_id AS SideId
            FROM cafeteria.food_option_type
            WHERE id = @id;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<FoodOptionTypeDto>(sql, new { id });
        return result;
    }

    public async Task<List<FoodOptionTypeDto>> GetAllFoodTypes()
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                food_option_type_name AS FoodOptionTypeName, 
                num_included AS NumIncluded, 
                max_amount AS MaxAmount, 
                food_option_price AS FoodOptionPrice, 
                entree_id AS EntreeId, 
                side_id AS SideId
            FROM cafeteria.food_option_type
            ORDER BY food_option_type_name;";

        var result = await _dbConnection.QueryAsync<FoodOptionTypeDto>(sql);
        return result.ToList();
    }

    public async Task<List<FoodOptionTypeDto>> GetOptionTypesByEntree(int entreeId)
    {
        string sql = @"
        SELECT 
            id AS Id,
            food_option_type_name AS FoodOptionTypeName,
            num_included AS NumIncluded,
            max_amount AS MaxAmount,
            food_option_price AS FoodOptionPrice,
            entree_id AS EntreeId,
            side_id AS SideId
        FROM cafeteria.food_option_type
        WHERE entree_id = @entreeId;";
        var result = await _dbConnection.QueryAsync<FoodOptionTypeDto>(sql, new { entreeId });
        return result.ToList();
    }

    public async Task<List<FoodOptionTypeWithOptionsDto>> GetOptionTypesWithOptionsByEntree(int entreeId)
    {
        var optionTypes = await GetOptionTypesByEntree(entreeId);
        var result = new List<FoodOptionTypeWithOptionsDto>();

        foreach (var optionType in optionTypes)
        {
            string sql = @"
            SELECT 
                fo.id AS Id,
                fo.food_option_name AS FoodOptionName, 
                fo.in_stock AS InStock, 
                fo.image_url AS ImageUrl
            FROM cafeteria.food_option fo
            INNER JOIN cafeteria.option_option_type oot ON fo.id = oot.food_option_id
            WHERE oot.food_option_type_id = @optionTypeId;";

            var options = await _dbConnection.QueryAsync<FoodOptionDto>(sql, new { optionTypeId = optionType.Id });

            result.Add(new FoodOptionTypeWithOptionsDto
            {
                OptionType = optionType,
                Options = options.ToList()
            });
        }

        return result;
    }

    public async Task<FoodOptionTypeDto?> UpdateFoodType(int id, FoodOptionTypeDto foodTypeDto)
    {
        const string sql = @"
            UPDATE cafeteria.food_option_type
            SET food_option_type_name = @FoodOptionTypeName,
                num_included = @NumIncluded,
                max_amount = @MaxAmount,
                food_option_price = @FoodOptionPrice,
                entree_id = @EntreeId,
                side_id = @SideId
            WHERE id = @id
            RETURNING id AS Id, food_option_type_name AS FoodOptionTypeName, num_included AS NumIncluded, max_amount AS MaxAmount, food_option_price AS FoodOptionPrice, entree_id AS EntreeId, side_id AS SideId;";

        var parameters = new
        {
            id,
            foodTypeDto.FoodOptionTypeName,
            foodTypeDto.NumIncluded,
            foodTypeDto.MaxAmount,
            foodTypeDto.FoodOptionPrice,
            foodTypeDto.EntreeId,
            foodTypeDto.SideId
        };

        var result = await _dbConnection.QuerySingleOrDefaultAsync<FoodOptionTypeDto>(sql, parameters);
        return result;
    }

    public async Task<bool> DeleteFoodType(int id)
    {
        const string sql = @"
            DELETE FROM cafeteria.food_option_type
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }
}
