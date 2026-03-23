using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AwesomeAssertions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Customers.Update;

public class UpdateCustomerHandlerTests
{
     private readonly Mock<IReadRepository<Customer>> _mockRepository = new();
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Fixture _fixture = new();

     private readonly UpdateCustomerCommand _mockCommand;
     private readonly Customer _mockCustomer;

     public UpdateCustomerHandlerTests()
     {
          _mockCommand = _fixture.Create<UpdateCustomerCommand>();
          _mockCustomer = _fixture.Build<Customer>()
               .With(c => c.Id, _mockCommand.Id)
               .Create();

          _mockRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Customer>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_mockCustomer);
     }

     private UpdateCustomerHandler CreateSut()
     {
          var validator = new UpdateCustomerCommandValidator(_mockRepository.Object);
          return new UpdateCustomerHandler(_mockRepository.Object, validator, _mockTransaction.Object);
     }

     private void SetupRepositoryTestingIfCodeExists(bool doesCodeExist)
     {
          if (doesCodeExist)
          {
               var conflictingCustomer = _fixture.Build<Customer>()
                    .With(c => c.Id, Guid.NewGuid()) // Different ID than _mockCommand.Id
                    .With(c => c.Code, _mockCommand.Code)
                    .Create();
          
               _mockRepository
                    .Setup(x => x.FirstOrDefaultAsync(
                         It.Is<ISpecification<Customer>>(spec => spec.GetType().Name == "CustomerByCodeSpecification"),
                         It.IsAny<CancellationToken>()))
                    .ReturnsAsync(conflictingCustomer);
          }
          else
          {
               _mockRepository
                    .Setup(x => x.FirstOrDefaultAsync(
                         It.Is<ISpecification<Customer>>(spec => spec.GetType().Name == "CustomerByCodeSpecification"),
                         It.IsAny<CancellationToken>()))
                    .ReturnsAsync(null as Customer);
          }
     }
     
     private void SetupRepositoryTestingIfSiretExists(bool doesSiretExist)
     {
          if (doesSiretExist)
          {
               var conflictingCustomer = _fixture.Build<Customer>()
                    .With(c => c.Id, Guid.NewGuid()) // Different ID than _mockCommand.Id
                    .With(c => c.Siret, _mockCommand.Siret)
                    .Create();
          
               _mockRepository
                    .Setup(x => x.FirstOrDefaultAsync(
                         It.Is<ISpecification<Customer>>(spec => spec.GetType().Name == "CustomerBySiretSpecification"),
                         It.IsAny<CancellationToken>()))
                    .ReturnsAsync(conflictingCustomer);
          }
          else
          {
               _mockRepository
                    .Setup(x => x.FirstOrDefaultAsync(
                         It.Is<ISpecification<Customer>>(spec => spec.GetType().Name == "CustomerBySiretSpecification"),
                         It.IsAny<CancellationToken>()))
                    .ReturnsAsync(null as Customer);
          }
     }
     
     private void SetupRepositoryTestingIfVatNumberExists(bool doesVatNumberExist)
     {
          if (doesVatNumberExist)
          {
               var conflictingCustomer = _fixture.Build<Customer>()
                    .With(c => c.Id, Guid.NewGuid()) // Different ID than _mockCommand.Id
                    .With(c => c.VatNumber, _mockCommand.VatNumber)
                    .Create();
          
               _mockRepository
                    .Setup(x => x.FirstOrDefaultAsync(
                         It.Is<ISpecification<Customer>>(spec => spec.GetType().Name == "CustomerByVatNumberSpecification"),
                         It.IsAny<CancellationToken>()))
                    .ReturnsAsync(conflictingCustomer);
          }
          else
          {
               _mockRepository
                    .Setup(x => x.FirstOrDefaultAsync(
                         It.Is<ISpecification<Customer>>(spec => spec.GetType().Name == "CustomerByVatNumberSpecification"),
                         It.IsAny<CancellationToken>()))
                    .ReturnsAsync(null as Customer);
          }
     }

     private void VerifyTransactionCommittedOnce()
     {
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldUpdateCustomerAndReturnOk_WhenCommandIsValid()
     {
          // Arrange
          SetupRepositoryTestingIfCodeExists(false);
          SetupRepositoryTestingIfSiretExists(false);
          SetupRepositoryTestingIfVatNumberExists(false);

          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          VerifyTransactionCommittedOnce();
     }

     [Fact]
     public async Task Handle_ShouldUpdateCustomerWithCorrectInformation()
     {
          // Arrange
          SetupRepositoryTestingIfCodeExists(false);
          SetupRepositoryTestingIfSiretExists(false);
          SetupRepositoryTestingIfVatNumberExists(false);

          var originalName = _mockCustomer.Name;
          var originalCode = _mockCustomer.Code;
          var originalEmail = _mockCustomer.Email;

          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();

          // Verify the customer was updated with command values
          _mockCustomer.Name.Should().Be(_mockCommand.Name);
          _mockCustomer.Code.Should().Be(_mockCommand.Code);
          _mockCustomer.Siret.Should().Be(_mockCommand.Siret);
          _mockCustomer.Email.Should().Be(_mockCommand.Email);
          _mockCustomer.PhoneNumber.Should().Be(_mockCommand.PhoneNumber);

          // Verify original values were different (ensures update actually happened)
          _mockCustomer.Name.Should().NotBe(originalName);
          _mockCustomer.Code.Should().NotBe(originalCode);
          _mockCustomer.Email.Should().NotBe(originalEmail);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFoundIfIdDoesNotExist()
     {
          // Arrange
          SetupRepositoryTestingIfCodeExists(false);
          SetupRepositoryTestingIfSiretExists(false);
          SetupRepositoryTestingIfVatNumberExists(false);
          _mockRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Customer>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Customer);
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenNameIsEmpty()
     {
          // Arrange
          var commandWithEmptyName = _mockCommand with { Name = "" };
          SetupRepositoryTestingIfCodeExists(false);
          SetupRepositoryTestingIfSiretExists(false);
          SetupRepositoryTestingIfVatNumberExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(commandWithEmptyName, CancellationToken.None).AsTask());

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenCodeIsEmpty()
     {
          // Arrange
          var commandWithEmptyCode = _mockCommand with { Code = "" };
          SetupRepositoryTestingIfCodeExists(false);
          SetupRepositoryTestingIfSiretExists(false);
          SetupRepositoryTestingIfVatNumberExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(commandWithEmptyCode, CancellationToken.None).AsTask());

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenEmailIsEmpty()
     {
          // Arrange
          var commandWithEmptyEmail = _mockCommand with { Email = "" };
          SetupRepositoryTestingIfCodeExists(false);
          SetupRepositoryTestingIfSiretExists(false);
          SetupRepositoryTestingIfVatNumberExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(commandWithEmptyEmail, CancellationToken.None).AsTask());

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenPhoneNumberIsEmpty()
     {
          // Arrange
          var commandWithEmptyPhoneNumber = _mockCommand with { PhoneNumber = "" };
          SetupRepositoryTestingIfCodeExists(false);
          SetupRepositoryTestingIfSiretExists(false);
          SetupRepositoryTestingIfVatNumberExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(commandWithEmptyPhoneNumber, CancellationToken.None).AsTask());

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenCodeAlreadyExists()
     {
          // Arrange
          SetupRepositoryTestingIfCodeExists(true);
          SetupRepositoryTestingIfSiretExists(false);
          SetupRepositoryTestingIfVatNumberExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(_mockCommand, CancellationToken.None).AsTask());

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }
     
     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenSiretAlreadyExists()
     {
          // Arrange
          SetupRepositoryTestingIfCodeExists(false);
          SetupRepositoryTestingIfSiretExists(true);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(_mockCommand, CancellationToken.None).AsTask());

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }
     
     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenVatNumberAlreadyExists()
     {
          // Arrange
          SetupRepositoryTestingIfCodeExists(false);
          SetupRepositoryTestingIfVatNumberExists(true);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(_mockCommand, CancellationToken.None).AsTask());

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldRespectCancellationToken()
     {
          // Arrange
          var cancellationTokenSource = new CancellationTokenSource();
          await cancellationTokenSource.CancelAsync();
          SetupRepositoryTestingIfCodeExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAnyAsync<Exception>(() =>
               sut.Handle(_mockCommand, cancellationTokenSource.Token).AsTask());
     }
}
