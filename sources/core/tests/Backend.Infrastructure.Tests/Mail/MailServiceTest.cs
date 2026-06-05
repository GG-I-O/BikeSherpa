using AutoFixture;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.Spi;
using Ggio.BikeSherpa.Backend.Infrastructure.Mail;
using JetBrains.Annotations;
using Moq;

namespace Backend.Infrastructure.Tests.Mail;

[TestSubject(typeof(MailService))]
public class MailServiceTest
{
     private readonly Fixture _fixture = new();

     private static MailService MakeSut(out Mock<IParameterRepository> parameterRepositoryMock, out Mock<IMailSender> mailSenderMock)
     {
          parameterRepositoryMock = new Mock<IParameterRepository>();
          mailSenderMock = new Mock<IMailSender>();
          return new MailService(parameterRepositoryMock.Object, mailSenderMock.Object);
     }

     [Fact]
     public async Task SendSimpleDeliveryMailToCustomerTest()
     {
          //Arrange
          var sut = MakeSut(out var parameterRepositoryMock, out var mailSenderMock);

          parameterRepositoryMock.Setup(x => x.GetSimpleDeliveryMailSubjectAsync()).Returns(ValueTask.FromResult("Test subject"));
          parameterRepositoryMock.Setup(x => x.GetSimpleDeliveryMailTemplateContent())
               .Returns(ValueTask.FromResult("{{ deliverycode }} {{ pickupaddress }} {{ destinationaddress }} {{ pickupdate }} {{ timeslot }} {{ loadingslot }} {{ options }} {{ price }} {{ comments }}"));

          var delivery = _fixture.Build<Delivery>()
               .With(x => x.Steps, [
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Pickup)
                         .Without(x => x.ParentDelivery)
                         .Create(),
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Dropoff)
                         .Without(x => x.ParentDelivery)
                         .Create()
               ])
               .Create();

          var customer = _fixture.Create<Customer>();

          //Act
          await sut.SendSimpleDeliveryMailToCustomer(delivery, customer);

          //Assert
          mailSenderMock.Verify(x => x.SendEmailAsync(customer.Email, "Test subject", It.IsAny<string>()), Times.Once);
     }

     [Trait("Category", "Integration")]
     [Fact]
     public async Task SendSimpleDeliveryMail_WithTemplate_Test()
     {
          //Arrange
          var sut = MakeSut(out var parameterRepositoryMock, out var mailSenderMock);
          using var fileReader = new StreamReader("/home/ludo/Dev/BikeSherpa/sources/core/Ggio.BikeSherpa.Backend.Infrastructure/Mail/SimpleDeliveryMail.html");
          var template = await fileReader.ReadToEndAsync(TestContext.Current.CancellationToken);
          parameterRepositoryMock.Setup(x => x.GetSimpleDeliveryMailTemplateContent()).Returns(ValueTask.FromResult(template));

          var delivery = _fixture.Build<Delivery>()
               .With(x => x.Steps, [
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Pickup)
                         .Without(x => x.ParentDelivery)
                         .Create(),
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Dropoff)
                         .Without(x => x.ParentDelivery)
                         .Create()
               ])
               .Create();


          string? resultMail = null;
          mailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask)
               .Callback((string dummy, string dummy2, string content) => { resultMail = content; });

          var customer = _fixture.Create<Customer>();

          //Act
          await sut.SendSimpleDeliveryMailToCustomer(delivery, customer);

          //Assert
          mailSenderMock.Verify(x => x.SendEmailAsync(customer.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
          if (resultMail is not null)
          {
               await using var fileWriter = new StreamWriter("simpleDeliveryMail.html");
               await fileWriter.WriteAsync(resultMail);
          }
     }

     [Trait("Category", "Integration")]
     [Fact]
     public async Task TourSimpleDeliveryMail_WithTemplate_Test()
     {
          //Arrange
          var sut = MakeSut(out var parameterRepositoryMock, out var mailSenderMock);
          using var fileReader = new StreamReader("/home/ludo/Dev/BikeSherpa/sources/core/Ggio.BikeSherpa.Backend.Infrastructure/Mail/TourDeliveryMail.html");
          var template = await fileReader.ReadToEndAsync(TestContext.Current.CancellationToken);
          parameterRepositoryMock.Setup(x => x.GetTourDeliveryMailTemplateContent()).Returns(ValueTask.FromResult(template));

          var delivery = _fixture.Build<Delivery>()
               .With(x => x.Steps, [
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Pickup)
                         .Without(x => x.ParentDelivery)
                         .Create(),
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Dropoff)
                         .Without(x => x.ParentDelivery)
                         .Create(),
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Dropoff)
                         .Without(x => x.ParentDelivery)
                         .Create(),
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Dropoff)
                         .Without(x => x.ParentDelivery)
                         .Create(),
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Dropoff)
                         .Without(x => x.ParentDelivery)
                         .Create(),
                    _fixture.Build<DeliveryStep>()
                         .With(x => x.StepType, StepType.Dropoff)
                         .Without(x => x.ParentDelivery)
                         .Create()
               ])
               .Create();


          string? resultMail = null;
          mailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask)
               .Callback((string dummy, string dummy2, string content) => { resultMail = content; });

          var customer = _fixture.Create<Customer>();

          //Act
          await sut.SendTourDeliveryMailToCustomer(delivery, customer);

          //Assert
          mailSenderMock.Verify(x => x.SendEmailAsync(customer.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
          if (resultMail is not null)
          {
               await using var fileWriter = new StreamWriter("tourDeliveryMail.html");
               await fileWriter.WriteAsync(resultMail);
          }
     }
}
