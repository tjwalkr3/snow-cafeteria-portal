using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public class EntreeService : IEntreeService
{
    private readonly IDbConnection _dbConnection;

    public EntreeService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<EntreeDto> CreateEntree(EntreeDto entreeDto)
    {
        const string sql = @"
            INSERT INTO cafeteria.entree (station_id, entree_name, entree_description, entree_price, image_url, in_stock)
            VALUES (@StationId, @EntreeName, @EntreeDescription, @EntreePrice, @ImageUrl, @InStock)
            RETURNING id AS Id, station_id AS StationId, entree_name AS EntreeName, entree_description AS EntreeDescription, entree_price AS EntreePrice, image_url AS ImageUrl, in_stock AS InStock;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<EntreeDto>(sql, entreeDto);
        return result ?? throw new InvalidOperationException("Failed to create entree");
    }

    public async Task<EntreeDto?> GetEntreeByID(int id)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                station_id AS StationId, 
                entree_name AS EntreeName, 
                entree_description AS EntreeDescription, 
                entree_price AS EntreePrice, 
                image_url AS ImageUrl,
                in_stock AS InStock
            FROM cafeteria.entree
            WHERE id = @id;";

        var result = await _dbConnection.QuerySingleOrDefaultAsync<EntreeDto>(sql, new { id });
        return result;
    }

    public async Task<List<EntreeDto>> GetAllEntrees()
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                station_id AS StationId, 
                entree_name AS EntreeName, 
                entree_description AS EntreeDescription, 
                entree_price AS EntreePrice, 
                image_url AS ImageUrl,
                in_stock AS InStock
            FROM cafeteria.entree
            ORDER BY entree_name, id;";

        var result = await _dbConnection.QueryAsync<EntreeDto>(sql);
        return result.ToList();
    }

    public async Task<List<EntreeDto>> GetEntreesByStationID(int stationId)
    {
        const string sql = @"
            SELECT 
                id AS Id, 
                station_id AS StationId, 
                entree_name AS EntreeName, 
                entree_description AS EntreeDescription, 
                entree_price AS EntreePrice, 
                image_url AS ImageUrl,
                in_stock AS InStock
            FROM cafeteria.entree
            WHERE station_id = @stationId
            ORDER BY entree_name, id;";

        var result = await _dbConnection.QueryAsync<EntreeDto>(sql, new { stationId });
        return result.ToList();
    }

    public async Task<EntreeDto?> UpdateEntreeByID(int id, EntreeDto entreeDto)
    {
        const string sql = @"
            UPDATE cafeteria.entree
            SET station_id = @StationId,
                entree_name = @EntreeName,
                entree_description = @EntreeDescription,
                entree_price = @EntreePrice,
                image_url = @ImageUrl,
                in_stock = @InStock
            WHERE id = @id
            RETURNING id AS Id, station_id AS StationId, entree_name AS EntreeName, entree_description AS EntreeDescription, entree_price AS EntreePrice, image_url AS ImageUrl, in_stock AS InStock;";

        var parameters = new
        {
            id,
            entreeDto.StationId,
            entreeDto.EntreeName,
            entreeDto.EntreeDescription,
            entreeDto.EntreePrice,
            entreeDto.ImageUrl,
            entreeDto.InStock
        };

        var result = await _dbConnection.QuerySingleOrDefaultAsync<EntreeDto>(sql, parameters);
        return result;
    }

    public async Task<bool> DeleteEntreeByID(int id)
    {
        const string sql = @"
            DELETE FROM cafeteria.entree
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }

    public async Task<bool> SetInStockById(int id, bool inStock)
    {
        const string sql = @"
            UPDATE cafeteria.entree
            SET in_stock = @inStock
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id, inStock });
        return rowsAffected > 0;
    }
}
