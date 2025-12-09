using Ggio.BikeSherpa.Backend.Domain.ClientAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
     public void Configure(EntityTypeBuilder<Client> builder)
     {
          builder.HasKey(x => x.Id);
     }
}
