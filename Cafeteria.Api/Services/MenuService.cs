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

    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        const string sql = @"
            SELECT 
                id,
                location_id AS LocationId,
                station_name AS StationName,
                station_description AS StationDescription
            FROM cafeteria.station
            WHERE location_id = @location_id";

        var result = await _dbConnection.QueryAsync<StationDto>(sql, new { location_id = locationId });
        return result.ToList();
    }

    public async Task<List<EntreeDto>> GetEntreesByStation(int stationId)
    {
        const string sql = @"
        SELECT id, station_id, entree_name, entree_description, entree_price, image_url
            FROM cafeteria.entree
            WHERE station_id = @stationId;";
        var result = await _dbConnection.QueryAsync<EntreeDto>(sql, new { stationId });
        return result.ToList();
    }

    public async Task<List<SideDto>> GetSidesByStation(int stationId)
    {
        const string sql = @"
        SELECT id, station_id, side_name, side_description, side_price, image_url
            FROM cafeteria.side
            WHERE station_id = @stationId;";
        var result = await _dbConnection.QueryAsync<SideDto>(sql, new { stationId });
        return result.ToList();
    }

    public async Task<List<DrinkDto>> GetDrinksByLocation(int locationId)
    {
        const string sql = @"
            SELECT d.id, d.station_id, d.drink_name, d.drink_description, d.drink_price, d.image_url
            FROM cafeteria.drink d
            INNER JOIN cafeteria.station s ON d.station_id = s.id
            WHERE s.location_id = @locationId;";
        var result = await _dbConnection.QueryAsync<DrinkDto>(sql, new { locationId });
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

    public async Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId)
    {
        string sql = @"
            SELECT fo.id, fo.food_option_name, fo.in_stock, fo.image_url
            FROM cafeteria.food_option fo
            INNER JOIN cafeteria.option_option_type oot ON fo.id = oot.food_option_id
            INNER JOIN cafeteria.food_option_type fot ON oot.food_option_type_id = fot.id
            WHERE fot.entree_id = @entreeId;";
        var result = await _dbConnection.QueryAsync<FoodOptionDto>(sql, new { entree_id = entreeId });
        return result.ToList();
    }

    public async Task<List<FoodOptionDto>> GetOptionsBySide(int sideId)
    {
        string sql = @"
        SELECT fo.id, fo.food_option_name, fo.in_stock, fo.image_url
            FROM cafeteria.food_option fo
            INNER JOIN cafeteria.option_option_type oot ON fo.id = oot.food_option_id
            INNER JOIN cafeteria.food_option_type fot ON oot.food_option_type_id = fot.id
            WHERE fot.side_id = @sideId;";
        var result = await _dbConnection.QueryAsync<FoodOptionDto>(sql, new { side_id = sideId });
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