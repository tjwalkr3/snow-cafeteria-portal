using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;
using Moq;
using Xunit;
using System.Net;
using System.Net.Http.Json;

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
        // Arrange
        var expectedSides = new List<SideDto>
        {
            new SideDto { Id = 1, SideName = "Fries", SidePrice = 2.50m },
            new SideDto { Id = 2, SideName = "Salad", SidePrice = 3.00m }
        };

        _mockHttpClient.Setup(x => x.GetAsync<List<SideDto>>("api/Side"))
            .ReturnsAsync(expectedSides);

        // Act
        var result = await _sideService.GetAllSides();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Fries", result[0].SideName);
        _mockHttpClient.Verify(x => x.GetAsync<List<SideDto>>("api/Side"), Times.Once);
    }

    [Fact]
    public async Task CreateSide_ReturnsSideDto_WhenSuccessful()
    {
        // Arrange
        var createDto = new SideDto { SideName = "Chips", SidePrice = 1.50m, StationId = 1 };
        var expectedSide = new SideDto { Id = 1, SideName = "Chips", SidePrice = 1.50m, StationId = 1 };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = JsonContent.Create(expectedSide)
        };

        _mockHttpClient.Setup(x => x.PostAsync<SideDto>("api/Side", It.IsAny<SideDto>()))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _sideService.CreateSide(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSide.Id, result.Id);
        Assert.Equal(expectedSide.SideName, result.SideName);
        _mockHttpClient.Verify(x => x.PostAsync<SideDto>("api/Side", It.IsAny<SideDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSide_ReturnsSideDto_WhenSuccessful()
    {
        // Arrange
        var updateDto = new SideDto { Id = 1, SideName = "Chips Updated", SidePrice = 1.75m, StationId = 1 };
        var expectedSide = new SideDto { Id = 1, SideName = "Chips Updated", SidePrice = 1.75m, StationId = 1 };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedSide)
        };

        _mockHttpClient.Setup(x => x.PutAsync<SideDto>($"api/Side/{updateDto.Id}", It.IsAny<SideDto>()))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _sideService.UpdateSide(updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSide.SideName, result.SideName);
        _mockHttpClient.Verify(x => x.PutAsync<SideDto>($"api/Side/{updateDto.Id}", It.IsAny<SideDto>()), Times.Once);
    }

    [Fact]
    public async Task CreateSide_ThrowsException_WhenFailure()
    {
        // Arrange
        var createDto = new SideDto { SideName = "Chips", SidePrice = 1.50m, StationId = 1 };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error message")
        };

        _mockHttpClient.Setup(x => x.PostAsync<SideDto>("api/Side", It.IsAny<SideDto>()))
            .ReturnsAsync(responseMessage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _sideService.CreateSide(createDto));
        Assert.Equal("Error message", exception.Message);
    }

    [Fact]
    public async Task DeleteSide_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        int sideId = 1;
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        _mockHttpClient.Setup(x => x.DeleteAsync<object>($"api/Side/{sideId}"))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _sideService.DeleteSide(sideId);

        // Assert
        Assert.True(result);
        _mockHttpClient.Verify(x => x.DeleteAsync<object>($"api/Side/{sideId}"), Times.Once);
    }

    [Fact]
    public async Task DeleteSide_ReturnsFalse_WhenFailed()
    {
        // Arrange
        int sideId = 1;
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

        _mockHttpClient.Setup(x => x.DeleteAsync<object>($"api/Side/{sideId}"))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _sideService.DeleteSide(sideId);

        // Assert
        Assert.False(result);
        _mockHttpClient.Verify(x => x.DeleteAsync<object>($"api/Side/{sideId}"), Times.Once);
    }
}
