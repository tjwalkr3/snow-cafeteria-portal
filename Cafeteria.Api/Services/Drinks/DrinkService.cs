using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public class DrinkService : IDrinkService
{
    private readonly IDbConnection _dbConnection;

    public DrinkService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<DrinkDto> CreateDrink(DrinkDto drinkDto)
    {
        const string sql = @"
            INSERT INTO cafeteria.drink (station_id, drink_name, drink_description, drink_price, image_url, in_stock)
            VALUES (@StationId, @DrinkName, @DrinkDescription, @DrinkPrice, @ImageUrl, @InStock)
            RETURNING id AS Id, station_id AS StationId, drink_name AS DrinkName, drink_description AS DrinkDescription, drink_price AS DrinkPrice, image_url AS ImageUrl, in_stock AS InStock;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<DrinkDto>(sql, drinkDto);
        return result ?? throw new InvalidOperationException("Failed to create drink");
    }

    public async Task<DrinkDto?> GetDrinkByID(int id)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                station_id AS StationId, 
                drink_name AS DrinkName, 
                drink_description AS DrinkDescription, 
                drink_price AS DrinkPrice, 
                image_url AS ImageUrl,
                in_stock AS InStock
            FROM cafeteria.drink
            WHERE id = @id;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<DrinkDto>(sql, new { id });
        return result;
    }

    public async Task<List<DrinkDto>> GetAllDrinks()
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                station_id AS StationId, 
                drink_name AS DrinkName, 
                drink_description AS DrinkDescription, 
                drink_price AS DrinkPrice, 
                image_url AS ImageUrl,
                in_stock AS InStock
            FROM cafeteria.drink
            ORDER BY drink_name;";

        var result = await _dbConnection.QueryAsync<DrinkDto>(sql);
        return result.ToList();
    }

    public async Task<List<DrinkDto>> GetDrinksByStationID(int stationId)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                station_id AS StationId, 
                drink_name AS DrinkName, 
                drink_description AS DrinkDescription, 
                drink_price AS DrinkPrice, 
                image_url AS ImageUrl,
                in_stock AS InStock
            FROM cafeteria.drink
            WHERE station_id = @stationId
            ORDER BY drink_name;";

        var result = await _dbConnection.QueryAsync<DrinkDto>(sql, new { stationId });
        return result.ToList();
    }

    public async Task<DrinkDto?> UpdateDrinkByID(int id, DrinkDto drinkDto)
    {
        const string sql = @"
            UPDATE cafeteria.drink
            SET station_id = @StationId,
                drink_name = @DrinkName,
                drink_description = @DrinkDescription,
                drink_price = @DrinkPrice,
                image_url = @ImageUrl,
                in_stock = @InStock
            WHERE id = @id
            RETURNING id AS Id, station_id AS StationId, drink_name AS DrinkName, drink_description AS DrinkDescription, drink_price AS DrinkPrice, image_url AS ImageUrl, in_stock AS InStock;";

        var parameters = new
        {
            id,
            drinkDto.StationId,
            drinkDto.DrinkName,
            drinkDto.DrinkDescription,
            drinkDto.DrinkPrice,
            drinkDto.ImageUrl,
            drinkDto.InStock
        };

        var result = await _dbConnection.QuerySingleOrDefaultAsync<DrinkDto>(sql, parameters);
        return result;
    }

    public async Task<bool> DeleteDrinkByID(int id)
    {
        const string sql = @"
            DELETE FROM cafeteria.drink
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }

    public async Task<bool> SetInStockById(int id, bool inStock)
    {
        const string sql = @"
            UPDATE cafeteria.drink
            SET in_stock = @inStock
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id, inStock });
        return rowsAffected > 0;
    }
}
