using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;

namespace BackendTests;

public static class TestFixtureFactory
{
     public static Fixture Create()
     {
          var fixture = new Fixture();

          fixture.Customize(new FixtureCustomization());

          return fixture;
     }
}

public sealed class FixtureCustomization : ICustomization
{
     public void Customize(IFixture fixture)
     {
          fixture.Customizations.Insert(0, new AddressPhoneSpecimenBuilder());
     }
}

public sealed class AddressPhoneSpecimenBuilder : ISpecimenBuilder
{
     public object Create(object request, ISpecimenContext context)
     {
          if (request is PropertyInfo propertyInfo &&
              propertyInfo.DeclaringType == typeof(Address) &&
              propertyInfo.Name == nameof(Address.Phone))
          {
               return "0600000000";
          }

          return new NoSpecimen();
     }
}
