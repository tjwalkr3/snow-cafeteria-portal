using System.Data;
using Dapper;
using Cafeteria.Shared.DTOsOld;
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
        const string sql = @"
            SELECT 
                id AS Id, 
                location_name AS LocationName, 
                location_description AS LocationDescription, 
                image_url AS ImageUrl
            FROM cafeteria.cafeteria_location";

        var result = await _dbConnection.QueryAsync<LocationDto>(sql);
        return result.ToList();
    }

    public async Task<List<StationDtoOld>> GetStationsByLocation(int locationId)
    {
        const string sql = @"
            SELECT 
                id,
                location_id AS LocationId,
                station_name AS StationName,
                station_description AS StationDescription
            FROM cafeteria.station
            WHERE location_id = @location_id";

        var result = await _dbConnection.QueryAsync<StationDtoOld>(sql, new { location_id = locationId });
        return result.ToList();
    }

    public async Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId)
    {
        const string sql = @"
            SELECT 
                id,
                station_id AS StationId,
                item_description AS ItemDescription,
                image_url AS ImageUrl,
                item_price AS ItemPrice
            FROM cafeteria.food_item
            WHERE station_id = @station_id";

        var result = await _dbConnection.QueryAsync<FoodItemDtoOld>(sql, new { station_id = stationId });
        return result.ToList();
    }

    public async Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        const string sql = @"
            SELECT 
                it.id, 
                it.type_name AS TypeName, 
                it.quantity
            FROM cafeteria.ingredient_type it
            JOIN cafeteria.food_item_ingredient_type fiit ON it.id = fiit.ingredient_type_id
            WHERE fiit.food_item_id = @food_item_id";

        var result = await _dbConnection.QueryAsync<IngredientTypeDtoOld>(sql, new { food_item_id = foodItemId });
        return result.ToList();
    }

    public async Task<List<IngredientDtoOld>> GetIngredientsByType(int ingredientTypeId)
    {
        const string sql = @"
            SELECT 
                i.id, 
                i.ingredient_name AS IngredientName, 
                i.image_url AS ImageUrl, 
                i.ingredient_price AS IngredientPrice
            FROM cafeteria.ingredient i
            JOIN cafeteria.ingredient_ingredient_type iit ON i.id = iit.ingredient_id
            WHERE iit.ingredient_type_id = @ingredient_type_id";

        var result = await _dbConnection.QueryAsync<IngredientDtoOld>(sql, new { ingredient_type_id = ingredientTypeId });
        return result.ToList();
    }

    public async Task<IngredientDtoOld> GetIngredientById(int ingredientId)
    {
        const string sql = @"
            SELECT 
                id, 
                ingredient_name AS IngredientName, 
                image_url AS ImageUrl, 
                ingredient_price AS IngredientPrice
            FROM cafeteria.ingredient
            WHERE id = @ingredient_id";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<IngredientDtoOld>(sql, new { ingredient_id = ingredientId });
        return result ?? throw new InvalidOperationException($"Ingredient with ID {ingredientId} not found.");
    }
}