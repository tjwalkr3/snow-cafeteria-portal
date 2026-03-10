using System.Data;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;

namespace Cafeteria.Api.Services.Icons;

public class IconService : IIconService
{
    private readonly IDbConnection _dbConnection;

    public IconService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<IconDto>> GetAllIcons()
    {
        const string sql = @"
            SELECT
                id AS Id,
                icon_name AS IconName,
                bootstrap_name AS BootstrapName
            FROM cafeteria.icon
            ORDER BY id;";

        var icons = await _dbConnection.QueryAsync<IconDto>(sql);
        return icons.ToList();
    }
}
