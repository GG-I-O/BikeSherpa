
### Realistic SUT
```cs
private AddDeliveryStepHandler CreateRealisticSut()
{
var validator = new AddDeliveryStepCommandValidator();

          var dbContextBuilder = new DbContextOptionsBuilder<BackendDbContext>();
          dbContextBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
          var dbContext = new BackendDbContext(dbContextBuilder.Options);

          var mockedMediator = new Mock<IMediator>();
          var eventDispatcher = new MediatorDomainEventDispatcher(mockedMediator.Object);

          var applicationTransactionContext = new ApplicationTransactionContext();
          
          var transaction = new ApplicationTransaction<BackendDbContext>(dbContext, applicationTransactionContext ,eventDispatcher);
          
          return new AddDeliveryStepHandler(
               validator,
               transaction,
               new EfCoreReadRepository<Delivery>(dbContext),
               _mockDeliveryZoneRepository.Object,
               _mockPricingStrategyService.Object,
               _mockItineraryApi.Object
          );
     }
```