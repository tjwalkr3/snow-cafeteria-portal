using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;
using Moq;
using Xunit;
using System.Net;

namespace Cafeteria.UnitTests.Management.Services;

public class SideServiceTests
{
    private readonly Mock<IHttpClientAuth> _mockHttpClient;
    private readonly SideService _sideService;

    public SideServiceTests()
    {
        _mockHttpClient = new Mock<IHttpClientAuth>();
        _sideService = new SideService(_mockHttpClient.Object);
    }

    [Fact]
    public async Task GetAllSides_ReturnsListOfSides()
    {
        var expectedSides = new List<SideDto>
        {
            new SideDto { Id = 1, SideName = "Fries", SidePrice = 2.50m },
            new SideDto { Id = 2, SideName = "Salad", SidePrice = 3.00m }
        };

        _mockHttpClient.Setup(x => x.GetAsync<List<SideDto>>("api/side"))
            .ReturnsAsync(expectedSides);

        var result = await _sideService.GetAllSides();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Fries", result[0].SideName);
        _mockHttpClient.Verify(x => x.GetAsync<List<SideDto>>("api/side"), Times.Once);
    }

    [Fact]
    public async Task DeleteSide_ReturnsTrue_WhenSuccessful()
    {
        int sideId = 1;
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        _mockHttpClient.Setup(x => x.DeleteAsync<object>($"api/side/{sideId}"))
            .ReturnsAsync(responseMessage);

        var result = await _sideService.DeleteSide(sideId);

        Assert.True(result);
        _mockHttpClient.Verify(x => x.DeleteAsync<object>($"api/side/{sideId}"), Times.Once);
    }

    [Fact]
    public async Task DeleteSide_ReturnsFalse_WhenFailed()
    {
        int sideId = 1;
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

        _mockHttpClient.Setup(x => x.DeleteAsync<object>($"api/side/{sideId}"))
            .ReturnsAsync(responseMessage);

        var result = await _sideService.DeleteSide(sideId);

        Assert.False(result);
        _mockHttpClient.Verify(x => x.DeleteAsync<object>($"api/side/{sideId}"), Times.Once);
    }
}
