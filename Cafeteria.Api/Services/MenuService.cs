using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs;
using Cafeteria.Api.Services;

namespace Cafeteria.Api.Services;

public class MenuService : IMenuService
{
    private readonly IDbConnection _dbConnection;

    public MenuService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<LocationDto>> GetAllLocations()
    {
        const string sql = @"select * from cafeteria_location";

        var result = await _dbConnection.QueryAsync<LocationDto>(sql);
        return result.ToList();
    }

    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        const string sql = @"
            SELECT *
            FROM cafeteria.station
            WHERE location_id = @location_id";

        var result = await _dbConnection.QueryAsync<StationDto>(sql, new { location_id = locationId });
        return result.ToList();
    }

    public async Task<List<FoodItemDto>> GetFoodItemsByStation(int stationId)
    {
        const string sql = @"
            SELECT *
            FROM cafeteria.food_item
            WHERE station_id = @station_id";

        var result = await _dbConnection.QueryAsync<FoodItemDto>(sql, new { station_id = stationId });
        return result.ToList();
    }

    public async Task<List<IngredientTypeDto>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        const string sql = @"
            SELECT it.id, it.type_name TypeName, it.quantity
            FROM cafeteria.ingredient_type it
            JOIN cafeteria.food_item_ingredient_type fiit ON it.id = fiit.ingredient_type_id
            WHERE fiit.food_item_id = @food_item_id";

        var result = await _dbConnection.QueryAsync<IngredientTypeDto>(sql, new { food_item_id = foodItemId });
        return result.ToList();
    }

    public async Task<List<IngredientDto>> GetIngredientsByType(int ingredientTypeId)
    {
        const string sql = @"
            SELECT i.id, i.ingredient_name IngredientName, i.image_url ImageUrl, i.ingredient_price IngredientPrice
            FROM cafeteria.ingredient i
            JOIN cafeteria.ingredient_ingredient_type iit ON i.id = iit.ingredient_id
            WHERE iit.ingredient_type_id = @ingredient_type_id";

        var result = await _dbConnection.QueryAsync<IngredientDto>(sql, new { ingredient_type_id = ingredientTypeId });
        return result.ToList();
    }

    public async Task<IngredientDto> GetIngredientById(int ingredientId)
    {
        const string sql = @"
            SELECT id, ingredient_name IngredientName, image_url ImageUrl, ingredient_price IngredientPrice
            FROM cafeteria.ingredient
            WHERE id = @ingredient_id";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<IngredientDto>(sql, new { ingredient_id = ingredientId });
        return result ?? throw new InvalidOperationException($"Ingredient with ID {ingredientId} not found.");
    }
}