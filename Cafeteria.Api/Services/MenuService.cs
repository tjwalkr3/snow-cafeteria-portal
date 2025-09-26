using System.Data;
using Dapper;
using Cafeteria.Api.DTOs;

namespace Cafeteria.Api.Services;

public class MenuService
{
    private readonly IDbConnection _dbConnection;

    public MenuService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<IngredientDto>> GetIngredientsForType(int ingredientTypeId)
    {
        const string sql = @"
            SELECT i.id, i.ingredient_name as IngredientName, i.image_url as ImageUrl, i.ingredient_price as IngredientPrice
            FROM cafeteria.ingredient i
            INNER JOIN cafeteria.ingredient_ingredient_type iit ON i.id = iit.ingredient_id
            WHERE iit.ingredient_type_id = @IngredientTypeId";

        return await _dbConnection.QueryAsync<IngredientDto>(sql, new { IngredientTypeId = ingredientTypeId });
    }

    public async Task<IEnumerable<IngredientTypeDto>> GetIngredientTypesForFoodItem(int foodItemId)
    {
        const string sql = @"
            SELECT it.id, it.type_name as TypeName, it.quantity
            FROM cafeteria.ingredient_type it
            INNER JOIN cafeteria.food_item_ingredient_type fiit ON it.id = fiit.ingredient_type_id
            WHERE fiit.food_item_id = @FoodItemId";

        return await _dbConnection.QueryAsync<IngredientTypeDto>(sql, new { FoodItemId = foodItemId });
    }

    public async Task<IEnumerable<IngredientDto>> GetDefaultIngredientsForFoodItem(int foodItemId)
    {
        const string sql = @"
            SELECT i.id, i.ingredient_name as IngredientName, i.image_url as ImageUrl, i.ingredient_price as IngredientPrice
            FROM cafeteria.ingredient i
            INNER JOIN cafeteria.food_builder_item_ingredient fbii ON i.id = fbii.ingredient_id
            WHERE fbii.food_item_id = @FoodItemId";

        return await _dbConnection.QueryAsync<IngredientDto>(sql, new { FoodItemId = foodItemId });
    }

    public async Task<IEnumerable<FoodItemDto>> GetFoodItemsByStation(int stationId)
    {
        const string sql = @"
            SELECT id, station_id as StationId, item_description as ItemDescription,
                   image_url as ImageUrl, item_price as ItemPrice
            FROM cafeteria.food_item
            WHERE station_id = @StationId";

        return await _dbConnection.QueryAsync<FoodItemDto>(sql, new { StationId = stationId });
    }

    public async Task<IEnumerable<IngredientDto>> GetAllIngredients()
    {
        const string sql = @"
            SELECT id, ingredient_name as IngredientName, image_url as ImageUrl, ingredient_price as IngredientPrice
            FROM cafeteria.ingredient";

        return await _dbConnection.QueryAsync<IngredientDto>(sql);
    }

    public async Task<IEnumerable<IngredientTypeDto>> GetAllIngredientTypes()
    {
        const string sql = @"
            SELECT id, type_name as TypeName, quantity
            FROM cafeteria.ingredient_type";

        return await _dbConnection.QueryAsync<IngredientTypeDto>(sql);
    }

    public async Task<IEnumerable<FoodItemDto>> GetAllFoodItems()
    {
        const string sql = @"
            SELECT id, station_id as StationId, item_description as ItemDescription,
                   image_url as ImageUrl, item_price as ItemPrice
            FROM cafeteria.food_item";

        return await _dbConnection.QueryAsync<FoodItemDto>(sql);
    }
}