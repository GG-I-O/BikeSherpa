using Ardalis.Specification;
using AutoFixture;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Customers.Add;

public class AddCustomerHandlerTests
{
    private readonly Mock<ICustomerFactory> _mockFactory = new();
    private readonly Mock<IReadRepository<Customer>> _mockRepository = new();
    private readonly Mock<IApplicationTransaction> _mockTransaction = new();
    private readonly Fixture _fixture = new();

    private readonly AddressCrud _mockAddress;
    private readonly AddCustomerCommand _mockCommand;
    private readonly Customer _mockCustomer;

    public AddCustomerHandlerTests()
    {
        _mockAddress = _fixture.Create<AddressCrud>();
        _mockCommand = _fixture.Create<AddCustomerCommand>();
        _mockCustomer = _fixture.Create<Customer>();

        _mockFactory
             .Setup(x => x.CreateCustomerAsync(
                  It.IsAny<string>(),
                  It.IsAny<string>(),
                  It.IsAny<string?>(),
                  It.IsAny<string>(),
                  It.IsAny<string>(),
                  It.IsAny<Address>()))
             .ReturnsAsync(_mockCustomer);
    }

    private AddCustomerHandler CreateSut()
    {
        var validator = new AddCustomerCommandValidator(_mockRepository.Object);
        return new AddCustomerHandler(_mockFactory.Object, validator, _mockTransaction.Object);
    }

    private void SetupRepositoryTestingIfCodeExist(string code, bool doesCodeExist)
    {
        _mockRepository
             .Setup(x => x.AnyAsync(
                  It.IsAny<ISpecification<Customer>>(),
                  It.IsAny<CancellationToken>()))
             .ReturnsAsync(doesCodeExist);
    }

    private void VerifyFactoryCalledOnce()
    {
        _mockFactory.Verify(
             x => x.CreateCustomerAsync(
                  It.IsAny<string>(),
                  It.IsAny<string>(),
                  It.IsAny<string?>(),
                  It.IsAny<string>(),
                  It.IsAny<string>(),
                  It.IsAny<Address>()),
             Times.Once);
    }

    private void VerifyTransactionCommittedOnce()
    {
        _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateCustomerAndReturnId_WhenCommandIsValid()
    {
        // Arrange
        SetupRepositoryTestingIfCodeExist(_mockCommand.Code, false); // Tell the validator that the code does not exist
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(_mockCommand, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_mockCustomer.Id, result.Value);
        VerifyFactoryCalledOnce();
        VerifyTransactionCommittedOnce();
    }

    [Fact]
    public async Task Handle_ShouldCreateCustomerWithCorrectInfos()
    {
        // Arrange
        SetupRepositoryTestingIfCodeExist(_mockCommand.Code, false);
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(_mockCommand, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_mockCustomer.Id, result.Value);
        _mockFactory.Verify(
             x => x.CreateCustomerAsync(
                  _mockCommand.Name,
                  _mockCommand.Code,
                  _mockCommand.Siret,
                  _mockCommand.Email,
                  _mockCommand.PhoneNumber,
                  It.IsAny<Address>()),
             Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenNameIsEmpty()
    {
        // Arrange
        var commandWithEmptyName = _mockCommand with { Name = "" };
        SetupRepositoryTestingIfCodeExist(commandWithEmptyName.Code, false);
        var sut = CreateSut();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
             sut.Handle(commandWithEmptyName, CancellationToken.None).AsTask());

        _mockFactory.Verify(
             x => x.CreateCustomerAsync(
                  It.IsAny<string>(),
                  It.IsAny<string>(),
                  It.IsAny<string?>(),
                  It.IsAny<string>(),
                  It.IsAny<string>(),
                  It.IsAny<Address>()),
             Times.Never);

        _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenCodeIsEmpty()
        {
            // Arrange
            var commandWithEmptyCode = _mockCommand with { Code = "" };
            SetupRepositoryTestingIfCodeExist(commandWithEmptyCode.Code, false);
            var sut = CreateSut();

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                 sut.Handle(commandWithEmptyCode, CancellationToken.None).AsTask());

            _mockFactory.Verify(
                 x => x.CreateCustomerAsync(
                      It.IsAny<string>(),
                      It.IsAny<string>(),
                      It.IsAny<string?>(),
                      It.IsAny<string>(),
                      It.IsAny<string>(),
                      It.IsAny<Address>()),
                 Times.Never);

            _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenEmailIsEmpty()
        {
            // Arrange
            var commandWithEmptyEmail = _mockCommand with { Email =  "" };
            SetupRepositoryTestingIfCodeExist(commandWithEmptyEmail.Code, false);
            var sut = CreateSut();

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                 sut.Handle(commandWithEmptyEmail, CancellationToken.None).AsTask());

            _mockFactory.Verify(
                 x => x.CreateCustomerAsync(
                      It.IsAny<string>(),
                      It.IsAny<string>(),
                      It.IsAny<string?>(),
                      It.IsAny<string>(),
                      It.IsAny<string>(),
                      It.IsAny<Address>()),
                 Times.Never);

            _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenPhoneNumberIsEmpty()
        {
            // Arrange
            var commandWithEmptyPhoneNumber = _mockCommand with { PhoneNumber =   "" };
            SetupRepositoryTestingIfCodeExist(commandWithEmptyPhoneNumber.Code, false);
            var sut = CreateSut();

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                 sut.Handle(commandWithEmptyPhoneNumber, CancellationToken.None).AsTask());

            _mockFactory.Verify(
                 x => x.CreateCustomerAsync(
                      It.IsAny<string>(),
                      It.IsAny<string>(),
                      It.IsAny<string?>(),
                      It.IsAny<string>(),
                      It.IsAny<string>(),
                      It.IsAny<Address>()),
                 Times.Never);

            _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenCodeAlreadyExists()
        {
            // Arrange
            SetupRepositoryTestingIfCodeExist(_mockCommand.Code, true);
            var sut = CreateSut();

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                 sut.Handle(_mockCommand, CancellationToken.None).AsTask());

            _mockFactory.Verify(
                 x => x.CreateCustomerAsync(
                      It.IsAny<string>(),
                      It.IsAny<string>(),
                      It.IsAny<string?>(),
                      It.IsAny<string>(),
                      It.IsAny<string>(),
                      It.IsAny<Address>()),
                 Times.Never);

            _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Fact]
        public async Task Handle_ShouldRespectCancellationToken()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            await cancellationTokenSource.CancelAsync();
            SetupRepositoryTestingIfCodeExist(_mockCommand.Code, false);
            var sut = CreateSut();

            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(() =>
                 sut.Handle(_mockCommand, cancellationTokenSource.Token).AsTask());
        }
}
