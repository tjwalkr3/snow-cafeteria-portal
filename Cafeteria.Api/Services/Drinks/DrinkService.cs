using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.Drinks;

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
            INSERT INTO cafeteria.drink (location_id, drink_name, drink_description, drink_price, in_stock, card_only, swipe_only)
            VALUES (@LocationId, @DrinkName, @DrinkDescription, @DrinkPrice, @InStock, @CardOnly, @SwipeOnly)
            RETURNING id AS Id, location_id AS LocationId, drink_name AS DrinkName, drink_description AS DrinkDescription, drink_price AS DrinkPrice, in_stock AS InStock, card_only AS CardOnly, swipe_only AS SwipeOnly;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<DrinkDto>(sql, drinkDto);
        return result ?? throw new InvalidOperationException("Failed to create drink");
    }

    public async Task<DrinkDto?> GetDrinkById(int id)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                location_id AS LocationId, 
                drink_name AS DrinkName, 
                drink_description AS DrinkDescription, 
                drink_price AS DrinkPrice,
                in_stock AS InStock,
                card_only AS CardOnly,
                swipe_only AS SwipeOnly
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
                location_id AS LocationId, 
                drink_name AS DrinkName, 
                drink_description AS DrinkDescription, 
                drink_price AS DrinkPrice, 
                in_stock AS InStock,
                card_only AS CardOnly,
                swipe_only AS SwipeOnly
            FROM cafeteria.drink
            ORDER BY drink_name, id;";

        var result = await _dbConnection.QueryAsync<DrinkDto>(sql);
        return result.ToList();
    }

    public async Task<List<DrinkDto>> GetDrinksByLocationId(int locationId)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                location_id AS LocationId, 
                drink_name AS DrinkName, 
                drink_description AS DrinkDescription, 
                drink_price AS DrinkPrice, 
                in_stock AS InStock,
                card_only AS CardOnly,
                swipe_only AS SwipeOnly
            FROM cafeteria.drink
            WHERE location_id = @locationId
            ORDER BY drink_name, id;";

        var result = await _dbConnection.QueryAsync<DrinkDto>(sql, new { locationId });
        return result.ToList();
    }

    public async Task<List<DrinkDto>> GetSwipeDrinksByLocationId(int locationId)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                location_id AS LocationId, 
                drink_name AS DrinkName, 
                drink_description AS DrinkDescription, 
                drink_price AS DrinkPrice, 
                in_stock AS InStock,
                card_only AS CardOnly,
                swipe_only AS SwipeOnly
            FROM cafeteria.drink
            WHERE location_id = @locationId AND (swipe_only = true OR (card_only = false AND swipe_only = false))
            ORDER BY drink_name, id;";

        var result = await _dbConnection.QueryAsync<DrinkDto>(sql, new { locationId });
        return result.ToList();
    }

    public async Task<List<DrinkDto>> GetCardDrinksByLocationId(int locationId)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                location_id AS LocationId, 
                drink_name AS DrinkName, 
                drink_description AS DrinkDescription, 
                drink_price AS DrinkPrice, 
                in_stock AS InStock,
                card_only AS CardOnly,
                swipe_only AS SwipeOnly
            FROM cafeteria.drink
            WHERE location_id = @locationId AND (card_only = true OR (card_only = false AND swipe_only = false))
            ORDER BY drink_name, id;";

        var result = await _dbConnection.QueryAsync<DrinkDto>(sql, new { locationId });
        return result.ToList();
    }

    public async Task<DrinkDto?> UpdateDrinkById(int id, DrinkDto drinkDto)
    {
        const string sql = @"
            UPDATE cafeteria.drink
            SET location_id = @LocationId,
                drink_name = @DrinkName,
                drink_description = @DrinkDescription,
                drink_price = @DrinkPrice,
                in_stock = @InStock,
                card_only = @CardOnly,
                swipe_only = @SwipeOnly
            WHERE id = @id
            RETURNING id AS Id, location_id AS LocationId, drink_name AS DrinkName, drink_description AS DrinkDescription, drink_price AS DrinkPrice, in_stock AS InStock, card_only AS CardOnly, swipe_only AS SwipeOnly;";

        var parameters = new
        {
            id,
            drinkDto.LocationId,
            drinkDto.DrinkName,
            drinkDto.DrinkDescription,
            drinkDto.DrinkPrice,
            drinkDto.InStock,
            drinkDto.CardOnly,
            drinkDto.SwipeOnly
        };

        var result = await _dbConnection.QuerySingleOrDefaultAsync<DrinkDto>(sql, parameters);
        return result;
    }

    public async Task<bool> DeleteDrinkById(int id)
    {
        const string sql = @"
            DELETE FROM cafeteria.drink
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }

    public async Task<bool> SetStockStatusById(int id, bool inStock)
    {
        const string sql = @"
            UPDATE cafeteria.drink
            SET in_stock = @inStock
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id, inStock });
        return rowsAffected > 0;
    }
}
