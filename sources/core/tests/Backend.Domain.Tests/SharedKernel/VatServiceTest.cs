using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Domain.Spi;
using Moq;

namespace Backend.Domain.Tests.SharedKernel;

public class VatServiceTests
{
    [Fact]
    public async Task GetPriceWithVatAsync_ShouldReturnPriceWithVat()
    {
        // Arrange
        var sut = MakeSut(out var parameterRepositoryMock);
        var price = 100.0;
        var vatRate = 20.0;
        parameterRepositoryMock.Setup(x => x.GetVatRateAsync()).ReturnsAsync(vatRate);

        // Act
        var result = await sut.GetPriceWithVatAsync(price);

        // Assert
        result.Should().Be(120.0);
    }

    [Fact]
    public async Task GetPriceWithVatAsync_ShouldCallGetVatRateAsync()
    {
        // Arrange
        var sut = MakeSut(out var parameterRepositoryMock);
        var price = 100.0;
        var vatRate = 10.0;
        parameterRepositoryMock.Setup(x => x.GetVatRateAsync()).ReturnsAsync(vatRate);

        // Act
        await sut.GetPriceWithVatAsync(price);

        // Assert
        parameterRepositoryMock.Verify(x => x.GetVatRateAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPriceWithVatAsync_WithZeroPrice_ShouldReturnZero()
    {
        // Arrange
        var sut = MakeSut(out var parameterRepositoryMock);
        var price = 0.0;
        var vatRate = 20.0;
        parameterRepositoryMock.Setup(x => x.GetVatRateAsync()).ReturnsAsync(vatRate);

        // Act
        var result = await sut.GetPriceWithVatAsync(price);

        // Assert
        result.Should().Be(0.0);
    }

    [Fact]
    public async Task GetPriceWithVatAsync_WithZeroVatRate_ShouldReturnOriginalPrice()
    {
        // Arrange
        var sut = MakeSut(out var parameterRepositoryMock);
        var price = 100.0;
        var vatRate = 0.0;
        parameterRepositoryMock.Setup(x => x.GetVatRateAsync()).ReturnsAsync(vatRate);

        // Act
        var result = await sut.GetPriceWithVatAsync(price);

        // Assert
        result.Should().Be(100.0);
    }

    private static VatService MakeSut(out Mock<IParameterRepository> parameterRepositoryMock)
    {
        parameterRepositoryMock = new Mock<IParameterRepository>();
        return new VatService(parameterRepositoryMock.Object);
    }
}