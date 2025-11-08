using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs;

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
        SELECT 
            id AS Id, 
            station_id AS StationId, 
            entree_name AS EntreeName, 
            entree_description AS EntreeDescription, 
            entree_price AS EntreePrice, 
            image_url AS ImageUrl
        FROM cafeteria.entree
        WHERE station_id = @stationId;";
        var result = await _dbConnection.QueryAsync<EntreeDto>(sql, new { stationId });
        return result.ToList();
    }

    public async Task<List<SideDto>> GetSidesByStation(int stationId)
    {
        const string sql = @"
        SELECT 
            id AS Id, 
            station_id AS StationId, 
            side_name AS SideName, 
            side_description AS SideDescription, 
            side_price AS SidePrice, 
            image_url AS ImageUrl
        FROM cafeteria.side
        WHERE station_id = @stationId;";
        var result = await _dbConnection.QueryAsync<SideDto>(sql, new { stationId });
        return result.ToList();
    }

    public async Task<List<DrinkDto>> GetDrinksByLocation(int locationId)
    {
        const string sql = @"
            SELECT
                d.id AS Id, 
                d.station_id AS StationId, 
                d.drink_name AS DrinkName, 
                d.drink_description AS DrinkDescription, 
                d.drink_price AS DrinkPrice, 
                d.image_url AS ImageUrl
            FROM cafeteria.drink d
            INNER JOIN cafeteria.station s ON d.station_id = s.id
            WHERE s.location_id = @locationId;";
        var result = await _dbConnection.QueryAsync<DrinkDto>(sql, new { locationId });
        return result.ToList();
    }

    public async Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId)
    {
        string sql = @"
            SELECT 
                fo.id AS Id,
                fo.food_option_name AS FoodOptionName, 
                fo.in_stock AS InStock, 
                fo.image_url AS ImageUrl
            FROM cafeteria.food_option fo
            INNER JOIN cafeteria.option_option_type oot ON fo.id = oot.food_option_id
            INNER JOIN cafeteria.food_option_type fot ON oot.food_option_type_id = fot.id
            WHERE fot.entree_id = @entreeId;";
        var result = await _dbConnection.QueryAsync<FoodOptionDto>(sql, new { entreeId });
        return result.ToList();
    }

    public async Task<List<FoodOptionDto>> GetOptionsBySide(int sideId)
    {
        string sql = @"
        SELECT 
            fo.id AS Id, 
            fo.food_option_name AS FoodOptionName, 
            fo.in_stock AS InStock, 
            fo.image_url AS ImageUrl
        FROM cafeteria.food_option fo
        INNER JOIN cafeteria.option_option_type oot ON fo.id = oot.food_option_id
        INNER JOIN cafeteria.food_option_type fot ON oot.food_option_type_id = fot.id
        WHERE fot.side_id = @sideId;";
        var result = await _dbConnection.QueryAsync<FoodOptionDto>(sql, new { sideId });
        return result.ToList();
    }
}