using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public class SideService : ISideService
{
    private readonly IDbConnection _dbConnection;

    public SideService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<SideDto> CreateSide(SideDto sideDto)
    {
        const string sql = @"
            INSERT INTO cafeteria.side (station_id, side_name, side_description, side_price, image_url, in_stock)
            VALUES (@StationId, @SideName, @SideDescription, @SidePrice, @ImageUrl, @InStock)
            RETURNING id AS Id, station_id AS StationId, side_name AS SideName, side_description AS SideDescription, side_price AS SidePrice, image_url AS ImageUrl, in_stock AS InStock;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<SideDto>(sql, sideDto);
        return result ?? throw new InvalidOperationException("Failed to create side");
    }

    public async Task<SideDto?> GetSideByID(int id)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                station_id AS StationId, 
                side_name AS SideName, 
                side_description AS SideDescription, 
                side_price AS SidePrice, 
                image_url AS ImageUrl,
                in_stock AS InStock
            FROM cafeteria.side
            WHERE id = @id;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<SideDto>(sql, new { id });
        return result;
    }

    public async Task<List<SideDto>> GetAllSides()
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                station_id AS StationId, 
                side_name AS SideName, 
                side_description AS SideDescription, 
                side_price AS SidePrice, 
                image_url AS ImageUrl,
                in_stock AS InStock
            FROM cafeteria.side
            ORDER BY side_name, id;";

        var result = await _dbConnection.QueryAsync<SideDto>(sql);
        return result.ToList();
    }

    public async Task<List<SideDto>> GetSidesByStationID(int stationId)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                station_id AS StationId, 
                side_name AS SideName, 
                side_description AS SideDescription, 
                side_price AS SidePrice, 
                image_url AS ImageUrl,
                in_stock AS InStock
            FROM cafeteria.side
            WHERE station_id = @stationId
            ORDER BY side_name, id;";

        var result = await _dbConnection.QueryAsync<SideDto>(sql, new { stationId });
        return result.ToList();
    }

    public async Task<SideDto?> UpdateSideByID(int id, SideDto sideDto)
    {
        const string sql = @"
            UPDATE cafeteria.side
            SET station_id = @StationId,
                side_name = @SideName,
                side_description = @SideDescription,
                side_price = @SidePrice,
                image_url = @ImageUrl,
                in_stock = @InStock
            WHERE id = @id
            RETURNING id AS Id, station_id AS StationId, side_name AS SideName, side_description AS SideDescription, side_price AS SidePrice, image_url AS ImageUrl, in_stock AS InStock;";

        var parameters = new
        {
            id,
            sideDto.StationId,
            sideDto.SideName,
            sideDto.SideDescription,
            sideDto.SidePrice,
            sideDto.ImageUrl,
            sideDto.InStock
        };

        var result = await _dbConnection.QuerySingleOrDefaultAsync<SideDto>(sql, parameters);
        return result;
    }

    public async Task<bool> DeleteSideByID(int id)
    {
        const string sql = @"
            DELETE FROM cafeteria.side
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }

    public async Task<bool> SetInStockById(int id, bool inStock)
    {
        const string sql = @"
            UPDATE cafeteria.side
            SET in_stock = @inStock
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id, inStock });
        return rowsAffected > 0;
    }
}
