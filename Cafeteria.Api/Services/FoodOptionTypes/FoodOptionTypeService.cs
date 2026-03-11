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

    public async Task<FoodOptionTypeDto> CreateFoodOptionType(FoodOptionTypeDto foodTypeDto)
    {
        const string sql = @"
            INSERT INTO cafeteria.food_option_type (food_option_type_name, required_amount, included_amount, max_amount, food_option_price, entree_id, side_id)
            VALUES (@FoodOptionTypeName, @RequiredAmount, @IncludedAmount, @MaxAmount, @FoodOptionPrice, @EntreeId, @SideId)
            RETURNING id AS Id, food_option_type_name AS FoodOptionTypeName, required_amount AS RequiredAmount, included_amount AS IncludedAmount, max_amount AS MaxAmount, food_option_price AS FoodOptionPrice, entree_id AS EntreeId, side_id AS SideId;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<FoodOptionTypeDto>(sql, foodTypeDto);
        return result ?? throw new InvalidOperationException("Failed to create food type");
    }

    public async Task<FoodOptionTypeDto?> GetFoodOptionTypeByID(int id)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                food_option_type_name AS FoodOptionTypeName, 
                required_amount AS RequiredAmount, 
                included_amount AS IncludedAmount, 
                max_amount AS MaxAmount, 
                food_option_price AS FoodOptionPrice, 
                entree_id AS EntreeId, 
                side_id AS SideId
            FROM cafeteria.food_option_type
            WHERE id = @id;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<FoodOptionTypeDto>(sql, new { id });
        return result;
    }

    public async Task<List<FoodOptionTypeDto>> GetAllFoodOptionTypes()
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                food_option_type_name AS FoodOptionTypeName, 
                required_amount AS RequiredAmount, 
                included_amount AS IncludedAmount, 
                max_amount AS MaxAmount, 
                food_option_price AS FoodOptionPrice, 
                entree_id AS EntreeId, 
                side_id AS SideId
            FROM cafeteria.food_option_type
            ORDER BY food_option_type_name;";

        var result = await _dbConnection.QueryAsync<FoodOptionTypeDto>(sql);
        return result.ToList();
    }

    public async Task<List<FoodOptionTypeDto>> GetFoodOptionTypesByEntreeId(int entreeId)
    {
        string sql = @"
        SELECT 
            id AS Id,
            food_option_type_name AS FoodOptionTypeName,
            required_amount AS RequiredAmount,
            included_amount AS IncludedAmount,
            max_amount AS MaxAmount,
            food_option_price AS FoodOptionPrice,
            entree_id AS EntreeId,
            side_id AS SideId
        FROM cafeteria.food_option_type
        WHERE entree_id = @entreeId;";
        var result = await _dbConnection.QueryAsync<FoodOptionTypeDto>(sql, new { entreeId });
        return result.ToList();
    }

    public async Task<List<FoodOptionTypeWithOptionsDto>> GetFoodOptionTypesWithOptionsByEntreeId(int entreeId)
    {
        const string sql = @"
            SELECT
                fot.id AS Id,
                fot.food_option_type_name AS FoodOptionTypeName,
                fot.required_amount AS RequiredAmount,
                fot.included_amount AS IncludedAmount,
                fot.max_amount AS MaxAmount,
                fot.food_option_price AS FoodOptionPrice,
                fot.entree_id AS EntreeId,
                fot.side_id AS SideId,
                fo.id AS Id,
                fo.food_option_name AS FoodOptionName,
                fo.in_stock AS InStock,
                fo.image_url AS ImageUrl
            FROM cafeteria.food_option_type fot
            LEFT JOIN cafeteria.option_option_type oot ON fot.id = oot.food_option_type_id
            LEFT JOIN cafeteria.food_option fo ON oot.food_option_id = fo.id
            WHERE fot.entree_id = @entreeId
            ORDER BY fot.id;";

        var lookup = new Dictionary<int, FoodOptionTypeWithOptionsDto>();

        await _dbConnection.QueryAsync<FoodOptionTypeDto, FoodOptionDto, FoodOptionTypeWithOptionsDto>(
            sql,
            (optionType, option) =>
            {
                if (!lookup.TryGetValue(optionType.Id, out var withOptions))
                {
                    withOptions = new FoodOptionTypeWithOptionsDto { OptionType = optionType };
                    lookup[optionType.Id] = withOptions;
                }
                if (option?.Id > 0)
                    withOptions.Options.Add(option);
                return withOptions;
            },
            new { entreeId },
            splitOn: "Id");

        return lookup.Values.ToList();
    }

    public async Task<FoodOptionTypeDto?> UpdateFoodOptionTypeById(int id, FoodOptionTypeDto foodTypeDto)
    {
        const string sql = @"
            UPDATE cafeteria.food_option_type
            SET food_option_type_name = @FoodOptionTypeName,
                required_amount = @RequiredAmount,
                included_amount = @IncludedAmount,
                max_amount = @MaxAmount,
                food_option_price = @FoodOptionPrice,
                entree_id = @EntreeId,
                side_id = @SideId
            WHERE id = @id
            RETURNING id AS Id, food_option_type_name AS FoodOptionTypeName, required_amount AS RequiredAmount, included_amount AS IncludedAmount, max_amount AS MaxAmount, food_option_price AS FoodOptionPrice, entree_id AS EntreeId, side_id AS SideId;";

        var parameters = new
        {
            id,
            foodTypeDto.FoodOptionTypeName,
            foodTypeDto.RequiredAmount,
            foodTypeDto.IncludedAmount,
            foodTypeDto.MaxAmount,
            foodTypeDto.FoodOptionPrice,
            foodTypeDto.EntreeId,
            foodTypeDto.SideId
        };

        var result = await _dbConnection.QuerySingleOrDefaultAsync<FoodOptionTypeDto>(sql, parameters);
        return result;
    }

    public async Task<bool> DeleteFoodOptionTypeById(int id)
    {
        const string sql = @"
            DELETE FROM cafeteria.food_option_type
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }
}
